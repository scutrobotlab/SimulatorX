using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Controllers.Child;
using Controllers.Components;
using Controllers.Items;
using Controllers.Ore;
using DG.Tweening;
using Gameplay;
using Gameplay.Customize;
using Gameplay.Events;
using Infrastructure;
using Mirror;
using Misc;
using UnityEngine;

namespace Controllers
{
    /// <summary>
    /// <c>Engineer</c> 控制工程机器人。
    /// <br/>可控制移动、鼠标控制旋转、夹取矿石。
    /// </summary>
    public class EngineerStore : RobotStoreBase
    {
        [Header("Take mine")]
        // 抬升机构
        public GameObject lift;

        //矿石位置
        public GameObject ore;

        // 平移机构
        public GameObject horizon;

        // 翻转机构
        public GameObject turnover;
        public GameObject leftClip;
        public GameObject rightClip;

        // 取矿接触区
        public OreGrip leftGrip;
        public OreGrip rightGrip;

        // 矿石取出
        public Transform popSpawn;
        public GameObject goldPrefab;
        public GameObject sliverPrefab;

        //简易取矿
        public bool iSPressed;
        [SyncVar] public bool isTaking;
        [SyncVar] public bool isExchanging;
        [SyncVar] public double pressedTime;
        public Camera oreCamera;
        public double lastExchangeTime;
        public double hardThreshold = 1.5f;
        public double takeOreRequiredTime = 5f;
        public double exchangeOreRequiredTime = 8.0f;
        public double exchangeColdTime = 3.0f;

        // 爪子
        public GameObject topClaw;
        public GameObject downClaw;
        public ClawGrip clawGrip;

        // 刷卡
        public GameObject card;
        public CardSensor cardSensor;

        // 相机
        public GameObject mainCamera;
        public GameObject clipCamera;
        public GameObject rescueCamera;

        [Header("Chassis")] public List<WheelCollider> wheels = new List<WheelCollider>();

        [Header("PTZ")] public Transform yaw;
        public Transform pitch;
        private readonly ToggleHelper _cameraHelper = new ToggleHelper();
        private readonly ToggleHelper _cardHelper = new ToggleHelper();
        private readonly ToggleHelper _clipHelper = new ToggleHelper();
        private readonly ToggleHelper _connectHelper = new ToggleHelper();
        private readonly ToggleHelper _exchangeHelper = new ToggleHelper();
        private readonly ToggleHelper _exchangeStartHelper = new ToggleHelper();
        private readonly ToggleHelper _exchangingHelper = new ToggleHelper();
        private readonly ToggleHelper _horizonHelper = new ToggleHelper();

        private readonly ToggleHelper _liftHelper = new ToggleHelper();
        private readonly Stack<Identity> _oreStorage = new Stack<Identity>();
        private readonly ToggleHelper _popHelper = new ToggleHelper();
        private readonly Queue<OreStoreBase> _storageOre = new Queue<OreStoreBase>();
        private readonly ToggleHelper _turnHelper = new ToggleHelper();
        private readonly ToggleHelper _turnoverHelper = new ToggleHelper();
        private int _cameraStage;
        private bool _exchanging = false;

        //八强兑矿
        private bool _exchangingStart = false;
        private OreStoreBase _grippingOre;
        private GroundChassis _groundChassis;
        private bool _ownOre;
        private Ptz _ptz;

        /// <summary>
        /// 组件初始化。
        /// </summary>
        protected override void Start()
        {
            base.Start();

            if (isServer)
            {
                _groundChassis = new GroundChassis(gameObject, wheels, yaw);
                _ptz = new Ptz(yaw, pitch);
            }
        }

        /// <summary>
        /// 更新云台。
        /// </summary>
        private void Update()
        {
            if (isClient)
            {
                if (id != EntityManager.Instance().LocalRobot())
                {
                    if (clipCamera.gameObject.activeSelf || rescueCamera.gameObject.activeSelf)
                    {
                        clipCamera.SetActive(false);
                        rescueCamera.SetActive(false);
                    }
                }
            }

            if (!isServer) return;

            // 执行小陀螺或云台跟随
            _ptz.Update();
            _ptz.YawRotate(_groundChassis.Update(Time.deltaTime, stickWithYaw: true));

            Power = _groundChassis.Rigidbody.velocity.magnitude * 4;
        }

        /// <summary>
        /// 刷卡复活与自动复活。
        /// </summary>

        // protected override void ReviveTick()
        // {
        //     base.ReviveTick();
        //     if (health > 0)
        //     {
        //         // 刷卡复活
        //         if (cardSensor.target != null)
        //         {
        //             Dispatcher.Instance().Send(new CardRevive
        //             {
        //                 Receiver = cardSensor.target.id
        //             });
        //         }
        //     }
        //     else
        //     {
        //         // 自动复活
        //         Dispatcher.Instance().Send(new CardRevive
        //         {
        //             Receiver = id
        //         });
        //     }
        // }

        /// <summary>
        /// 转发底盘碰撞事件。
        /// </summary>
        /// <param name="other"></param>
        private void OnCollisionEnter(Collision other)
        {
            if (!isServer) return;
            var armor = other.contacts[0].thisCollider.GetComponent<Armor>();
            if (armor != null)
            {
                armor.DelegateCollisionEnter(other);
            }
            else
            {
                _groundChassis.OnCollisionEnter(other);
            }
        }

        /// <summary>
        /// 防止车辆自转。
        /// </summary>
        /// <param name="other"></param>
        private void OnCollisionExit(Collision other)
        {
            if (isServer) _groundChassis.OnCollisionExit(other);
        }


        /// <summary>
        /// 声明接收事件。
        /// </summary>
        /// <returns></returns>
        public override List<string> InputActions()
        {
            //在父类中原方法的作用是返回一个空字符串，而在robot中需要覆盖这个方法来返回“兴趣字符串”
            //这个函数将会把robot感兴趣的字符串存到Dispatcher中去
            return base.InputActions().Concat(new List<string>
            {
                ActionID.Input.PrimaryAxis,
                ActionID.Input.ViewControl,
                ActionID.Input.StateControl,
                ActionID.Clock.EngineerBuff
            }).ToList();
        }

        /// <summary>
        /// 处理事件。
        /// </summary>
        /// <param name="action"></param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        [Server]
        public override void Receive(IAction action)
        {
            base.Receive(action);
            if (!isServer) return;
            //在父类中原方法未定义，在robot中覆盖
            //写出switch语句去判断action类型去执行相应的动作
            switch (action.ActionName())
            {
                case ActionID.Input.PrimaryAxis:
                    if (health == 0) break;
                    var exchangeSet = CustomizeManager.Instance().Data(
                        id,
                        CustomizeProperties.EngnieerSet.Exchanging);
                    if (exchangeSet > hardThreshold && _exchangingStart) break;
                    var primaryAxisAction = (PrimaryAxis)action;
                    if (primaryAxisAction.Receiver == id)
                    {
                        if (_cameraStage == 2)
                        {
                            primaryAxisAction.X *= -1;
                            primaryAxisAction.Y *= -1;
                        }

                        _groundChassis.Move(
                            new Vector2(primaryAxisAction.X, primaryAxisAction.Y) * 1.2f,
                            CustomizeManager.Instance().Data(
                                id, CustomizeProperties.Chassis.Velocity));
                    }

                    break;

                case ActionID.Input.ViewControl:
                    if (health == 0) break;
                    var viewControlAction = (ViewControl)action;
                    if (viewControlAction.Receiver == id)
                    {
                        if (_cameraStage == 0)
                        {
                            _ptz.View(
                                Time.deltaTime,
                                new Vector2(viewControlAction.X, viewControlAction.Y),
                                Vector2.one);
                        }
                        else
                        {
                            _ptz.View(
                                Time.deltaTime,
                                new Vector2(viewControlAction.X, 0),
                                Vector2.one);
                        }
                    }

                    break;

                case ActionID.Input.StateControl:
                    if (health == 0) break;
                    var stateControlAction = (StateControl)action;
                    var oreSet = CustomizeManager.Instance().Data(
                        id,
                        CustomizeProperties.EngnieerSet.Ore);
                    var exchangingSet = CustomizeManager.Instance().Data(
                        id,
                        CustomizeProperties.EngnieerSet.Exchanging);
                    if (stateControlAction.Receiver == id)
                    {
                        // 抬升
                        switch (_liftHelper.Toggle(stateControlAction.FunctionA))
                        {
                            case ToggleHelper.State.Re:
                                lift.transform.DOLocalMoveY(0.167f, 0.5f);
                                break;
                            case ToggleHelper.State.De:
                                lift.transform.DOLocalMoveY(0.05f, 0.5f);
                                break;
                            case ToggleHelper.State.Hold:
                                break;
                            default:
                                throw new ArgumentOutOfRangeException();
                        }

                        if (oreSet < hardThreshold && exchangingSet > hardThreshold)
                        {
                            //八强兑矿
                            switch (_exchangeStartHelper.Toggle(stateControlAction.FunctionG))
                            {
                                case ToggleHelper.State.Re:
                                    _exchangingStart = true;
                                    break;
                                case ToggleHelper.State.De:
                                    _exchangingStart = true;
                                    break;
                                case ToggleHelper.State.Hold:
                                    break;
                                default:
                                    throw new ArgumentOutOfRangeException();
                            }

                            switch (_exchangeHelper.Toggle(stateControlAction.FunctionF))
                            {
                                case ToggleHelper.State.Re:
                                    _exchangingStart = false;
                                    _exchanging = false;
                                    break;
                                case ToggleHelper.State.De:
                                    _exchangingStart = false;
                                    _exchanging = false;
                                    break;
                                case ToggleHelper.State.Hold:
                                    break;
                                default:
                                    throw new ArgumentOutOfRangeException();
                            }
                        }

                        //oreSet代表取矿机制的不同，当大于1.5时，为复杂取矿，小于1.5时为简易取矿，1.5数值是
                        //机构设置面板滑条移动表示的数值（适应操作手的不同需求）
                        if (oreSet > hardThreshold)
                        {
                            // 平移
                            switch (_horizonHelper.Toggle(stateControlAction.FunctionF))
                            {
                                case ToggleHelper.State.Re:
                                    horizon.transform.DOLocalMoveZ(0.51f, 0.5f);
                                    break;
                                case ToggleHelper.State.De:
                                    horizon.transform.DOLocalMoveZ(0.18f, 0.5f);
                                    StartCoroutine(TryCollectOre());
                                    break;
                                case ToggleHelper.State.Hold:
                                    break;
                                default:
                                    throw new ArgumentOutOfRangeException();
                            }

                            // 翻转
                            switch (_turnoverHelper.Toggle(stateControlAction.FunctionC))
                            {
                                case ToggleHelper.State.Re:
                                    StartCoroutine(
                                        RotateTweenHelper.RotateAroundLocal(
                                            turnover.transform,
                                            Vector3.right,
                                            180,
                                            0.5f)
                                    );
                                    break;
                                case ToggleHelper.State.De:
                                    StartCoroutine(
                                        RotateTweenHelper.RotateAroundLocal(
                                            turnover.transform,
                                            Vector3.right,
                                            -180,
                                            0.5f)
                                    );
                                    StartCoroutine(TryCollectOre());
                                    break;
                                case ToggleHelper.State.Hold:
                                    break;
                                default:
                                    throw new ArgumentOutOfRangeException();
                            }


                            // 取矿
                            switch (_clipHelper.Toggle(stateControlAction.FunctionD))
                            {
                                // 数据来自模型调试
                                case ToggleHelper.State.Re:
                                    leftClip.transform.DOLocalMoveX(-0.066f, 0.5f);
                                    rightClip.transform.DOLocalMoveX(0.073f, 0.5f);
                                    StartCoroutine(CheckOreGrip());
                                    break;
                                case ToggleHelper.State.De:
                                    leftClip.transform.DOLocalMoveX(-0.1291543f, 0.5f);
                                    rightClip.transform.DOLocalMoveX(0.1342912f, 0.5f);
                                    if (_grippingOre)
                                    {
                                        _grippingOre.gameObject.AddComponent<Rigidbody>();
                                        _grippingOre.transform.SetParent(null);
                                        _grippingOre = null;
                                    }

                                    break;
                                case ToggleHelper.State.Hold:
                                    break;
                                default:
                                    throw new ArgumentOutOfRangeException();
                            }

                            // 取出
                            if (_popHelper.Toggle(stateControlAction.FunctionJ) != ToggleHelper.State.Hold)
                            {
                                if (_horizonHelper.Current() && _turnoverHelper.Current() && _clipHelper.Current())
                                {
                                    if (_grippingOre == null)
                                    {
                                        if (_oreStorage.Count > 0)
                                        {
                                            var position = popSpawn.position;
                                            var rotation = popSpawn.rotation;
                                            var newOreIdentity = _oreStorage.Pop();
                                            var newOre = newOreIdentity.role switch
                                            {
                                                Identity.Roles.GoldOre => Instantiate(goldPrefab, position,
                                                    rotation),
                                                Identity.Roles.SilverOre => Instantiate(sliverPrefab, position,
                                                    rotation),
                                                _ => new GameObject()
                                            };
                                            newOre.GetComponent<OreStoreBase>().Identify(newOreIdentity);
                                            newOre.transform.SetParent(turnover.transform);
                                            _grippingOre = newOre.GetComponent<OreStoreBase>();
                                            NetworkServer.Spawn(newOre);

                                            //矿石出队并销毁
                                            NetworkServer.Destroy(_storageOre.Dequeue().gameObject);
                                        }
                                    }
                                }
                            }
                        }

                        //简易取矿机制
                        else if (oreSet < hardThreshold)
                        {
                            if (stateControlAction.Fire)
                            {
                                var oreCameraTransform = oreCamera.transform;
                                var ray = new Ray(oreCameraTransform.position, oreCameraTransform.forward);

                                //当检测到射线碰撞到物体时开始执行
                                if (Physics.Raycast(ray, out var rayHit, Mathf.Infinity))
                                {
                                    //开始计时
                                    if (!iSPressed)
                                    {
                                        pressedTime = NetworkTime.time;
                                        iSPressed = true;
                                    }

                                    //检测到矿石，表明正在进行取矿操作
                                    if (rayHit.transform.name == "GoldOre" || rayHit.transform.name == "SilverOre")
                                    {
                                        isTaking = true;
                                        //长按时间到达取矿所需时间
                                        if (NetworkTime.time - pressedTime > takeOreRequiredTime)
                                        {
                                            _grippingOre = rayHit.collider.GetComponent<OreStoreBase>();
                                            if (_grippingOre != null)
                                            {
                                                var position = ore.transform.position;
                                                var rotation = ore.transform.rotation;
                                                var newPressedOre = _grippingOre.id.role switch
                                                {
                                                    Identity.Roles.GoldOre => Instantiate(goldPrefab, position,
                                                        rotation),
                                                    Identity.Roles.SilverOre => Instantiate(sliverPrefab, position,
                                                        rotation),
                                                    _ => new GameObject()
                                                };

                                                //矿石入队
                                                _storageOre.Enqueue(newPressedOre.GetComponent<OreStoreBase>());
                                                _oreStorage.Push(_grippingOre.id);
                                                NetworkServer.Spawn(newPressedOre);
                                                Destroy(newPressedOre.GetComponent<Rigidbody>());
                                                newPressedOre.transform.SetParent(ore.transform);
                                                NetworkServer.Destroy(_grippingOre.gameObject);
                                                _grippingOre = null;
                                                pressedTime = NetworkTime.time;
                                                isTaking = false;
                                            }
                                        }
                                    }
                                    //检测到兑换传感器，表示正在进行兑换操作
                                    else if (rayHit.transform.name == "RedExchangeSensor" ||
                                             rayHit.transform.name == "BlueExchangeSensor")
                                    {
                                        if (exchangingSet > hardThreshold && _exchangingStart &&
                                            oreSet < hardThreshold)
                                        {
                                            switch (_exchangingHelper.Toggle(stateControlAction.FunctionC))
                                            {
                                                case ToggleHelper.State.Re:
                                                    _exchanging = true;
                                                    break;
                                                case ToggleHelper.State.De:
                                                    _exchanging = true;
                                                    break;
                                                case ToggleHelper.State.Hold:
                                                    break;
                                                default:
                                                    throw new ArgumentOutOfRangeException();
                                            }
                                        }

                                        if (exchangingSet < hardThreshold || _exchanging)
                                        {
                                            if (_storageOre.Count > 0)
                                            {
                                                isExchanging = true;
                                                // 兑换
                                                var holdTime = NetworkTime.time - pressedTime;
                                                var waitTime = NetworkTime.time - lastExchangeTime;
                                                if (holdTime > exchangeOreRequiredTime && waitTime > exchangeColdTime)
                                                {
                                                    var tran = rayHit.transform;
                                                    var position = tran.position;
                                                    var rotation = tran.rotation;
                                                    var newOreIdentity = _oreStorage.Pop();
                                                    var newOre = newOreIdentity.role switch
                                                    {
                                                        Identity.Roles.GoldOre => Instantiate(goldPrefab, position,
                                                            rotation),
                                                        Identity.Roles.SilverOre => Instantiate(sliverPrefab, position,
                                                            rotation),
                                                        _ => new GameObject()
                                                    };
                                                    NetworkServer.Spawn(newOre);
                                                    newOre.GetComponent<OreStoreBase>().Identify(newOreIdentity);
                                                    newOre.transform.SetParent(turnover.transform);
                                                    _grippingOre = newOre.GetComponent<OreStoreBase>();
                                                    //矿石出队并销毁
                                                    NetworkServer.Destroy(_storageOre.Dequeue().gameObject);
                                                    isExchanging = false;
                                                    _exchanging = false;
                                                    lastExchangeTime = NetworkTime.time;
                                                    pressedTime = NetworkTime.time;
                                                }
                                            }
                                            else
                                            {
                                                isExchanging = false;
                                            }
                                        }
                                    }
                                }
                            }
                            else
                            {
                                iSPressed = false;
                                isExchanging = false;
                                isTaking = false;
                            }
                        }

                        switch (_connectHelper.Toggle(stateControlAction.FunctionB))
                        {
                            case ToggleHelper.State.Re:
                                StartCoroutine(
                                    RotateTweenHelper.RotateAroundLocal(
                                        topClaw.transform,
                                        Vector3.left,
                                        15,
                                        0.2f)
                                );
                                StartCoroutine(
                                    RotateTweenHelper.RotateAroundLocal(
                                        downClaw.transform,
                                        Vector3.left,
                                        -15,
                                        0.2f)
                                );
                                clawGrip.Catch();


                                break;

                            case ToggleHelper.State.De:
                                StartCoroutine(
                                    RotateTweenHelper.RotateAroundLocal(
                                        topClaw.transform,
                                        Vector3.left,
                                        -15,
                                        0.5f)
                                );
                                StartCoroutine(
                                    RotateTweenHelper.RotateAroundLocal(
                                        downClaw.transform,
                                        Vector3.left,
                                        15,
                                        0.5f)
                                );
                                clawGrip.Release();


                                break;
                            case ToggleHelper.State.Hold:
                                break;
                            default:
                                throw new ArgumentOutOfRangeException();
                        }

                        switch (_cardHelper.Toggle(stateControlAction.FunctionH))
                        {
                            case ToggleHelper.State.Re:
                                card.transform.DOLocalMoveZ(-0.37f, 0.5f);
                                break;
                            case ToggleHelper.State.De:
                                card.transform.DOLocalMoveZ(-0.13f, 0.5f);
                                break;
                        }

                        // 转向
                        switch (_turnHelper.Toggle(stateControlAction.FunctionL))
                        {
                            case ToggleHelper.State.Re:
                                _ptz.View(
                                    0.1f * Time.deltaTime,
                                    new Vector2(90000, 0),
                                    Vector2.one);
                                break;
                            case ToggleHelper.State.De:
                                _ptz.View(
                                    0.1f * Time.deltaTime,
                                    new Vector2(90000, 0),
                                    Vector2.one);
                                break;
                            case ToggleHelper.State.Hold:
                                break;
                            default:
                                throw new ArgumentOutOfRangeException();
                        }

                        //取矿相机翻转
                        if (_cameraHelper.Toggle(stateControlAction.FunctionE) != ToggleHelper.State.Hold)
                        {
                            if (!RotateTweenHelper.Rotating(mainCamera.transform))
                            {
                                StartCoroutine(
                                    RotateTweenHelper.RotateAroundLocal(
                                        mainCamera.transform,
                                        Vector3.right,
                                        new List<float> { -95, -85, 180 }[_cameraStage],
                                        0.2f
                                    )
                                );
                                _cameraStage++;
                                _cameraStage %= 3;
                            }
                        }
                    }

                    break;

                case ActionID.Clock.EngineerBuff:
                    var buffAction = (EngineerBuff)action;
                    if (buffAction.canGet)
                        AddEffect(new Gameplay.Effects.EngineerBuff());
                    else
                        RemoveEffect(EffectID.Buffs.EngineerBuff);
                    break;
            }
        }

        /// <summary>
        /// 检查是否夹住了矿石。
        /// 夹住的话进行固连。
        /// </summary>
        /// <returns></returns>
        private IEnumerator CheckOreGrip()
        {
            yield return new WaitForSeconds(0.5f);
            if (leftGrip.ore != null && leftGrip.ore == rightGrip.ore)
            {
                var rigid = leftGrip.ore.GetComponent<Rigidbody>();
                if (rigid != null)
                {
                    Destroy(rigid);
                }

                _grippingOre = leftGrip.ore;
                _grippingOre.transform.SetParent(turnover.transform);
            }
        }

        /// <summary>
        /// 将夹住的矿石放进车里。
        /// </summary>
        /// <returns></returns>
        private IEnumerator TryCollectOre()
        {
            yield return new WaitForSeconds(0.5f);
            if (!_turnoverHelper.Current() && !_horizonHelper.Current())
            {
                if (_grippingOre != null)
                {
                    // TODO: 所有的网络同步

                    var position = ore.transform.position;
                    var rotation = ore.transform.rotation;
                    var newOre = _grippingOre.id.role switch
                    {
                        Identity.Roles.GoldOre => Instantiate(goldPrefab, position,
                            rotation),
                        Identity.Roles.SilverOre => Instantiate(sliverPrefab, position,
                            rotation),
                        _ => new GameObject()
                    };
                    newOre.GetComponent<OreStoreBase>().Identify(_grippingOre.id);
                    //矿石入队
                    _storageOre.Enqueue(newOre.GetComponent<OreStoreBase>());
                    NetworkServer.Spawn(newOre);
                    Destroy(newOre.GetComponent<Rigidbody>());
                    newOre.transform.SetParent(ore.transform);

                    _oreStorage.Push(_grippingOre.id);
                    NetworkServer.Destroy(_grippingOre.gameObject);
                    _grippingOre = null;
                }
            }
        }
    }
}