using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Controllers.Child;
using Controllers.Components;
using Controllers.Items;
using DG.Tweening;
using Gameplay;
using Gameplay.Attribute;
using Gameplay.Customize;
using Gameplay.Events;
using Gameplay.Events.Child;
using Infrastructure;
using Infrastructure.Child;
using Mirror;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Controllers
{
    /// <summary>
    /// <c>SentinelStore</c> 控制哨兵机器人。
    /// <br/>执行巡逻、自动打击目标。
    /// </summary>
    public class SentinelStore : RobotStoreBase
    {
        [Header("上云台")] 
        public Transform upperYaw;
        public Transform upperPitch;
        private Ptz _upperPtz;
        [Header("下云台")] 
        public Transform lowerYaw;
        public Transform lowerPitch;
        private Ptz _lowerPtz;

        // 自动瞄准
        public Camera lowerAimCam;
        private Aimbot _lowerAim;
        public Camera upperAimCam;
        private Aimbot _upperAim;

        // 自动射击
        public GameObject bullet;
        private Gun _lowerGun;
        private Gun _upperGun;

        // 往返移动
        private Vector3 _initialPosition;
        private float _fullRailLength;
        private float _fullMoveTime;
        private Coroutine _movement;
        public int lengthSection = 5;
        private int _currentPosition;


        // 目标机器人
        private Armor _lowerTarget;
        private Armor _upperTarget;

        // Pitch 轴扫描
        private float _pitchScanTime;

        /// <summary>
        /// 在编辑器中设置。
        /// </summary>
        protected override void Identify()
        {
        }
        
        /// <summary>
        /// 声明接收事件。
        /// </summary>
        /// <returns></returns>
        public override List<string> InputActions()
        {
            return base.InputActions().Concat(new List<string>
            {
                ActionID.Recorder.SentinelMove,
                ActionID.SentialControl.ChangeSential,
                ActionID.SentialControl.StopSential
            }).ToList();
        }
        
        /// <summary>
        /// 处理事件。
        /// </summary>
        /// <param name="action">事件</param>
        public override void Receive(IAction action)
        {
            base.Receive(action);
            if (!isServer) return;
            switch (action.ActionName())
            {
                case ActionID.Recorder.SentinelMove:
                    var moveAction = (SentinelMove) action;
                    if (moveAction.Receiver == id)
                    {
                        MoveTo(moveAction.CurrentPosition, moveAction.Duration);
                    }
                    break;
                
                case ActionID.SentialControl.StopSential:
                    var stopAction = (StopSential) action;
                    if (stopAction.Camp == id.camp && health != 0)
                    {
                        switch (stopAction.stop)
                        {
                            case true:
                                StopCoroutine(_movement);
                                break;
                            case false:
                                _movement = StartCoroutine(PlanMove());
                                break;
                        }
                    }
                    break;
            }
        }

        /// <summary>
        /// RMUL 运动范围。
        /// </summary>
        /// <param name="mapType"></param>
        protected override void InitializeByMapType(MapType mapType)
        {
            base.InitializeByMapType(mapType);
            if (mapType == MapType.RMUL2022)
            {
                _fullRailLength = 1.6f;
                _fullMoveTime = 2f;
            }
            _movement = StartCoroutine(PlanMove());
        }

        // 只在服务端运行的逻辑

        /// <summary>
        /// 初始化组件及参数。
        /// </summary>
        protected override void Start()
        {
            base.Start();
            if (!isServer) return;
            _upperPtz = new Ptz(upperYaw, upperPitch, null, -85, 10);
            _lowerPtz = new Ptz(lowerYaw, lowerPitch, null, -85, 10);
            _upperAim = new Aimbot(upperAimCam, multiplier: 2e2f);
            _lowerAim = new Aimbot(lowerAimCam, multiplier: 2e2f);
            _lowerGun = new Gun(
                MechanicType.CaliberType.Small,
                lowerAimCam.transform, LowerMuzzleLaunch,
                30, 320, 100);
            _upperGun = new Gun(
                MechanicType.CaliberType.Small,
                upperAimCam.transform, UpperMuzzleLaunch,
                30, 320, 100);

            _initialPosition = transform.position;
            _fullRailLength = 3.15f;
            _fullMoveTime = 3.5f;
            SetVisual();
        }

        /// <summary>
        /// 自动扫描和锁定射击。
        /// </summary>
        protected override void FixedUpdate()
        {
            base.FixedUpdate();
            if (!isServer) return;

            if (health == 0)
            {
                StopCoroutine(_movement);
                for (var i = 0; i < 2; i++)
                {
                    Dispatcher.Instance().SendChild(new TurnArmor
                    {
                        ReceiverChild = new ChildIdentity(ChildType.Armor, i),
                        IsOn = false
                    }, id);
                }

                _lowerAim.EndSession();
                _upperAim.EndSession();
                return;
            }
            if(health != 0)
            {
                
                _pitchScanTime += Time.deltaTime;
            var scanUp = _pitchScanTime - Math.Floor(_pitchScanTime) > 0.5;
            var scanMultiplier = Vector2.one * 200;
            
            if (!_lowerAim.IsTracking())
            {
                _lowerPtz.View(Time.fixedDeltaTime, Vector2.right, scanMultiplier);
                _lowerPtz.View(Time.fixedDeltaTime, scanUp ? Vector2.up : Vector2.down, scanMultiplier);
                StartContinuousAimbotSession();
            }
            else
            {
                _lowerPtz.View(Time.fixedDeltaTime, _lowerAim.Update(Time.fixedDeltaTime, true), Vector2.one);
            }

            if (!_upperAim.IsTracking())
            {
                _upperPtz.View(Time.fixedDeltaTime, Vector2.left, scanMultiplier);
                _upperPtz.View(Time.fixedDeltaTime, scanUp ? Vector2.up : Vector2.down, scanMultiplier);
                StartContinuousAimbotSession();
            }
            else
            {
                _upperPtz.View(Time.fixedDeltaTime, _upperAim.Update(Time.fixedDeltaTime, true), Vector2.one);
            }

            _upperPtz.Update();
            _lowerPtz.Update();

            _lowerGun.Update(Time.deltaTime);
            _upperGun.Update(Time.deltaTime);
                
            }
            
        }

        /// <summary>
        /// 下云台目标筛选函数。
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        private bool LowerFilterTarget(object target)
        {
            if (!(target is Armor armorComp))
            {
                _lowerTarget = ((GameObject) target).GetComponent<Armor>();
            }
            else
            {
                _lowerTarget = armorComp;
            }

            if (_lowerTarget.camp == id.camp) return false;

            var robotStore = _lowerTarget.GetComponentInParent<RobotStoreBase>();
            if (robotStore == null) return false;
            if (!robotStore.id.IsGroundRobot()) return false;
            if (robotStore.id.role == Identity.Roles.Engineer && robotStore.health > 50) return false;
            if (robotStore.HasEffect(EffectID.Buffs.Revival)) return false;

            var maxRecognitionDistance = EntityManager.Instance().CurrentMap() == MapType.RMUC2022 ? 12 : 8;
            if ((transform.position - _lowerTarget.transform.position).magnitude > maxRecognitionDistance) return false;

            var facing = Vector3.Dot(_lowerTarget.transform.up, lowerAimCam.transform.forward) < -0.8f;

            return _lowerTarget.lightOn && facing;
        }

        /// <summary>
        /// 上云台目标筛选函数。
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        private bool UpperFilterTarget(object target)
        {
            if (!(target is Armor armorComp))
            {
                _upperTarget = ((GameObject) target).GetComponent<Armor>();
            }
            else
            {
                _upperTarget = armorComp;
            }

            if (_upperTarget.camp == id.camp) return false;

            var robotStore = _upperTarget.GetComponentInParent<RobotStoreBase>();
            if (robotStore == null) return false;
            if (!robotStore.id.IsGroundRobot()) return false;
            if (robotStore.id.role == Identity.Roles.Engineer && robotStore.health > 50) return false;
            if (robotStore.HasEffect(EffectID.Buffs.Revival)) return false;

            var maxRecognitionDistance = EntityManager.Instance().CurrentMap() == MapType.RMUC2022 ? 12 : 8;
            if ((transform.position - _upperTarget.transform.position).magnitude > maxRecognitionDistance) return false;

            var facing = Vector3.Dot(_upperTarget.transform.up, lowerAimCam.transform.forward) < -0.8f;

            return _upperTarget.lightOn && facing;
        }

        /// <summary>
        /// 连续索敌。
        /// </summary>
        private void StartContinuousAimbotSession()
        {
            if (!_lowerAim.IsTracking())
            {
                _lowerAim.StartSession<Armor>(
                    Aimbot.BallisticLevel.Parabola,
                    Aimbot.PredictionLevel.VectorAndPowerRune,
                    LowerFilterTarget, StartContinuousAimbotSession, () =>
                    {
                        _lowerGun.Trigger();
                        _lowerGun.Release();
                    });
            }

            if (!_upperAim.IsTracking())
            {
                _upperAim.StartSession<Armor>(
                    Aimbot.BallisticLevel.Parabola,
                    Aimbot.PredictionLevel.VectorAndPowerRune,
                    UpperFilterTarget, StartContinuousAimbotSession, () =>
                    {
                        _upperGun.Trigger();
                        _upperGun.Release();
                    });
            }
        }

        /// <summary>
        /// 计划移动。
        /// </summary>
        /// <returns></returns>
        private IEnumerator PlanMove()
        {
            while (true)
            {
                var moveLength = Random.Range(-lengthSection, lengthSection);
                while (_currentPosition + moveLength < 0
                       || _currentPosition + moveLength >= lengthSection)
                {
                    moveLength = Random.Range(-lengthSection, lengthSection);
                }

                _currentPosition += moveLength;

                var duration = Mathf.Abs(_fullMoveTime / lengthSection * moveLength + 0.02f);
                Dispatcher.Instance().Send(new SentinelMove
                {
                    Receiver = id,
                    CurrentPosition = _currentPosition,
                    Duration = duration
                });
                yield return new WaitForSeconds(duration);
            }
        }
        

        /// <summary>
        /// 移动到轨道指定位置。
        /// </summary>
        /// <param name="pos">在轨道上的位置比例</param>
        /// <param name="duration">移动时间</param>
        private void MoveTo(int pos, float duration)
        {
            transform.DOMove(
                _initialPosition + transform.forward * (_fullRailLength * pos / lengthSection),
                duration);
        }

        /// <summary>
        /// 下云台弹丸发射。
        /// </summary>
        /// <param name="velocity">弹丸初速</param>
        /// <returns></returns>
        [Server]
        private bool LowerMuzzleLaunch(Vector3 velocity)
        {
            return SyncLaunch(velocity, lowerAimCam.transform);
        }

        /// <summary>
        /// 上云台弹丸发射。
        /// </summary>
        /// <param name="velocity">弹丸初速</param>
        /// <returns></returns>
        [Server]
        private bool UpperMuzzleLaunch(Vector3 velocity)
        {
            return SyncLaunch(velocity, upperAimCam.transform);
        }

        /// <summary>
        /// 同步发射弹丸。
        /// </summary>
        /// <param name="velocity">弹丸初速</param>
        /// <param name="muzzle">枪口位置</param>
        /// <returns></returns>
        [Server]
        private bool SyncLaunch(Vector3 velocity, Transform muzzle)
        {
            if (magSmall <= 0) return false;
            magSmall--;
            // 弹速波动
            var jitter = CustomizeManager.Instance().Data(
                id,
                CustomizeProperties.Gun.MuzzleVelocity);
            var realVelocity = velocity * (1 + Random.Range(0, jitter - 1) * (Random.Range(0, 2) == 1 ? 1 : -1));
            // 水平与垂直弹道波动
            var hbj = CustomizeManager.Instance()
                .Data(
                    id,
                    CustomizeProperties.Gun.HorizontalBallisticJitter);
            var vbj = CustomizeManager.Instance()
                .Data(
                    id,
                    CustomizeProperties.Gun.VerticalBallisticJitter);

            realVelocity += muzzle.right * (Random.Range(0, hbj - 1) * (Random.Range(0, 2) == 1 ? 1 : -1));
            realVelocity += muzzle.up * (Random.Range(0, vbj - 1) * (Random.Range(0, 2) == 1 ? 1 : -1));

            if (isServerOnly)
            {
                var newBullet = Instantiate(bullet, muzzle.position, Quaternion.identity);
                newBullet.GetComponent<Bullet>().owner = id;
                newBullet.GetComponent<Rigidbody>().velocity = realVelocity;
            }

            // 未考虑带客户端的 Host
            LaunchRpc(realVelocity, muzzle.position);
            return true;
        }

        /// <summary>
        /// 在客户端生成弹丸。
        /// </summary>
        /// <param name="velocity">弹丸初速</param>
        /// <param name="muzzle">枪口位置</param>
        [ClientRpc]
        private void LaunchRpc(Vector3 velocity, Vector3 muzzle)
        {
            var newBullet = Instantiate(bullet, muzzle, Quaternion.identity);
            newBullet.GetComponent<Bullet>().owner = id;
            newBullet.GetComponent<Rigidbody>().velocity = velocity;
        }
    }
}