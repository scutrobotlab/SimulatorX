using System.Collections;
using System.Collections.Generic;
using Gameplay;
using Gameplay.Attribute;
using Infrastructure;
using Mirror;
using UnityEngine;
using UnityEngine.Serialization;

namespace Controllers.Items
{
    /// <summary>
    /// <c>Bullet</c> 用于控制子弹。
    /// <br/>主要用于控制子弹物理组件的自动销毁，节省性能。
    /// </summary>
    public class Dart : Bullet
    {
        public GameObject target;
        private bool _destroyTriggered;
    
        /// <summary>
        /// 飞镖速度
        /// </summary>
        public float dartSpeed = 250f;

        private Rigidbody _rigidbody;


        /// <summary>
        /// 初始化组件。
        /// </summary>
        private void Start()
        {
            _rigidbody = GetComponent<Rigidbody>();
            var trailRenderer = GetComponent<TrailRenderer>();
            if (trailRenderer != null)
            {
                if (NetworkClient.active && EntityManager.Instance().LocalRobot().IsRobot())
                {
                    trailRenderer.enabled = false;
                }
                else
                {
                    trailRenderer.material = owner.camp == Identity.Camps.Red ? redTrail : blueTrail;
                }
            }
        }

        /// <summary>
        /// 跟踪目标。
        /// </summary>
        private void FixedUpdate()
        {
            if (_rigidbody != null)
            {
                //飞镖追踪
                if (caliber == MechanicType.CaliberType.Dart && !_destroyTriggered)
                {
                    if (target == null) return;
                    var lookDirection = (target.transform.position - transform.position).normalized;
                    _rigidbody.AddForce(lookDirection * dartSpeed);

                }
            }
        }
        
        /// <summary>
        /// 碰撞后不可再追踪
        /// </summary>
        private void OnCollisionEnter()
        {
            if (_destroyTriggered) return;
            _destroyTriggered = true;
        }
    }
}