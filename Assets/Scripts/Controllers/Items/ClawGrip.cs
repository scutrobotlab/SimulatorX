using Gameplay.Events;
using Infrastructure;
using UnityEngine;

namespace Controllers.Items
{
    /// <summary>
    /// 工程爪子传感器。
    /// </summary>
    [RequireComponent(typeof(Collider))]
    public class ClawGrip : MonoBehaviour
    {
        private StoreBase _toCatch;
        private StoreBase _catching;
        private Vector3 _relativePosition;
        private Quaternion _relativeRotation;

        /// <summary>
        /// 初始化组件。
        /// </summary>
        private void Start()
        {
            GetComponent<Collider>().isTrigger = true;
        }

        /// <summary>
        /// 检测抓到的机器人。
        /// </summary>
        /// <param name="other"></param>
        private void OnTriggerEnter(Collider other)
        {
            var store = other.GetComponent<StoreBase>();
            if (store == null) store = other.GetComponentInParent<StoreBase>();
            if (store == null) return;
            if (store.id.IsGroundRobot() || store.id.role == Identity.Roles.Obstacle)
            {
                _toCatch = store;
            }
        }

        /// <summary>
        /// 机器人离开触发区域。
        /// </summary>
        /// <param name="other"></param>
        private void OnTriggerExit(Collider other)
        {
            var store = other.GetComponent<StoreBase>();
            if (store == null) store = other.GetComponentInParent<StoreBase>();
            if (store == null) return;
            if (_toCatch != null && store.id == _toCatch.id)
            {
                _toCatch = null;
            }
        }

        /// <summary>
        /// 尝试抓取，并记录相对姿态。
        /// </summary>
        public void Catch()
        {
            if (_toCatch != null)
            {
                _catching = _toCatch;
                Dispatcher.Instance().Send(new CatchState
                {
                    Receiver = _catching.id,
                    Catching = true
                });
                Transform selfTransform;
                _relativePosition = (selfTransform = transform).InverseTransformPoint(_catching.transform.position);
                _relativeRotation = Quaternion.Inverse(selfTransform.rotation) * _catching.transform.rotation;
            }
        }

        /// <summary>
        /// 取消抓取。
        /// </summary>
        public void Release()
        {
            if (_catching == null) return;
            Dispatcher.Instance().Send(new CatchState
            {
                Receiver = _catching.id,
                Catching = false
            });
            _catching = null;
        }

        /// <summary>
        /// 若抓住了机器人，则保持其相对姿态。
        /// </summary>
        private void FixedUpdate()
        {
            if (_catching != null)
            {
                // 保持相对姿态
                var selfTransform = transform;
                var catchingTransform = _catching.gameObject.transform;
                var newPosition = selfTransform.TransformPoint(_relativePosition);
                var newRotation = selfTransform.rotation * _relativeRotation;
                catchingTransform.position = newPosition;
                catchingTransform.rotation = newRotation;
            }
        }
    }
}