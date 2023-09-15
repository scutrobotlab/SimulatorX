using Controllers.Ore;
using UnityEngine;

namespace Controllers.Items
{
    /// <summary>
    /// 取矿传感器。
    /// </summary>
    [RequireComponent(typeof(Collider))]
    public class OreGrip : MonoBehaviour
    {
        [HideInInspector] public OreStoreBase ore;

        /// <summary>
        /// 初始化组件。
        /// </summary>
        private void Start()
        {
            GetComponent<Collider>().isTrigger = true;
        }

        /// <summary>
        /// 检测进入的矿石。
        /// </summary>
        /// <param name="other"></param>
        private void OnTriggerEnter(Collider other)
        {
            ore = other.GetComponent<OreStoreBase>();
        }

        /// <summary>
        /// 矿石离开。
        /// </summary>
        /// <param name="other"></param>
        private void OnTriggerExit(Collider other)
        {
            if (other.GetComponent<OreStoreBase>() == ore)
                ore = null;
        }
    }
}