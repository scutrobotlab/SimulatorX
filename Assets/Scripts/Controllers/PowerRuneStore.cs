using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Controllers.Child;
using Gameplay.Events;
using Gameplay.Events.Child;
using Gameplay;
using Infrastructure;
using Infrastructure.Child;
using Mirror;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using Random = UnityEngine.Random;

namespace Controllers
{
    /// <summary>
    /// 能量机关状态。
    /// </summary>
    public enum PowerRuneState
    {
        /// <summary>
        /// 不可激活。
        /// </summary>
        Unavailable,

        /// <summary>
        /// 可激活。
        /// </summary>
        Available,

        /// <summary>
        /// 正在激活。
        /// </summary>
        Activating,

        /// <summary>
        /// 已激活。
        /// </summary>
        Activated,
    }

    /// <summary>
    /// 能量机关分支状态。
    /// </summary>
    public enum PowerRuneBranchState
    {
        /// <summary>
        /// 不可击打。
        /// </summary>
        Idle,

        /// <summary>
        /// 可击打。
        /// </summary>
        CanHit,

        /// <summary>
        /// 已击打点亮。
        /// </summary>
        Lit
    }

    /// <summary>
    /// 能量机关控制器。
    /// </summary>
    public class PowerRuneStore : StoreBase
    {
        private float _a;
        private float _b;
        private float _w;
        private float _speed;
        private bool _isLarge;
        private PowerRuneState _runeState;
        private PowerRuneState _lastRuneState;
        private float _lastHitTime;
        private float _activateTime;
        private float _nextAvailableTime;
        private bool _canStartActivation;
        private bool _isActivated = false;
        
        private readonly List<PowerRuneBranchState> _branchStates = new List<PowerRuneBranchState>();
        private readonly List<PowerRuneBranchState> _targetStates = new List<PowerRuneBranchState>();

        private int _score;
        
        /// <summary>
        /// 编辑器中设置阵营
        /// </summary>
        protected override void Identify()
        {
        }

        /// <summary>
        /// 初始化分支状态。
        /// </summary>
        protected override void Start()
        {
            base.Start();
            _runeState = PowerRuneState.Unavailable;
            for (var i = 0; i < 5; i++) _branchStates.Add(PowerRuneBranchState.Idle);
            for (var i = 0; i < 50; i++) _targetStates.Add(PowerRuneBranchState.Idle);
            UpdateBranchStates();
            _lastRuneState = PowerRuneState.Unavailable;
        }

        /// <summary>
        /// 声明兴趣事件。
        /// </summary>
        /// <returns></returns>
        public override List<string> InputActions()
        {
            return base.InputActions().Concat(new List<string>
            {
                ActionID.Clock.PowerRuneAvailable,
                ActionID.Stage.PowerRuneActivating,
                ActionID.Armor.ArmorHit,
                ActionID.Stage.PowerRuneActivated,
            }).ToList();
        }

        /// <summary>
        /// 能量机关旋转事件接收
        /// </summary>
        /// <param name="action"></param>
        public override void Receive(IAction action)
        {
            base.Receive(action);
            if (!isServer) return;
            switch (action.ActionName())
            {
                case ActionID.Clock.PowerRuneAvailable:
                    var canActivateAction = (PowerRuneAvailable)action;
                    if (canActivateAction.Available)
                    {
                        _isLarge = canActivateAction.IsLarge;
                        _runeState = PowerRuneState.Available;
                        // 旋转参数初始化
                        if (_isLarge)
                        {
                            _a = canActivateAction.A;
                            _b = canActivateAction.B;
                            _w = canActivateAction.W;
                        }
                        else
                        {
                            _speed = (float)1.047;
                        }

                        _nextAvailableTime = (float)NetworkTime.time;
                    }
                    else if (!canActivateAction.Available)
                    {
                        if (_isActivated)
                        {
                            var activateTime = (float)NetworkTime.time - _activateTime;
                            if (activateTime > 45)
                            {
                                _runeState = PowerRuneState.Unavailable;
                                for (var i = 0; i < 5; i++) _branchStates[i] = PowerRuneBranchState.Idle;
                                for (var i = 0; i < 50; i++) _targetStates[i] = PowerRuneBranchState.Idle;
                                UpdateBranchStates();
                            }
                            else if (activateTime < 45)
                            {
                                StartCoroutine(WaitToClose(45 - activateTime));
                            }
                        }
                        else
                        {
                            _runeState = PowerRuneState.Unavailable;
                            for (var i = 0; i < 5; i++) _branchStates[i] = PowerRuneBranchState.Idle;
                            for (var i = 0; i < 50; i++) _targetStates[i] = PowerRuneBranchState.Idle;
                            UpdateBranchStates();
                        }
                    }

                    break;

                case ActionID.Stage.PowerRuneActivating:
                    var activatingAction = (PowerRuneActivating)action;
                    if (activatingAction.Camp == id.camp)
                    {
                        _canStartActivation = activatingAction.Activating;
                    }

                    break;

                /*case ActionID.Armor.ArmorHit:
                    var hitAction = (ArmorHit) action;
                    if (hitAction.Receiver == id)
                    {
                        if (_runeState == PowerRuneState.Activating)
                        {
                            if (_branchStates[hitAction.Armor.serial] == PowerRuneBranchState.CanHit)
                            {
                                _branchStates[hitAction.Armor.serial] = PowerRuneBranchState.Lit;
                                // 判断激活状态
                                if (_branchStates.Count(bs => bs == PowerRuneBranchState.Lit) == 5)
                                {
                                    // 激活成功
                                    _runeState = PowerRuneState.Activated;
                                    _isActivated = true;
                                    _activateTime = (float) NetworkTime.time;
                                    StartCoroutine(ActivatedBlink());
                                    // 同步
                                    Dispatcher.Instance().Send(new PowerRuneActivated
                                    {
                                        Camp = id.camp,
                                        IsLarge = _isLarge,
                                        ActivatedTime = NetworkTime.time
                                    });
                                }
                                else
                                {
                                    // 点亮下一分支
                                    var nextBranch = Random.Range(0, 5);
                                    while (_branchStates[nextBranch] != PowerRuneBranchState.Idle)
                                    {
                                        nextBranch = Random.Range(0, 5);
                                    }

                                    _branchStates[nextBranch] = PowerRuneBranchState.CanHit;
                                    _lastHitTime = (float) NetworkTime.time;
                                }

                                UpdateBranchStates();
                            }
                            else
                            {
                                // 击打错误，重置
                                for (var i = 0; i < 5; i++) _branchStates[i] = PowerRuneBranchState.Idle;
                                _lastHitTime = (float) NetworkTime.time;
                                _branchStates[Random.Range(0, 5)] = PowerRuneBranchState.CanHit;
                                UpdateBranchStates();
                            }
                        }
                    }

                    break;*/

                //新增
                case ActionID.Armor.ArmorHit:
                    var hitAction = (ArmorHit)action;
                    if (hitAction.Receiver == id)
                    {
                        if (_runeState == PowerRuneState.Activating)
                        {
                            if (_branchStates[hitAction.Armor.serial] == PowerRuneBranchState.CanHit)
                            {
                                _branchStates[hitAction.Armor.serial] = PowerRuneBranchState.Lit;
                                _targetStates[hitAction.Armor.serial * 10 + 1] = PowerRuneBranchState.Idle;
                                _targetStates[hitAction.Armor.serial * 10 + 3] = PowerRuneBranchState.Idle;
                                _targetStates[hitAction.Armor.serial * 10 + 8] = PowerRuneBranchState.Idle;
                                var distance = (hitAction.CenterPos - hitAction.Position).magnitude;
                                var s = 10 - (int)(distance * 1000 / 15);
                                _targetStates[hitAction.Armor.serial * 10 + s - 1] =
                                    PowerRuneBranchState.Lit;
                                if (s == 10)
                                {
                                    _targetStates[hitAction.Armor.serial * 10] = PowerRuneBranchState.Lit;
                                    _targetStates[hitAction.Armor.serial * 10 + 2] = PowerRuneBranchState.Lit;
                                    _targetStates[hitAction.Armor.serial * 10 + 4] = PowerRuneBranchState.Lit;
                                    _targetStates[hitAction.Armor.serial * 10 + 6] = PowerRuneBranchState.Lit;
                                    _targetStates[hitAction.Armor.serial * 10 + 8] = PowerRuneBranchState.Lit;

                                }
                                // 判断激活状态
                                if (_branchStates.Count(bs => bs == PowerRuneBranchState.Lit) == 5)
                                {
                                    // 激活成功
                                    _runeState = PowerRuneState.Activated;
                                    _isActivated = true;
                                    _score += s;
                                    _activateTime = (float)NetworkTime.time;
                                    StartCoroutine(ActivatedBlink());
                                    // 同步
                                    Dispatcher.Instance().Send(new PowerRuneActivated
                                    {
                                        Camp = id.camp,
                                        IsLarge = _isLarge,
                                        ActivatedTime = NetworkTime.time,
                                        Score = _score
                                    });
                                    Debug.Log(_score);
                                }
                                else
                                {
                                    //计算分数
                                    _score += s;

                                    // 点亮下一分支
                                    var nextBranch = Random.Range(0, 5);
                                    while (_branchStates[nextBranch] != PowerRuneBranchState.Idle)
                                    {
                                        nextBranch = Random.Range(0, 5);
                                    }

                                    _branchStates[nextBranch] = PowerRuneBranchState.CanHit;
                                    _lastHitTime = (float)NetworkTime.time;
                                }

                                UpdateBranchStates();
                            }
                            else
                            {
                                // 击打错误，重置
                                for (var i = 0; i < 5; i++) _branchStates[i] = PowerRuneBranchState.Idle;
                                for (var i = 0; i < 50; i++) _targetStates[i] = PowerRuneBranchState.Idle;
                                _lastHitTime = (float)NetworkTime.time;
                                _score = 0;
                                _branchStates[Random.Range(0, 5)] = PowerRuneBranchState.CanHit;
                                UpdateBranchStates();
                            }
                        }
                    }

                    break;


                case ActionID.Stage.PowerRuneActivated:
                    var activatedAction = (PowerRuneActivated)action;
                    if (activatedAction.Camp != id.camp)
                    {
                        if(!_isLarge)
                        {
                            _runeState = PowerRuneState.Unavailable;
                            for (var i = 0; i < 5; i++) _branchStates[i] = PowerRuneBranchState.Idle;
                            for (var i = 0; i < 50; i++) _targetStates[i] = PowerRuneBranchState.Idle;
                        }

                        UpdateBranchStates();
                    }
                    
                    break;
                
            }
        }

        /// <summary>
        /// 能量机关旋转和激活逻辑。
        /// </summary>
        protected override void FixedUpdate()
        {
            base.FixedUpdate();
            if (!isServer) return;

           /* if (NetworkTime.time >= _nextAvailableTime && ClockStore.Instance().powerAvailable)
            {
                if (_runeState == PowerRuneState.Unavailable)
                {
                    _runeState = PowerRuneState.Available;
                }
            }*/

            // 旋转
            if (NetworkTime.time >= _nextAvailableTime && NetworkTime.time < _nextAvailableTime + 31)
            {
                if (!_isLarge)
                {
                    if (_runeState == PowerRuneState.Available || _runeState == PowerRuneState.Activating)
                    {
                        //超过30s没击打成功停转
                        if (NetworkTime.time >= _nextAvailableTime + 30 && !_isActivated)
                        {
                            Debug.Log(_runeState);
                            _runeState = PowerRuneState.Unavailable;
                            for (var i = 0; i < 5; i++) _branchStates[i] = PowerRuneBranchState.Idle;
                            for (var i = 0; i < 50; i++) _targetStates[i] = PowerRuneBranchState.Idle;
                            UpdateBranchStates();
                        }

                        var rotation = _speed * (360 / (2 * Mathf.PI)) * Time.fixedDeltaTime;
                        //if (id.camp == Identity.Camps.Red) rotation *= -1;
                        transform.Rotate(Vector3.up, rotation);
                    }
                }
                else
                {
                    if (_runeState != PowerRuneState.Unavailable)
                    {
                        if (NetworkTime.time >= _nextAvailableTime + 30)
                        {
                            Debug.Log(_runeState);
                            for (var i = 0; i < 5; i++) _branchStates[i] = PowerRuneBranchState.Idle;
                            for (var i = 0; i < 50; i++) _targetStates[i] = PowerRuneBranchState.Idle;
                            _runeState = PowerRuneState.Unavailable;
                            UpdateBranchStates();
                        }

                        else
                        {
                            _speed = _a * Mathf.Sin(_w * Time.time) + _b;

                            var rotation = _speed * (360 / (2 * Mathf.PI)) * Time.fixedDeltaTime;
                            transform.Rotate(Vector3.up, rotation);
                        }
                    }
                }
            }

            // 是否有机器人在激活点
            if (_canStartActivation && _runeState == PowerRuneState.Available 	)
            {
                // 激活第一个分支
                _runeState = PowerRuneState.Activating;
                _lastRuneState	 = PowerRuneState.Activating;

                _lastHitTime = (float)NetworkTime.time;
                for (var i = 0; i < 5; i++)
                {
                    _branchStates[i] = PowerRuneBranchState.Idle;
                }

                for (var i = 0; i < 50; i++) _targetStates[i] = PowerRuneBranchState.Idle;

                _branchStates[Random.Range(0, 5)] = PowerRuneBranchState.CanHit;
                UpdateBranchStates();
            }
            // 是否已离开激活点
            else if (!_canStartActivation && _runeState == PowerRuneState.Activating)
            {
                _runeState = PowerRuneState.Available;
                _lastRuneState	 = PowerRuneState.Available;
                _score = 0;
                for (var i = 0; i < 5; i++) _branchStates[i] = PowerRuneBranchState.Idle;
                for (var i = 0; i < 50; i++) _targetStates[i] = PowerRuneBranchState.Idle;
                UpdateBranchStates();
            }
            else if (_canStartActivation && _runeState == PowerRuneState.Unavailable && _lastRuneState	!=_runeState)
            {
                _runeState = PowerRuneState.Unavailable;
                _lastRuneState	 = PowerRuneState.Unavailable;
                _score = 0;
                for (var i = 0; i < 5; i++) _branchStates[i] = PowerRuneBranchState.Idle;
                for (var i = 0; i < 50; i++) _targetStates[i] = PowerRuneBranchState.Idle;
                UpdateBranchStates();
            }


            // 检查
            if (_runeState == PowerRuneState.Activating)
            {
                if ((float)NetworkTime.time - _lastHitTime > 2.5f)
                {
                    // 击打超时，重置
                    for (var i = 0; i < 5; i++) _branchStates[i] = PowerRuneBranchState.Idle;
                    for (var i = 0; i < 50; i++) _targetStates[i] = PowerRuneBranchState.Idle;
                    _lastHitTime = (float)NetworkTime.time;
                    _branchStates[Random.Range(0, 5)] = PowerRuneBranchState.CanHit;
                    _score = 0;
                    UpdateBranchStates();
                }
            }

            if (_isActivated)
            {
                var activateTime = (float)NetworkTime.time - _activateTime;
                if (activateTime > 45)
                {
                    _runeState = PowerRuneState.Unavailable;
                    _score = 0;
                    for (var i = 0; i < 5; i++) _branchStates[i] = PowerRuneBranchState.Idle;
                    for (var i = 0; i < 50; i++) _targetStates[i] = PowerRuneBranchState.Idle;
                    UpdateBranchStates();
                    _isActivated = false;
                }
               /* else if (activateTime > 45)
                {
                    _runeState = PowerRuneState.Unavailable;
                    _score = 0;
                    for (var i = 0; i < 5; i++) _branchStates[i] = PowerRuneBranchState.Idle;
                    for (var i = 0; i < 50; i++) _targetStates[i] = PowerRuneBranchState.Idle;
                    UpdateBranchStates();
                }*/

                else
                {
                    var percentage = activateTime / 45;
                    for (var i = 0; i < 5; i++)
                    {
                        Dispatcher.Instance().SendChild(
                            new SetPowerRunePercentage
                            {
                                Parent	 = new ChildIdentity(ChildType.Fan,i),
                                Receiver = new ChildIdentity(ChildType.BranchLight, i),
                                Percentage = (float)Math.Round(1 - percentage, 1)
                            }, id);
                    }
                }

                if (activateTime > 55.0 && activateTime < 55.1)
                {
                    Dispatcher.Instance().Send(new DroneWarning
                    {
                        warningType = DroneWarning.WarningType.PowerRune
                    });

                }
                
            }
        }

        /// <summary>
        /// 更新灯光状态显示。
        /// </summary>
        private void UpdateBranchStates()
        {
            for (var i = 0; i < 5; i++)
            {
                switch (_branchStates[i])
                {
                    case PowerRuneBranchState.Idle:
                        // 关闭所有灯光
                        foreach (var lightType in new List<ChildType>
                                     { ChildType.RingLight, ChildType.FrameLight, ChildType.BranchLight })
                        {
                            Dispatcher.Instance().SendChild(
                                new TurnPowerRuneLight
                                {
                                    Parent	 = new ChildIdentity(ChildType.Fan,i),
                                    Receiver = new ChildIdentity(lightType, i),
                                    IsOn = false
                                }, id);
                        }

                        Dispatcher.Instance().SendChild(new TurnPowerRuneLight
                        {
                            Parent	 = new ChildIdentity(ChildType.Fan,i),
                            Receiver = new ChildIdentity(ChildType.BranchLight, (i + 1) * 5),
                            IsOn = false
                        }, id);

                        // 关闭装甲板
                        
                        Dispatcher.Instance().SendChild(new TurnArmor
                        {
                            ReceiverChild = new ChildIdentity(ChildType.Armor, i),
                            IsOn = false
                        }, id);

                        break;

                    case PowerRuneBranchState.CanHit:
                        // 打开顶部和流动灯光
                        foreach (var lightType in new List<ChildType>
                                     { ChildType.RingLight, ChildType.BranchLight })
                        {
                            Dispatcher.Instance().SendChild(
                                new TurnPowerRuneLight
                                {
                                    Parent	 = new ChildIdentity(ChildType.Fan,i),
                                    Receiver = new ChildIdentity(lightType, i),
                                    IsOn = true
                                }, id);
                        }

                        Dispatcher.Instance().SendChild(
                            new TurnPowerRuneLight
                            {
                                Parent	 = new ChildIdentity(ChildType.Fan,i),
                                Receiver = new ChildIdentity(ChildType.BranchLight, (i + 1) * 5),
                                IsOn = true
                            }, id);
                        
                        //灯条状态
                        _targetStates[i * 10 + 1] = PowerRuneBranchState.Lit;
                        _targetStates[i * 10 + 3] = PowerRuneBranchState.Lit;
                        _targetStates[i * 10 + 8] = PowerRuneBranchState.Lit;
                        
                        Dispatcher.Instance().SendChild(
                            new SetPowerRuneLightState
                            {
                                Parent	 = new ChildIdentity(ChildType.Fan,i),
                                Receiver = new ChildIdentity(ChildType.RingLight, i),
                                State = LightState.On
                            }, id);
                        Dispatcher.Instance().SendChild(
                            new SetPowerRuneLightState
                            {
                                Parent	 = new ChildIdentity(ChildType.Fan,i),
                                Receiver = new ChildIdentity(ChildType.BranchLight, i),
                                State = LightState.On
                            }, id);
                        Dispatcher.Instance().SendChild(
                            new SetPowerRuneLightState
                            {
                                Parent	 = new ChildIdentity(ChildType.Fan,i),
                                Receiver = new ChildIdentity(ChildType.BranchLight, (i + 1) * 5),
                                State = LightState.Flow
                            }, id);

                        // 关闭侧枝灯光
                        Dispatcher.Instance().SendChild(
                            new TurnPowerRuneLight
                            {
                                Parent	 = new ChildIdentity(ChildType.Fan,i),
                                Receiver = new ChildIdentity(ChildType.FrameLight, i),
                                IsOn = false
                            }, id);

                        // 激活装甲板

                        Dispatcher.Instance().SendChild(new TurnArmor
                            {
                                ReceiverChild = new ChildIdentity(ChildType.Armor, i),
                                IsOn = true
                            }, id);

                        break;

                    case PowerRuneBranchState.Lit:
                        // 打开所有灯光
                        foreach (var lightType in new List<ChildType>
                                     { ChildType.FrameLight, ChildType.BranchLight })
                        {
                            Dispatcher.Instance().SendChild(
                                new TurnPowerRuneLight
                                {
                                    Parent	 = new ChildIdentity(ChildType.Fan,i),
                                    Receiver = new ChildIdentity(lightType, i),
                                    IsOn = true
                                }, id);
                            Dispatcher.Instance().SendChild(
                                new SetPowerRuneLightState

                                {
                                    Parent	 = new ChildIdentity(ChildType.Fan,i),
                                    Receiver = new ChildIdentity(lightType, i),
                                    State = LightState.On
                                }, id);
                        }
                        
                        //关闭十字准心灯光
                        Dispatcher.Instance().SendChild(
                            new TurnPowerRuneLight
                            {
                                Parent	 = new ChildIdentity(ChildType.Fan,i),
                                Receiver = new ChildIdentity(ChildType.RingLight, i),
                                IsOn = false
                            }, id);
                        
                        //关闭流动灯条
                        Dispatcher.Instance().SendChild(
                            new TurnPowerRuneLight
                            {
                                Parent	 = new ChildIdentity(ChildType.Fan,i),
                                Receiver = new ChildIdentity(ChildType.BranchLight, (i + 1) * 5),
                                IsOn = false
                            }, id);
                        
                        // 关闭装甲板

                        Dispatcher.Instance().SendChild(new TurnArmor
                        {
                            ReceiverChild = new ChildIdentity(ChildType.Armor, i),
                            IsOn = false
                        }, id);

                        break;
                }
            }

            for (var i = 0; i < 50; i++)
            {
                switch (_targetStates[i])
                {
                    case PowerRuneBranchState.Idle:
                        // 关闭灯光
                        Dispatcher.Instance().SendChild(
                            new TurnPowerRuneLight
                            {
                                Parent	 = new ChildIdentity(ChildType.Fan,(i-i%10)/10),
                                Receiver = new ChildIdentity(ChildType.TargetLight, i),
                                IsOn = false
                            }, id);
                        break;

                    case PowerRuneBranchState.Lit:
                        // 打开灯光
                        Dispatcher.Instance().SendChild(
                            new TurnPowerRuneLight
                            {
                                Parent	 = new ChildIdentity(ChildType.Fan,(i-i%10)/10),
                                Receiver = new ChildIdentity(ChildType.TargetLight, i),
                                IsOn = true
                            }, id);
                        Dispatcher.Instance().SendChild(
                            new SetPowerRuneLightState

                            {
                                Parent	 = new ChildIdentity(ChildType.Fan,(i-i%10)/10),
                                Receiver = new ChildIdentity(ChildType.TargetLight, i),
                                State = LightState.On
                            }, id);
                        break;
                }
            }
        }

        /// <summary>
        /// 激活后灯光效果。
        /// </summary>
        /// <returns></returns>
        private IEnumerator ActivatedBlink()
        {
            for (var i = 0; i < 10; i++)
            {
                for (var j = 0; j < 5; j++)
                {
                    _branchStates[j] = PowerRuneBranchState.Idle;
                }

                UpdateBranchStates();

                yield return new WaitForSeconds(0.1f);

                for (var j = 0; j < 5; j++)
                {
                    _branchStates[j] = PowerRuneBranchState.Lit;
                }

                UpdateBranchStates();

                yield return new WaitForSeconds(0.1f);
            }

            for (var i = 0; i < 5; i++)
            {
                Dispatcher.Instance().SendChild(
                    new SetPowerRuneLightState
                    {
                        Parent	 = new ChildIdentity(ChildType.Fan,i),
                        Receiver = new ChildIdentity(ChildType.BranchLight, (i + 1) * 5),
                        State = LightState.Percentage
                    }, id);
            }
        }

        /// <summary>
        /// 延迟能量机关关闭
        /// </summary>
        /// <param name="a">距离能量机关关闭的时间</param>
        /// <returns></returns>
        private IEnumerator WaitToClose(float a)
        {
            yield return new WaitForSeconds(a);
            _runeState = PowerRuneState.Unavailable;
            for (var i = 0; i < 5; i++) _branchStates[i] = PowerRuneBranchState.Idle;
            UpdateBranchStates();
            _isActivated = false;
        }
    }
}