using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Controllers;
using Controllers.Child;
using Gameplay.Attribute;
using Gameplay.Effects;
using Gameplay.Events;
using Gameplay.Events.Child;
using Honeti;
using Infrastructure;
using Infrastructure.Child;
using Mirror;
using Misc;
using UnityEngine;
using Ejected = Gameplay.Events.Ejected;

namespace Gameplay
{
    /// <summary>
    /// <c>RobotStoreBase</c> 是所有机器人控制器的基类。
    /// <br/>保存血量、剩余弹药量等数据。
    /// </summary>
    public class RobotStoreBase : StoreBase
    {
        // 血量,弹量
        [SyncVar] public float health;
        [SyncVar] public int magLarge;
        [SyncVar] public int launchedLarge;
        [SyncVar] public int totalMagLarge;
        [SyncVar] public int magSmall;
        [SyncVar] public int launchedSmall;
        [SyncVar] public int totalMagSmall;
        [SyncVar] public bool isDead = false;

        // 等级、经验
        [SyncVar] public int level;
        [SyncVar] public float experience;

        // 复活读条,额外读条
        [SyncVar] public int revivalProcess;
        [SyncVar] public int requiredAdded = 0;
        [SyncVar] public int revivalProcessRequired;

        // 机构类型
        [SyncVar] public MechanicType.Chassis chassisType = MechanicType.Chassis.Default;

        // 主摄像机
        public Camera fpvCamera;

        // 发射机构锁定
        [SyncVar] public bool gunLocked;

        //超级电容
        [SyncVar] public bool superBattery;
        [SyncVar] public bool isMoving;
        [SyncVar] public double startTime;
        [SyncVar] public double switchEndTime;
        [SyncVar] public double movingEndTime;
        [SyncVar] public double remainingCapacity;
        public double batteryCapacity = 100;
        public float batteryConsumeSpeed = 2f;
        public float batteryRecoverSpeed = 2.5f;
        public int partyCountDown;

        // 小陀螺
        [SyncVar] public bool spinning;

        //倒计时三分钟回血增益
        public bool partyTime = false; //回血点增益增强

        //进攻时间
        public double lastHitTime;

        // 防飞天
        private readonly Queue<Vector3> _lastPositions = new Queue<Vector3>();

        // 过热
        private readonly ToggleHelper _overheatHelper = new ToggleHelper();
        public readonly SyncList<MechanicType.GunInfo> Guns = new SyncList<MechanicType.GunInfo>();

        // 防翻车
        private int _antiCarCrash;

        //防飞天
        private int _jumpCount = 0;
        private float _jumpTime1 = 0;
        private Vector3 _lastAngle;

        //装甲板状态
        private bool _lastArmor = false;
        private float _naturalGrowCountdown = 12;

        // 复活读条
        private float _nextReviveCountdown;

        // 延迟启动防飞天
        private bool _reviveInitiated;

        private Rigidbody _rigidbody;

        // 延迟启动防飞天（UC）
        private bool _stabilizeInitiated;

        //工程回血的判断
        private bool _startRecover = true;

        // 发射机构数值
        [SyncVar] protected float Heat = 0;
        [SyncVar] protected float HeatLimit = 1e5f;
        [SyncVar] protected float Power;

        /// <summary>
        /// 每帧更新状态。
        /// </summary>
        protected override void FixedUpdate()
        {
            base.FixedUpdate();
            if (!isServer) return;

            if (!_rigidbody)
            {
                _rigidbody = GetComponent<Rigidbody>();
                if (!_rigidbody)
                {
                    _rigidbody = GetComponentInParent<Rigidbody>();
                }
            }

            if (!_reviveInitiated)
            {
                if (HasEffect(EffectID.Buffs.Revive))
                {
                    _reviveInitiated = true;
                }
            }

            if (!_stabilizeInitiated)
            {
                if (HasEffect(EffectID.Status.Stabilize))
                {
                    _stabilizeInitiated = true;
                }
            }

            _jumpTime1 += Time.fixedDeltaTime;
            if (_jumpTime1 > 4)
            {
                //Debug.Log("清除位置突变次数 "+_jumpCount);
                _jumpCount = 0;
                _jumpTime1 = 0;
            }

            if (EntityManager.Instance() != null && EntityManager.Instance().CurrentMap() == MapType.RMUL2022)
            {
                // 二代防飞天
                if (_reviveInitiated && id.IsGroundRobot())
                {
                    var currentPosition = transform.position;
                    var lastPosition = _lastPositions.Count > 0 ? _lastPositions.Peek() : currentPosition;
                    var jumped = (currentPosition - lastPosition).sqrMagnitude > 2.5f;
                    if (HasEffect(EffectID.Buffs.Revive) && !jumped)
                    {
                        _lastPositions.Enqueue(currentPosition);
                        if (_lastPositions.Count > 20) _lastPositions.Dequeue();
                    }
                    else
                    {
                        var returnTo = _lastPositions.Count < 10
                            ? SpawnManager.Instance().LoadLocation(id).position
                            : _lastPositions.ToArray()[jumped ? 5 : 10];
                        transform.position = returnTo;
                        _rigidbody.velocity = Vector3.zero;
                        Dispatcher.Instance().Send(new AddEffect
                        {
                            Receiver = id,
                            Effect = new ReviveBuff()
                        });
                        OnRectify();
                    }
                }
            }

            if (EntityManager.Instance() != null && EntityManager.Instance().CurrentMap() == MapType.RMUC2022)
            {
                // 二代防飞天 UC
                if (_stabilizeInitiated && id.IsGroundRobot())
                {
                    var currentPosition = transform.position;
                    var lastPosition = _lastPositions.Count > 0 ? _lastPositions.Peek() : currentPosition;
                    var jumped = (currentPosition - lastPosition).sqrMagnitude > 3f;
                    //Debug.Log(this.name+"is jumped? "+jumped+"  "+(currentPosition - lastPosition).sqrMagnitude);
                    //Debug.Log("突变次数 "+_jumpCount);
                    if (jumped) _jumpCount++;
                    if (_jumpCount == 0 && !jumped)
                    {
                        _lastPositions.Enqueue(currentPosition);
                        if (_lastPositions.Count > 20) _lastPositions.Dequeue();
                    }
                    else if (_jumpCount < 20)
                    {
                        var returnTo = _lastPositions.Count < 10
                            ? SpawnManager.Instance().LoadLocation(id).position
                            : _lastPositions.ToArray()[jumped ? 5 : 10];
                        transform.position = returnTo;
                        transform.localEulerAngles = new Vector3(0, transform.localEulerAngles.y, 0);
                        _rigidbody.velocity = Vector3.zero;
                        /*Dispatcher.Instance().Send(new AddEffect
                        {
                            Receiver = id,
                            Effect = new Stabilize()
                        });*/
                        OnRectify();
                    }
                }
            }

            if (HasEffect(EffectID.Status.Ejected))
            {
                health = 0;
            }

            // 经验自然增长
            if (ClockStore.Instance().playing && health > 0)
            {
                _naturalGrowCountdown -= Time.fixedDeltaTime;
                if (_naturalGrowCountdown <= 0)
                {
                    _naturalGrowCountdown = 12;
                    experience += id.role switch
                    {
                        Identity.Roles.Infantry => 0.2f,
                        Identity.Roles.BalanceInfantry => 0.2f,
                        Identity.Roles.Hero => 0.4f,
                        _ => 0
                    };
                }
            }

            // 升级
            var expToUpgrade = AttributeManager.Instance().RobotAttributes(this).ExperienceToUpgrade;
            if (experience >= expToUpgrade)
            {
                experience -= expToUpgrade;
                var healthLimit = AttributeManager.Instance().RobotAttributes(this).MaxHealth;
                level++;
                var newHealthLimit = AttributeManager.Instance().RobotAttributes(this).MaxHealth;
                health += newHealthLimit - healthLimit;
                if (health > newHealthLimit) health = newHealthLimit;
            }

            // 装甲板灯光
            if (health > 0 && _lastArmor == false)
            {
                for (var i = 0; i < 4; i++)
                {
                    Dispatcher.Instance().SendChild(new TurnArmor
                    {
                        ReceiverChild = new ChildIdentity(ChildType.Armor, i),
                        IsOn = true
                    }, id);
                }

                _lastArmor = true;
            }
            else if (health <= 0 && _lastArmor == true)
            {
                for (var i = 0; i < 4; i++)
                {
                    Dispatcher.Instance().SendChild(new TurnArmor
                    {
                        ReceiverChild = new ChildIdentity(ChildType.Armor, i),
                        IsOn = false
                    }, id);
                }

                _lastArmor = false;
            }

            _nextReviveCountdown -= Time.fixedDeltaTime;
            if (_nextReviveCountdown <= 0)
            {
                _nextReviveCountdown = 1;
                ReviveTick();
            }

            // 复活读条满
            if (revivalProcessRequired != 0 && revivalProcess >= revivalProcessRequired + requiredAdded)
            {
                //判断读条复活or立即复活

                if (revivalProcessRequired != 1 && revivalProcessRequired > 0)
                    health = AttributeManager.Instance().RobotAttributes(this).MaxHealth * 0.1f;
                else
                {
                    health = AttributeManager.Instance().RobotAttributes(this).MaxHealth;
                    requiredAdded += 20;
                }


                //重置复活进度
                revivalProcess = 0;

                Dispatcher.Instance().Send(new AddEffect
                {
                    Receiver = id,
                    Effect = new RevivalBuff()
                });

                isDead = false;
            }

            // 灯条
            Dispatcher.Instance().SendChild(new SetLightBarState
            {
                Health = health / AttributeManager.Instance().RobotAttributes(this).MaxHealth,
                Revive = (float)revivalProcess / revivalProcessRequired + requiredAdded,
                Buff = HasEffect(EffectID.Buffs.Revival)
                       || HasEffect(EffectID.Buffs.SmallPowerRune)
                       || HasEffect(EffectID.Buffs.LargePowerRune)
            }, id);

            // 解锁发射机构
            if (EntityManager.Instance().CurrentMap() == MapType.RMUL2022)
            {
                if (HasEffect(EffectID.Status.AtSupply))
                {
                    gunLocked = false;
                }
            }

            //工程自动恢复血量（23赛季取消）
            /*if (id.role == Identity.Roles.Engineer && health != 0)
            {
                if (health < AttributeManager.Instance().RobotAttributes(this).MaxHealth)
                {
                    var currentTime = NetworkTime.time;
                    var lastAttackTime = EntityManager.Instance().lastAttackTime[id];
                    if (currentTime-lastAttackTime>20)
                    {
                        if (_startRecover)
                        {
                            StartCoroutine(AutoRecoverRoutine(0.05f));
                        }
                    }
                }
                else if (health > AttributeManager.Instance().RobotAttributes(this).MaxHealth)
                    health = AttributeManager.Instance().RobotAttributes(this).MaxHealth;
            }*/

            //自动哨兵自动恢复血量
            if (id.role == Identity.Roles.AutoSentinel && health != 0)
            {
                if (health < AttributeManager.Instance().RobotAttributes(this).MaxHealth)
                {
                    var currentTime = NetworkTime.time;
                    var lastAttackTime = EntityManager.Instance().GetLastAttackTime(id);
                    if (currentTime - lastAttackTime > 20)
                    {
                        if (_startRecover)
                        {
                            StartCoroutine(AutoRecoverRoutine(0.005f));
                        }
                    }
                }
                else if (health > AttributeManager.Instance().RobotAttributes(this).MaxHealth)
                {
                    health = AttributeManager.Instance().RobotAttributes(this).MaxHealth;
                }
            }


            // 过热扣血
            switch (_overheatHelper.Toggle(GetHeat(0) > GetHeatLimit(0)))
            {
                case ToggleHelper.State.Re:
                    Dispatcher.Instance().Send(new AddEffect
                    {
                        Receiver = id,
                        Effect = new OverHeat()
                    });
                    break;

                case ToggleHelper.State.De:
                    Dispatcher.Instance().Send(new RemoveEffect
                    {
                        Receiver = id,
                        Effect = new OverHeat()
                    });
                    break;
            }

            if (GetHeat(0) > GetHeatLimit(0) && health > 0)
            {
                if (GetHeat(0) < GetHeatLimit(0) * 2)
                {
                    health -= (GetHeat(0) - GetHeatLimit(0)) / 2500 *
                              AttributeManager.Instance().RobotAttributes(this).MaxHealth;
                }
                else
                {
                    health -= (GetHeat(0) - GetHeatLimit(0) * 2) / 250 *
                              AttributeManager.Instance().RobotAttributes(this).MaxHealth;
                    SetHeat(0, GetHeatLimit(0) * 2);
                }

                if (health < 0)
                {
                    if (EntityManager.Instance().CurrentMap() == MapType.RMUL2022)
                    {
                        gunLocked = true;
                        var baseStore = (BaseStore)EntityManager.Instance().Ref(
                            new Identity(id.camp, Identity.Roles.Base));
                        baseStore.invincible = false;
                    }

                    Dispatcher.Instance().Send(new Kill
                    {
                        killer = id,
                        victim = id,
                        method = I18N.instance.getValue("^excess_heat_death")
                    });
                    health = 0;
                }
            }

            if (health == 0)
            {
                _rigidbody.velocity = Vector3.zero;

                if (!isDead)
                {
                    isDead = true;
                    gunLocked = true;
                    revivalProcessRequired = 10 + (420 + 5 - ClockStore.Instance().countDown) / 10; //四舍五入
                }
            }

            // 更新热量信息
            Dispatcher.Instance().SendChild(new HeatStatus
            {
                percentage = (float)Math.Round(GetHeat(0) / GetHeatLimit(0), 1)
            }, id);

            // 防翻车
            if (Mathf.Abs(this.transform.rotation.eulerAngles.x - 180) <= 140
                || Mathf.Abs(this.transform.rotation.eulerAngles.z - 180) <= 140)
            {
                if (_antiCarCrash == 0)
                {
                    transform.rotation = Quaternion.Euler(_lastAngle);
                    _rigidbody.angularVelocity = Vector3.zero;
                }

                _antiCarCrash++;
                if (_antiCarCrash == 30)
                {
                    _antiCarCrash = 0;
                }
            }
            else
            {
                _lastAngle = transform.rotation.eulerAngles;
            }
        }

        /// <summary>
        /// 身份全部使用统一确认。
        /// </summary>
        protected override void Identify()
        {
            // ID = new Identity(Identity.Camps.Red, Identity.Roles.Infantry);
        }

        /// <summary>
        /// 声明兴趣事件。
        /// </summary>
        /// <returns></returns>
        public override List<string> InputActions()
        {
            return base.InputActions().Concat(new List<string>
            {
                ActionID.Magazine.AddBullet,
                ActionID.Engineer.Revive,
                ActionID.Stage.Ejected,
                ActionID.Recorder.MechanicSelect,
                ActionID.Exchange.ExchangeHealth,
                ActionID.Clock.PartyTime
            }).ToList();
        }

        /// <summary>
        /// 处理事件。
        /// </summary>
        /// <param name="action">事件</param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public override void Receive(IAction action)
        {
            base.Receive(action);
            if (!isServer) return;
            switch (action.ActionName())
            {
                case ActionID.Magazine.AddBullet:
                    var bulletAddAction = (AddBullet)action;
                    if (bulletAddAction.Receiver == id)
                    {
                        switch (bulletAddAction.Type)
                        {
                            // TODO: 容量上限
                            case MechanicType.CaliberType.Small:
                                magSmall += bulletAddAction.Amount;
                                break;
                            case MechanicType.CaliberType.Large:
                                magLarge += bulletAddAction.Amount;
                                break;
                            default:
                                throw new ArgumentOutOfRangeException();
                        }
                    }

                    break;

                case ActionID.Engineer.Revive:
                    var cardStateAction = (CardRevive)action;
                    if (cardStateAction.Receiver == id)
                    {
                        AddEffect(new CardReviveBuff()
                        {
                            timeout = 1.0f,
                        });
                        if (health == 0) revivalProcess += 1;
                    }

                    break;

                case ActionID.Stage.Ejected:
                    var ejectedAction = (Ejected)action;
                    if (ejectedAction.target == id)
                    {
                        Dispatcher.Instance().Send(new AddEffect
                        {
                            Receiver = id,
                            Effect = new Effects.Ejected()
                        });
                    }

                    break;

                case ActionID.Recorder.MechanicSelect:
                    var mechanicSelectAction = (MechanicSelect)action;
                    if (mechanicSelectAction.Receiver == id)
                    {
                        var maxHealth = AttributeManager.Instance().RobotAttributes(this).MaxHealth;
                        chassisType = (MechanicType.Chassis)Enum.Parse(typeof(MechanicType.Chassis),
                            mechanicSelectAction.Chassis);
                        Guns[0].type =
                            (MechanicType.GunType)Enum.Parse(typeof(MechanicType.GunType), mechanicSelectAction.Gun);
                        var newMaxHealth = AttributeManager.Instance().RobotAttributes(this).MaxHealth;
                        health = health / maxHealth * newMaxHealth;
                    }

                    break;

                case ActionID.Exchange.ExchangeHealth:
                    var ExHealth = (ExchangeHealth)action;
                    if (ExHealth.Id == id)
                    {
                        revivalProcessRequired = 1 - requiredAdded;
                        gunLocked = false;
                    }

                    break;

                case ActionID.Clock.PartyTime:
                    partyTime = true;
                    break;
            }
        }

        /// <summary>
        /// 设置装甲板和灯条外观。
        /// </summary>
        /// <param name="text">装甲板字符</param>
        public void SetVisual(char text = ' ')
        {
            for (var i = 0; i < GetComponentsInChildren<Armor>().Length; i++)
            {
                Dispatcher.Instance().SendChild(new SyncArmor
                {
                    Camp = id.camp,
                    ReceiverChild = new ChildIdentity(ChildType.Armor, i),
                    Text = text
                }, id);
            }

            Dispatcher.Instance().SendChild(new SyncLightBar
            {
                Camp = id.camp
            }, id);
        }

        [ClientRpc]
        private void OnRectify()
        {
            var playerStore = FindObjectsOfType<PlayerStore>().FirstOrDefault(p => p.localRobot == this);
            if (playerStore != null && playerStore.isLocalPlayer)
            {
                Debug.LogWarning("位置修正！如出现地震请联系裁判、赛务或开发者并知会此信息！");
                Debug.LogWarning("造成不便，非常抱歉 Orz");
            }
        }

        /// <summary>
        /// 每秒一 tick 复活周期。
        /// </summary>
        protected virtual void ReviveTick()
        {
            if (id.role == Identity.Roles.AutoSentinel)
                return;
            // // 复活区读条
            if (health == 0 && !HasEffect(EffectID.Status.Ejected))
            {
                //补血点读条加速和发射机构上电
                if (HasEffect(EffectID.Buffs.Revive))
                {
                    for (var i = 0; i < 4; i++)
                    {
                        Dispatcher.Instance().Send(new CardRevive
                        {
                            Receiver = id
                        });
                    }

                    if (gunLocked == true)
                        gunLocked = false;
                }
                //复活读条
                else
                {
                    Dispatcher.Instance().Send(new CardRevive
                    {
                        Receiver = id
                    });
                }
            }
            else
            {
                //回血行为,发射机构上电,补血点增益(partyTime)判断
                if (EntityManager.Instance().CurrentMap() == MapType.RMUC2022)
                {
                    if (partyCountDown > 0)
                        --partyCountDown;
                    if (HasEffect(EffectID.Buffs.Revive))
                    {
                        var maxHealth = AttributeManager.Instance().RobotAttributes(this).MaxHealth;
                        var curTime = NetworkTime.time;

                        if ((partyTime) && (curTime >= (lastHitTime + 6)) &&
                            (EntityManager.Instance().lastAttackTime.ContainsKey(id)
                                ? (curTime > EntityManager.Instance().lastAttackTime[id] + 6)
                                : true))
                        {
                            health += 0.25f * maxHealth;
                            partyCountDown = 4;
                        }
                        else
                        {
                            health += 0.1f * maxHealth;
                        }

                        if (health > maxHealth) health = maxHealth;
                        if (gunLocked == true)
                            gunLocked = false;
                    }
                }

                if (EntityManager.Instance().CurrentMap() == MapType.RMUL2022)
                {
                    if (HasEffect(EffectID.Buffs.Medical))
                    {
                        var maxHealth = AttributeManager.Instance().RobotAttributes(this).MaxHealth;
                        health += maxHealth * 0.1f;
                        if (health > maxHealth) health = maxHealth;
                    }
                }
            }
        }

        /// <summary>
        /// 获取特定发射机构热量。
        /// </summary>
        /// <param name="gunIndex">机构序号</param>
        /// <returns></returns>
        // ReSharper disable once InconsistentNaming
        public virtual float GetHeat(int gunIndex)
        {
            return Heat;
        }

        /// <summary>
        /// 获取特定发射机构热量上限。
        /// </summary>
        /// <param name="gunIndex">机构序号</param>
        /// <returns></returns>
        // ReSharper disable once InconsistentNaming
        public virtual float GetHeatLimit(int gunIndex)
        {
            return HeatLimit;
        }

        /// <summary>
        /// 用于设置特定发射机构热量值。
        /// </summary>
        /// <param name="gunIndex">机构序号</param>
        /// <param name="heat">热量值</param>
        protected virtual void SetHeat(int gunIndex, float heat)
        {
        }

        /// <summary>
        /// 用于工程每秒自动恢复血量的协程
        /// </summary>
        /// <returns></returns>
        private IEnumerator AutoRecoverRoutine(float persent)
        {
            _startRecover = false;
            yield return new WaitForSeconds(1);
            health += AttributeManager.Instance().RobotAttributes(this).MaxHealth * persent;
            _startRecover = true;
        }

        public virtual float GetPower() => Power;
    }
}