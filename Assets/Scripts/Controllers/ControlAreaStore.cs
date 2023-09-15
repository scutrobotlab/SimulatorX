using System;
using System.Collections.Generic;
using System.Linq;
using Controllers.RobotSensor;
using DG.Tweening.Core.Easing;
using Gameplay;
using Gameplay.Effects;
using Gameplay.Events;
using Infrastructure;
using Mirror;
using Misc;
using UnityEngine;

namespace Controllers
{
    public class ControlAreaStore : BuffAreaStore
    {
        //针对中央控制区
        private readonly Dictionary<Identity.Camps, bool> _gained = new Dictionary<Identity.Camps, bool>
        {
            {Identity.Camps.Blue, false},
            {Identity.Camps.Red, false}
        };
        private bool _canCount;
        private bool _canSearch;
        private float _time = 0;
        //有占领动作阵营
        private Identity.Camps _camps;
        
        //search计时
        private float _countTime;

        // Start is called before the first frame update
        protected override void Start()
        {
            base.Start();
            if (GetComponent<Sensor>()) _sensor = GetComponent<Sensor>();
            if (GetComponent<SensorGroup>()) _sensor = GetComponent<SensorGroup>();
            Debug.Log("sensor "+_sensor.GetType());
            if (_sensor == null)
            {
                throw new Exception("Initializing buff area without sensor.");
            }
            _sensorUtility = new SensorUtility(
                enter =>
                {
                    if (!(enter is RobotStoreBase robot)) return;
                    RobotEnter(robot);
                },
                exit =>
                {
                    if (!(exit is RobotStoreBase robot)) return;
                    RobotExit(robot);
                });
            _canCount = false;
            _canSearch = false;

        }
        
        protected override void FixedUpdate()
        {
            base.FixedUpdate();
            if (!isServer) return;
            if (_sensor != null)
            {
                _sensorUtility.Update(_sensor);
            }
            
            //针对中央控制区
            if (_canCount && NetworkTime.time - _time > 6.0f)
            {
                Dispatcher.Instance().Send(new OccupyControlArea() //已占领动作
                {
                    Camp = _camps
                });
                _canCount = false;
                _gained[_camps] = true;
                Debug.Log("已占领");
            }

            //机器人进入增益区，向EntityManager确认
            if ( NetworkTime.time > _countTime)
            {
                if (_canSearch)
                {
                    Dispatcher.Instance().Send(new SearchControlEffect());
                }
                if (_gained[Identity.Camps.Red] || _gained[Identity.Camps.Blue])
                {
                    Dispatcher.Instance().Send(new SearchControlEffect());
                }
                _countTime = (float)NetworkTime.time;
                _countTime += 2;
            }
            
        }
        
        public override List<string> InputActions()
        {
            return base.InputActions().Concat(new List<string>
            {
                ActionID.ControlAction.SendOccupiedMessage,
                ActionID.ControlAction.SendLeftMessage
            }).ToList();
        }
        
        public override void Receive(IAction action)
        {
            base.Receive(action);
            if (!isServer) return;
            switch (action.ActionName())
            {
                case ActionID.ControlAction.SendOccupiedMessage:
                    var occupiedMessage = (SendOccupiedMessage)action;
                    _canCount = occupiedMessage.CanCount;
                    _time = occupiedMessage.Time;
                    _camps = occupiedMessage.Camp;

                    break;
                
                case ActionID.ControlAction.SendLeftMessage:
                    var leftMessage = (SendLeftMessage)action;
                    Dispatcher.Instance().Send(new LeaveControlArea()
                    {
                        Camp = leftMessage.Camp
                    });
                    _gained[leftMessage.Camp] = false;
                    
                    break;
                    
            }
        }

        //机器人进入中央控制区
        private void RobotEnter(RobotStoreBase robot)
        {
            if (robot.id.role == Identity.Roles.Hero || (robot.id.role == Identity.Roles.Infantry ) ||
                robot.id.role == Identity.Roles.BalanceInfantry)
            {
                Dispatcher.Instance().Send(new AddEffect
                {
                    Receiver = robot.id,
                    Effect = new ControlBuff()
                });
                
            }
            //Debug.Log("control enter触发");
            _canSearch = true;
        }


        //机器人离开中央控制区
        private void RobotExit(RobotStoreBase robot)
        {
            if (robot.id.role == Identity.Roles.Hero ||
                (robot.id.role == Identity.Roles.Infantry) || robot.id.role == Identity.Roles.BalanceInfantry)
            {
                Dispatcher.Instance().Send(new RemoveEffect
                {
                    Receiver = robot.id,
                    Effect = new ControlBuff()
                });
            }
            //Debug.Log("control exit触发");
            _canSearch = false;
        }

        
    }
}