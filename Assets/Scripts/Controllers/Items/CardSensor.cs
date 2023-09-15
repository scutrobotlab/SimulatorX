using Gameplay;
using UnityEngine;

namespace Controllers.Items
{
    /// <summary>
    /// 复活卡传感器。
    /// 用于检测被刷卡的机器人。
    /// </summary>
    [RequireComponent(typeof(Collider))]
    public class CardSensor : MonoBehaviour
    {
        [HideInInspector] public RobotStoreBase target;

        /// <summary>
        /// 初始化组件。
        /// </summary>
        private void Start()
        {
            GetComponent<Collider>().isTrigger = true;
        }

        /// <summary>
        /// 记录被刷卡的机器人。
        /// </summary>
        /// <param name="other"></param>
        private void OnTriggerEnter(Collider other)
        {
            var robotStore = other.GetComponent<RobotStoreBase>();
            if (robotStore == null) robotStore = other.GetComponentInParent<RobotStoreBase>();
            if (robotStore == null) return;
            target = robotStore;
        }

        /// <summary>
        /// 刷卡结束。
        /// </summary>
        /// <param name="other"></param>
        private void OnTriggerExit(Collider other)
        {
            var robotStore = other.GetComponent<RobotStoreBase>();
            if (robotStore == null) robotStore = other.GetComponentInParent<RobotStoreBase>();
            if (robotStore == null) return;
            if (robotStore == target)
            {
                target = null;
            }
        }
    }
}