using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cinemachine;
using Controllers.Items;
using Gameplay.Events;
using Infrastructure;
using Mirror;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Controllers
{
    public class DartLauncherStore : StoreBase
    {
        /// <summary>
        /// 飞镖的动画控制器。
        /// </summary>
        private Animator _dartController;

        /// <summary>
        /// 重力常数
        /// </summary>
        private float _g = 9.81f;

        /// <summary>
        /// 误差控制
        /// </summary>
        public float error;

        //飞镖预制件
        public GameObject dart;
        private int _numberOfDarts = 4;
        
        //打击目标
        public GameObject outpostTarget;
        public GameObject baseTarget;
        private GameObject _finalTarget;
        
        /// <summary>
        /// 发射点
        /// </summary>
        public GameObject spawnLocation;
        
        
        /// <summary>
        /// 飞镖初始速度
        /// </summary>
        public float originSpeed = 2.5f;
        
        private static readonly int Unfold = Animator.StringToHash("Unfold");
        private static readonly int Close = Animator.StringToHash("Close");

        protected override void Start()
        {
            base.Start();
            _dartController = GetComponent<Animator>();
            _finalTarget = outpostTarget;
        }
        
        /// <summary>
        /// 声明感兴趣事件。
        /// </summary>
        /// <returns></returns>
        public override List<string> InputActions()
        {
            return base.InputActions().Concat(new List<string>
            {
                ActionID.Stage.DartFire,
                ActionID.Stage.OpenLaunchStation,
                ActionID.ChangeTarget.ToBase,
                ActionID.ChangeTarget.ToOutpost
            }).ToList();
        }
        protected override void Identify()
        {
        }
        
        
        /// <summary>
        /// 处理事件。
        /// </summary>
        /// <param name="action"></param>
        public override void Receive(IAction action)
        {
            base.Receive(action);
            if (!isServer) return;
            switch (action.ActionName())
            {
                //飞镖发射
                case ActionID.Stage.DartFire:
                    var dartFireAction = (DartFire) action;
                    if (dartFireAction.Camp == id.camp && dartFireAction.Role == id.role)
                    {
                        if (_numberOfDarts>0)
                        {
                            error = dartFireAction.Error;
                            _numberOfDarts--;
                            var newDart = Instantiate(dart, spawnLocation.transform.position, Quaternion.identity);
                            newDart.GetComponent<Dart>().owner = id;
                            
                            var realVelocity = CalculateVel(_finalTarget.transform,spawnLocation.transform);
                            if (_finalTarget == outpostTarget)
                                realVelocity += new Vector3(0, -0.03f, 0);
                            else
                                realVelocity += new Vector3(0, 0.05f, 0);

                            if(error > 10e-3)
                                realVelocity = Vector3.Scale(realVelocity,
                                    Vector3.one + new Vector3(Random.Range(-error, error), Random.Range(-error, error),Random.Range(-error, error)));

                            if (id.camp == Identity.Camps.Blue)
                            {
                                newDart.GetComponent<Rigidbody>().velocity = realVelocity;
                                if (isServerOnly)
                                {
                                    LaunchRpc(realVelocity,_finalTarget);
                                }
                            
                            }
                            else
                            {
                                newDart.GetComponent<Rigidbody>().velocity =realVelocity;
                                if (isServerOnly)
                                {
                                    LaunchRpc(realVelocity,_finalTarget);
                                }
                            }
                        }
                        
                    }
                    break;
                
                //打开飞镖闸门
                case ActionID.Stage.OpenLaunchStation:
                    var openLaunchStationAction = (OpenLaunchStation)action;
                    if (openLaunchStationAction.Camp == id.camp && openLaunchStationAction.Role == id.role)
                    {
                        _dartController.SetTrigger(Unfold);
                        if (isServerOnly)
                        { LaunchUnfoldRpc(); }
                        
                        //30秒后关闭飞镖闸门
                        StartCoroutine(CloseRoutine());
                    }
                    break;
                
                case ActionID.ChangeTarget.ToBase:
                    _finalTarget = baseTarget;
                    break;
                
                case ActionID.ChangeTarget.ToOutpost:
                    _finalTarget = outpostTarget;
                    break;
            }
        }
        
        
        [ClientRpc]
        private void LaunchRpc(Vector3 velocity,GameObject target)
        {
            var newDart = Instantiate(dart, spawnLocation.transform.position, Quaternion.identity);
            newDart.GetComponent<Dart>().owner = id;
            newDart.GetComponent<Dart>().target = target;
            newDart.GetComponent<Rigidbody>().velocity = velocity;
        }
        
        [ClientRpc]
        private void LaunchUnfoldRpc()
        {
            _dartController.SetTrigger(Unfold);
        }

        private IEnumerator CloseRoutine()
        {
            yield return new WaitForSeconds(30);
            _dartController.SetTrigger(Close);
        }
        
        private Vector3 CalculateVel(Transform to ,Transform from)
        {
            
            Vector3 diff = to.position - from.position;
            Vector3 output = new Vector3(diff.x, 0, diff.z).normalized;
            
            output = (output + Vector3.up).normalized;//keep 45 degree

            double distance = Math.Sqrt(diff.x * diff.x + diff.z * diff.z);

            var a = Math.Sqrt(_g / (distance - diff.y)) * distance;

            output = (float)a * output;

            return output;


        }
    }
}