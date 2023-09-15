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
    public class Bullet : MonoBehaviour
    {
        public Identity owner;
        public MechanicType.CaliberType caliber;
        private GameObject _target;
        public bool collected;

        // 空速记录
        // 若记录不足，表示枪口离目标过近。
        public Vector3 lastVelocity => _lastVelocities.Count > 1 ? _lastVelocities.Dequeue() : Vector3.one * 30;
        
        /// <summary>
        /// 用于储存发生碰撞前帧数物体的速度队列，一帧记录一次速度。
        /// </summary>
        private readonly Queue<Vector3> _lastVelocities = new Queue<Vector3>();
        private Rigidbody _rigidbody;
       
        // 尾迹材质
        public Material redTrail;
        public Material blueTrail;

        /// <summary>
        /// 子弹第一次与物体碰撞触发销毁定时器。
        /// </summary>
        private void OnCollisionEnter()
        {
            if (GetComponent<Rigidbody>() != null)
                StartCoroutine(RemovePhysics());
        }

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
        /// 保留空速数据。
        /// </summary>
        private void FixedUpdate()
        {
            if (_rigidbody != null)
            {

                if (caliber != MechanicType.CaliberType.Dart)
                {
                    //如果记录的速度个数大于3，则清空队列（只保存三帧速度）
                    if (_lastVelocities.Count > 3)
                    {
                        _lastVelocities.Dequeue();
                    }
                
                    _lastVelocities.Enqueue(_rigidbody.velocity);
                }
                
            }
            else _lastVelocities.Clear();
            //飞出场外销毁
            if (this.transform.position.x > 100 || this.transform.position.y > 100 || this.transform.position.z > 100)
            {
                if (GetComponent<Rigidbody>() != null)
                    StartCoroutine(RemovePhysics());
            }
                
        }
        
        /// <summary>
        /// 销毁子弹物理组件。
        /// 延迟10秒后开始，当子弹停下时执行销毁。
        /// </summary>
        /// <returns></returns>
        private IEnumerator RemovePhysics()
        {
            if (EntityManager.Instance().isServer)
            {
                yield return new WaitForSeconds(5);
                Destroy(gameObject);
            }
            else
            {
                yield return new WaitForSeconds(5);
                Destroy(gameObject);
            }
            
        }
    }
}