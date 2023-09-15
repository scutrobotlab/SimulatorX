using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Controllers.Child;
using Controllers.Components;
using Controllers.Items;
using Gameplay;
using Gameplay.Attribute;
using Gameplay.Customize;
using Gameplay.Events;
using Infrastructure;
using Mirror;
using Misc;
using UI;
using UnityEngine;
using UnityEngine.InputSystem;
using Random = UnityEngine.Random;

namespace Controllers
{
    /// <summary>
    /// 无人机控制器组件。
    /// 实现无人机的多轴姿态、飞行控制、云台手功能等。
    /// </summary>
    public class DroneStore : RobotStoreBase
    {
        // 云台组件
        [Header("PTZ")] public Transform yaw;

        public Transform pitch;
        public Transform droneTransform;

        // 自动瞄准
        [Header("Aimbot")] public Camera aimbotCamera;

        //地图
        [SerializeField] private Camera map;
        [SerializeField] private bool clickEnable = true;
        [SyncVar] public Vector3 pos;

        // 发弹
        [Header("Fire")] public GameObject bullet;

        public Transform muzzle;

        //本次空中支援结束时间
        public double droneSupportTime;
        public bool droneFly;
        public bool freeCount;
        private readonly ToggleHelper _aimbotHelper = new ToggleHelper();

        //飞镖发射
        private readonly ToggleHelper _dartHelper = new ToggleHelper();

        //空中支援
        private readonly ToggleHelper _droneHelper = new ToggleHelper();
        private readonly ToggleHelper _mapCallHelper = new ToggleHelper();
        private readonly ToggleHelper _rightClickHelper = new ToggleHelper();

        //哨兵启动
        private readonly ToggleHelper _stopHelper = new ToggleHelper();

        private Aimbot _aimbot;

        // 机身组件
        [Header("DroneChassis")] private DroneChassis _droneChassis;

        private Gun _gun;


        RaycastHit _HitInfo = new RaycastHit();
        [SyncVar] private bool _mapOff = true;


        private Vector2 _primaryAxisInput;
        private Ptz _ptz;
        private Vector2 _secondaryAxisInput;

        /// <summary>
        /// 组件初始化。
        /// </summary>
        protected override void Start()
        {
            base.Start();
            if (isServer)
            {
                _droneChassis = new DroneChassis(gameObject, yaw);
                _ptz = new Ptz(yaw, pitch, GetComponent<Rigidbody>());
                _aimbot = new Aimbot(aimbotCamera, multiplier: 2e2f);
                _gun = new Gun(
                    MechanicType.CaliberType.Small,
                    muzzle, SyncLaunch,
                    30, 10000, 10000, true);
                _gun.IsDrone = true;
                freeCount = false;
            }
        }

        /// <summary>
        /// 更新组件。
        /// </summary>
        protected override void FixedUpdate()
        {
            //哨兵控制
            if (isClient && clickEnable)
            {
                if (map == null)
                    map = GameObject.Find("MapCamera").GetComponent<Camera>();

                if (!_mapOff && id == EntityManager.Instance().LocalRobot())
                {
                    if (_rightClickHelper.Toggle(Mouse.current.rightButton.isPressed) == ToggleHelper.State.De)
                    {
                        var ray = map.ScreenPointToRay(Mouse.current.position
                            .ReadValue());
                        if (Physics.Raycast(ray.origin, ray.direction, out _HitInfo))
                            pos = _HitInfo.point;
                        PosUpload(pos, id);
                        clickEnable = false;
                        StartCoroutine(ClickDelayEnable());
                    }
                }
            }

            base.FixedUpdate();
            if (!isServer) return;

            if (map == null)
                map = GameObject.Find("MapCamera").GetComponent<Camera>();

            _ptz.Update();
            _gun.Update(Time.deltaTime);
            Vector3 p = transform.position;


            if (transform.position.y > 8)
            {
                p.y = 8;
                transform.position = p;
            }

            if (transform.position.y < -1f)
            {
                p.y = -1f;
                transform.position = p;
            }

            if (transform.position.z < -9f)
            {
                p.z = -9f;
                transform.position = p;
            }

            if (transform.position.z > 9f)
            {
                p.y = 9f;
                transform.position = p;
            }

            if (transform.position.x < -13.4f)
            {
                p.x = -13.4f;
                transform.position = p;
            }

            if (transform.position.x > 13.4f)
            {
                p.x = 13.4f;
                transform.position = p;
            }

            if (droneFly)
            {
                var supportTime = NetworkTime.time - droneSupportTime;
                if (supportTime > 0)
                {
                    Dispatcher.Instance().Send(new AgreeDroneSupport
                    {
                        Camp = id.camp,
                        Agree = false
                    });
                    magSmall = 0;
                }
            }
        }

        /// <summary>
        /// 声明接收事件。
        /// </summary>
        /// <returns></returns>
        public override List<string> InputActions()
        {
            return base.InputActions().Concat(new List<string>
            {
                ActionID.Input.PrimaryAxis,
                ActionID.Input.ViewControl,
                ActionID.Input.StateControl,
                //ActionID.DroneControl.UpdateDrone,
                ActionID.Input.SecondaryAxis,
                ActionID.DroneControl.AgreeDroneSupport,
                ActionID.DroneControl.StartDroneCount,
                ActionID.DroneControl.AutoDriveTarget
            }).ToList();
        }

        /// <summary>
        /// 处理事件。
        /// </summary>
        /// <param name="action">事件</param>
        [Server]
        public override void Receive(IAction action)
        {
            base.Receive(action);
            if (!isServer) return;
            switch (action.ActionName())
            {
                /*case ActionID.DroneControl.UpdateDrone:
                    var updateDroneAction = (UpdateDrone) action;
                    if (updateDroneAction.Camp == id.camp)
                    {
                        _primaryAxisInput = updateDroneAction.primaryAxisInput;
                        Vector3 primary;
                        primary.x = _primaryAxisInput.x;
                        primary.z = _primaryAxisInput.y;
                        primary.y = 0;
                        float angle = Vector3.Angle(yaw.right,droneTransform.right);
                        float judge = Vector3.Dot(yaw.right, droneTransform.right);
                        float judge2 = Vector3.Cross(yaw.right, droneTransform.right).y;
                        Debug.Log("前后"+judge);
                        Debug.Log("左右"+judge2);
                        //Debug.Log(180-angle);
                        Quaternion a;
                        if (180-angle>10)
                        {
                            if (judge2 < 0&&judge<0)
                            {
                                a = Quaternion.AngleAxis( -angle-45, yaw.up) ;
                                primary = a * primary;
                                Debug.DrawRay(yaw.position, primary, Color.red);
                                _primaryAxisInput.x = primary.x;
                                _primaryAxisInput.y = primary.z;
                            }
                            else if (judge2<0&&judge>0)
                            {
                                _primaryAxisInput.x=-_primaryAxisInput.x;
                                _primaryAxisInput.y = -_primaryAxisInput.y;
                            }
                            else if (judge2>0&&judge>0)
                            {
                                a = Quaternion.AngleAxis( -angle-45, yaw.up) ;
                                primary = a * primary;
                                Debug.DrawRay(yaw.position, primary, Color.red);
                                _primaryAxisInput.x = primary.x;
                                _primaryAxisInput.y = primary.z;
                            }

                            else
                            {
                                _primaryAxisInput = updateDroneAction.primaryAxisInput;
                            }

                        }
                        else
                        {
                            _primaryAxisInput = updateDroneAction.primaryAxisInput;
                        }
                        _droneChassis.Move(_primaryAxisInput);
                        _secondaryAxisInput = updateDroneAction.secondAxisInput;
                        _droneChassis.Rotate(_secondaryAxisInput);
                    }
                    break;*/

                case ActionID.DroneControl.AutoDriveTarget:
                    var auto = (AutoDriveTarget)action;
                    if (auto.Camp == id.camp)
                        pos = auto.Pos;

                    break;

                case ActionID.DroneControl.AgreeDroneSupport:
                    var agreeDroneSupportAction = (AgreeDroneSupport)action;
                    if (agreeDroneSupportAction.Timestart)
                    {
                        droneSupportTime = agreeDroneSupportAction.StopTime;
                    }

                    if (agreeDroneSupportAction.Camp == id.camp)
                    {
                        if (agreeDroneSupportAction.Agree)
                        {
                            droneSupportTime = agreeDroneSupportAction.StopTime;
                            droneFly = true;
                            magSmall = 500;
                        }

                        if (!agreeDroneSupportAction.Agree)
                        {
                            droneFly = false;
                        }
                    }

                    AgreeDroneSupportRpc(agreeDroneSupportAction);
                    break;

                //开局开始倒计时
                case ActionID.DroneControl.StartDroneCount:
                    freeCount = true;
                    StartFreeCountRpc();
                    break;

                case ActionID.Input.PrimaryAxis:
                    var primaryAxisAction = (PrimaryAxis)action;
                    if (primaryAxisAction.Receiver == id)
                    {
                        _primaryAxisInput = new Vector2(primaryAxisAction.X, primaryAxisAction.Y);
                        // _droneChassis.Move(_primaryAxisInput);
                        // _droneChassis.Buoyancy(_primaryAxisInput, _secondaryAxisInput);
                        /*Dispatcher.Instance().Send(new UpdateDrone
                        {
                            Camp = id.camp,
                            primaryAxisInput = _primaryAxisInput
                        });*/
                    }

                    break;

                case ActionID.Input.SecondaryAxis:
                    var secondaryAxisAction = (SecondaryAxis)action;
                    if (secondaryAxisAction.Receiver == id)
                    {
                        _secondaryAxisInput = new Vector2(secondaryAxisAction.X, secondaryAxisAction.Y);
                        _droneChassis.Rotate(_secondaryAxisInput);
                        /*Dispatcher.Instance().Send(new UpdateDrone
                        {
                            Camp = id.camp,
                            secondAxisInput = _secondaryAxisInput
                        });*/
                    }

                    break;

                case ActionID.Input.ViewControl:
                    var viewControlAction = (ViewControl)action;
                    if (viewControlAction.Receiver == id)
                    {
                        //控制云台视角
                        _ptz.View(
                            Time.deltaTime,
                            !_aimbot.IsTracking()
                                ? new Vector2(viewControlAction.X, viewControlAction.Y)
                                : _aimbot.Update(Time.fixedDeltaTime),
                            Vector2.one);
                    }

                    break;

                case ActionID.Input.StateControl:
                    var stateControlAction = (StateControl)action;
                    if (stateControlAction.Receiver == id)
                    {
                        //强制修正
                        Vector3 offset = new Vector3(-0.002125435f, -0.008881041f, 0.01224007f);
                        muzzle.localPosition = offset;
                        muzzle.rotation = fpvCamera.transform.rotation;
                        // 开火
                        if (stateControlAction.Fire && droneFly)
                        {
                            _gun.Trigger();
                        }
                        else
                        {
                            _gun.Release();
                        }

                        // 按M地图呼出
                        switch (_mapCallHelper.Toggle(stateControlAction.FunctionN))
                        {
                            case ToggleHelper.State.Re:
                                CameraUpdateRpc(!_mapOff);
                                _mapOff = !_mapOff;
                                break;

                            case ToggleHelper.State.De:
                                CameraUpdateRpc(!_mapOff);
                                _mapOff = !_mapOff;
                                break;


                            case ToggleHelper.State.Hold:
                                break;
                            default:
                                throw new ArgumentOutOfRangeException();
                        }

                        // 辅助瞄准
                        switch (_aimbotHelper.Toggle(stateControlAction.SecondaryFire))
                        {
                            case ToggleHelper.State.Re:
                                StartContinuousAimbotSession();
                                break;
                            case ToggleHelper.State.De:
                                _aimbot.EndSession();
                                break;
                        }

                        // 暂停和启动哨兵
                        switch (_stopHelper.Toggle(stateControlAction.FunctionA))
                        {
                            case ToggleHelper.State.Re:
                                Dispatcher.Instance().Send(new StopSential
                                {
                                    Camp = id.camp,
                                    stop = true,
                                });
                                break;
                            case ToggleHelper.State.De:
                                Dispatcher.Instance().Send(new StopSential
                                {
                                    Camp = id.camp,
                                    stop = false,
                                });
                                break;
                            case ToggleHelper.State.Hold:
                                break;
                            default:
                                throw new ArgumentOutOfRangeException();
                        }

                        // 空中支援
                        switch (_droneHelper.Toggle(stateControlAction.FunctionB))
                        {
                            case ToggleHelper.State.Re:
                                if (NetworkTime.time - droneSupportTime > 0)
                                {
                                    Dispatcher.Instance().Send(new RequestDroneSupport
                                    {
                                        Camp = id.camp,
                                        RequestTime = NetworkTime.time - droneSupportTime
                                    });
                                    break;
                                }
                                else
                                {
                                    break;
                                }

                            case ToggleHelper.State.De:
                                if (NetworkTime.time - droneSupportTime > 0)
                                {
                                    Dispatcher.Instance().Send(new RequestDroneSupport
                                    {
                                        Camp = id.camp,
                                        RequestTime = NetworkTime.time - droneSupportTime
                                    });
                                    break;
                                }
                                else
                                {
                                    break;
                                }

                            case ToggleHelper.State.Hold:
                                break;
                            default:
                                throw new ArgumentOutOfRangeException();
                        }

                        //飞镖发射

                        //未开舱门时，按下L为切换目标
                        if (!stateControlAction.FunctionC)
                        {
                            switch (_dartHelper.Toggle(stateControlAction.FunctionE))
                            {
                                case ToggleHelper.State.Re:
                                    Dispatcher.Instance().Send(new ToBase());
                                    break;
                                case ToggleHelper.State.De:
                                    Dispatcher.Instance().Send(new ToOutpost());
                                    break;
                                case ToggleHelper.State.Hold:
                                    break;
                                default:
                                    throw new ArgumentOutOfRangeException();
                            }
                        }

                        if (stateControlAction.FunctionC)
                        {
                            //打开舱门
                            Dispatcher.Instance().Send(new OpenLaunchStation()
                            {
                                Camp = id.camp,
                                Role = Identity.Roles.LaunchingStation
                            });

                            switch (_dartHelper.Toggle(stateControlAction.FunctionE))
                            {
                                case ToggleHelper.State.Re:
                                    Dispatcher.Instance().Send(new DartFire()
                                    {
                                        Camp = id.camp,
                                        Role = Identity.Roles.LaunchingStation,
                                        Error = CustomizeManager.Instance()
                                            .Data(id, CustomizeProperties.DartError.DartErr),
                                    });
                                    break;
                                case ToggleHelper.State.De:
                                    Dispatcher.Instance().Send(new DartFire()
                                    {
                                        Camp = id.camp,
                                        Role = Identity.Roles.LaunchingStation,
                                        Error = CustomizeManager.Instance()
                                            .Data(id, CustomizeProperties.DartError.DartErr),
                                    });
                                    break;
                                case ToggleHelper.State.Hold:
                                    break;
                                default:
                                    throw new ArgumentOutOfRangeException();
                            }
                        }

                        _gun.ToggleFullAuto(stateControlAction.FunctionG);
                    }

                    break;
            }
        }

        /// <summary>
        /// 弹丸发射。
        /// </summary>
        /// <param name="velocity">弹丸初速</param>
        /// <returns></returns>
        [Command(requiresAuthority = false)]
        private void PosUpload(Vector3 uploadPos, Identity posId)
        {
            Dispatcher.Instance().Send(new AutoDriveTarget()
            {
                Pos = uploadPos,
                Camp = posId.camp
            });
        }

        /// <summary>
        /// 同步发射弹丸。
        /// </summary>
        /// <param name="velocity">弹丸初速</param>
        /// <param name="gunMuzzle">枪口位置</param>
        /// <returns></returns>
        [Server]
        private bool SyncLaunch(Vector3 velocity)
        {
            if (magSmall <= 0) return false;
            magSmall--;
            // 弹速波动
            var jitter = CustomizeManager.Instance().Data(
                id,
                CustomizeProperties.Gun.MuzzleVelocity);
            var realVelocity = velocity * (1 + Random.Range(0, jitter - 1) * (Random.Range(0, 2) >= 1 ? 1 : -1));
            // 水平与垂直弹道波动
            var hbj = CustomizeManager.Instance()
                .Data(
                    id,
                    CustomizeProperties.Gun.HorizontalBallisticJitter) + 0.6f;
            var vbj = CustomizeManager.Instance()
                .Data(
                    id,
                    CustomizeProperties.Gun.VerticalBallisticJitter) + 0.3f;

            realVelocity += muzzle.right * (Random.Range(0, hbj - 1) * (Random.Range(0, 2) == 1 ? 1 : -1));
            realVelocity += muzzle.up * (Random.Range(0, vbj - 1) * (Random.Range(0, 2) == 1 ? 1 : -1));

            //强制修正
            if (Vector2.Dot(new Vector2(realVelocity.x, realVelocity.z),
                    new Vector2(muzzle.forward.x, muzzle.forward.z)) <= 0)
                realVelocity = new Vector3(-realVelocity.x, realVelocity.y, -realVelocity.z);

            if (isServerOnly)
            {
                var newBullet = Instantiate(bullet, muzzle.position, Quaternion.identity);
                newBullet.GetComponent<Bullet>().owner = id;
                newBullet.GetComponent<Rigidbody>().velocity = realVelocity;
            }

            // 未考虑带客户端的 Host
            LaunchRpc(realVelocity);
            return true;
        }

        /// <summary>
        /// 在客户端生成弹丸。
        /// </summary>
        /// <param name="velocity">弹丸初速</param>
        /// <param name="gunMuzzle">枪口位置</param>
        [ClientRpc]
        private void LaunchRpc(Vector3 velocity)
        {
            var newBullet = Instantiate(bullet, muzzle.position, Quaternion.identity);
            //Debug.Log(gunMuzzle);
            //Debug.Log(aimbotCamera.transform.position);
            newBullet.GetComponent<Bullet>().owner = id;
            newBullet.GetComponent<Rigidbody>().velocity = velocity;
        }


        /// <summary>
        /// 在客户端同步摄像头。
        /// </summary>
        [Server]
        private void CameraUpdateRpc(bool isOff = true)
        {
            if (map == null)
                map = GameObject.Find("MapCamera").GetComponent<Camera>();

            if (isOff)
            {
                fpvCamera.enabled = isOff;
                map.enabled = !isOff;
            }
            else
            {
                fpvCamera.enabled = isOff;
                map.enabled = !isOff;
            }

            // Debug.Log(isOff);
            CameraUpdate(isOff);
        }

        [ClientRpc]
        private void CameraUpdate(bool isOff)
        {
            if (id != EntityManager.Instance().LocalRobot())
                return;

            if (map == null)
            {
                map = GameObject.Find("MapCamera").GetComponent<Camera>();
            }

            if (isOff)
            {
                fpvCamera.enabled = isOff;
                map.enabled = !isOff;
            }
            else
            {
                fpvCamera.enabled = isOff;
                map.enabled = !isOff;
            }

            // Debug.Log(isOff);
            UIManager.SetCursorLock(isOff);

            UIManager.Instance().GetPanelUc().MapControl(isOff);
        }

        /// <summary>
        /// 在客户端同步起飞信息。
        /// </summary>
        /// <param name="action">起飞事件</param>
        [ClientRpc]
        private void AgreeDroneSupportRpc(AgreeDroneSupport action)
        {
            //非本地不开摄像头
            /*if(EntityManager.Instance().LocalRobot().camp != id .camp)
                return;*/

            if (action.Timestart)
            {
                droneSupportTime = action.StopTime;
            }

            if (action.Camp == id.camp)
            {
                if (action.Agree)
                {
                    droneSupportTime = action.StopTime;
                    droneFly = true;
                    magSmall = 500;
                }

                if (!action.Agree)
                {
                    droneFly = false;
                }
            }
        }

        /// <summary>
        /// 在客户端同步倒计时。
        /// </summary>
        /// <param name="action">起飞事件</param>
        [ClientRpc]
        private void StartFreeCountRpc()
        {
            freeCount = true;
        }

        /// <summary>
        /// 尝试寻找下一个目标。
        /// </summary>
        private void StartContinuousAimbotSession()
        {
            // 弹道解算与运动预测水平
            var bL = CustomizeManager.Instance().Data(
                id,
                CustomizeProperties.Aimbot.Ballistic);
            var pL = CustomizeManager.Instance().Data(
                id,
                CustomizeProperties.Aimbot.Prediction);
            _aimbot.StartSession<Armor>(
                bL switch
                {
                    0 => Aimbot.BallisticLevel.None,
                    1 => Aimbot.BallisticLevel.Parabola,
                    _ => Aimbot.BallisticLevel.None
                },
                pL switch
                {
                    0 => Aimbot.PredictionLevel.VectorAndPowerRune,
                    1 => Aimbot.PredictionLevel.VectorAndPowerRune,
                    2 => Aimbot.PredictionLevel.VectorAndPowerRune,
                    _ => Aimbot.PredictionLevel.VectorAndPowerRune
                },
                /* 辅助瞄准目标识别 */
                /* TODO：搬走 */
                target =>
                {
                    Armor armor;
                    if (!(target is Armor armorComp))
                    {
                        armor = ((GameObject)target).GetComponent<Armor>();
                    }
                    else
                    {
                        armor = armorComp;
                    }

                    // 识别灵敏度
                    // TODO：失效

                    if (armor.camp == id.camp) return false;

                    if ((armor.transform.position - transform.position).magnitude > 9) return false;

                    var facing = Vector3.Dot(armor.transform.up, muzzle.forward) < -0.5f;

                    return armor.lightOn && facing;
                },
                TryAimbotLater,
                () =>
                {
                    // _gun.Trigger();
                    // _gun.Release();
                });
        }

        /// <summary>
        /// 延迟重试锁定。
        /// </summary>
        private void TryAimbotLater()
        {
            StartCoroutine(StartDelayAimbot());
        }

        /// <summary>
        /// 延迟重试锁定的协程实现。
        /// </summary>
        /// <returns></returns>
        private IEnumerator StartDelayAimbot()
        {
            yield return new WaitForSeconds(0.2f);
            StartContinuousAimbotSession();
        }

        private IEnumerator ClickDelayEnable()
        {
            yield return new WaitForSeconds(3f);
            clickEnable = true;
        }
    }
}