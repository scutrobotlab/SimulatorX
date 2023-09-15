using Controllers.Ore;
using UnityEngine;

namespace Misc
{
    [RequireComponent(typeof(Collider))]
    public class OreEater : MonoBehaviour
    {
        private void Start()
        {
            GetComponent<Collider>().isTrigger = true;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.GetComponent<OreStoreBase>() != null)
            {
                Destroy(other.gameObject);
            }
        }
    }
}