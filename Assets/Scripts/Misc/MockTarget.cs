using UnityEngine;

namespace Misc
{
    /// <summary>
    /// 早期用于测试自瞄的目标。
    /// </summary>
    [RequireComponent(typeof(Collider))]
    public class MockTarget : MonoBehaviour
    {
        private bool _cloned;

        /// <summary>
        /// 初始化颜色。
        /// </summary>
        private void Start()
        {
            GetComponent<Renderer>().material.color = Color.red;
        }

        /// <summary>
        /// 被击中后随机移动。
        /// </summary>
        private void OnCollisionEnter()
        {
            if (!_cloned)
            {
                const float randRange = 0.5f;
                Instantiate(gameObject,
                    new Vector3(
                        Random.Range(-randRange, randRange) - 2,
                        Random.Range(-randRange, randRange),
                        Random.Range(-randRange, randRange) - 6
                    ),
                    Quaternion.identity);
                _cloned = true;
            }

            Destroy(gameObject);
        }
    }
}