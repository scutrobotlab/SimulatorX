using System.Collections;
using Controllers.Items;
using Gameplay.Events;
using Infrastructure.Child;
using UnityEngine;

namespace Controllers.Child
{
    /// <summary>
    /// <c>Magazine</c> 管理机器人弹舱。
    /// <br/>记录并收集落入弹舱的子弹，统计子弹总数。
    /// </summary>
    public class Magazine : StoreChildBase
    {
        /// <summary>
        /// 标识身份。
        /// </summary>
        protected override void Identify()
        {
            id = new ChildIdentity(ChildType.Magazine);
        }

        /// <summary>
        /// 检测到子弹落入时，触发收集流程。
        /// </summary>
        /// <param name="other"></param>
        private void OnTriggerEnter(Collider other)
        {
            var bullet = other.GetComponent<Bullet>();
            if (!bullet) return;
            if (!bullet.collected)
            {
                // TODO: 收集
                DispatcherSend(new AddBullet
                {
                    Receiver = Root.id,
                    Type = bullet.caliber
                });
                bullet.collected = true;
            }

            StartCoroutine(SetParent(other.gameObject));
            StartCoroutine(DestroyCollected(other.gameObject));
        }

        /// <summary>
        /// 防止子弹落入后弹出。
        /// </summary>
        /// <param name="other"></param>
        private void OnTriggerExit(Collider other)
        {
            if (!other.GetComponent<Bullet>()) return;
            Destroy(other.gameObject);
        }

        /// <summary>
        /// 经过延迟，将子弹固定在弹舱内。
        /// </summary>
        /// <param name="bullet"></param>
        /// <returns></returns>
        private IEnumerator SetParent(GameObject bullet)
        {
            yield return new WaitForSeconds(0.5f);
            if (bullet.gameObject == null) yield break;
            bullet.transform.SetParent(transform);
            Destroy(bullet.GetComponent<Rigidbody>());
        }

        /// <summary>
        /// 一段时间过后，清除弹舱内的子弹。
        /// </summary>
        /// <param name="bullet"></param>
        /// <returns></returns>
        private static IEnumerator DestroyCollected(GameObject bullet)
        {
            yield return new WaitForSeconds(10);
            if (bullet.gameObject == null) yield break;
            Destroy(bullet.gameObject);
        }
    }
}