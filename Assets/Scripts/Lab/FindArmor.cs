using Controllers.Child;
using UnityEngine;

namespace Lab
{
    public class FindArmor : MonoBehaviour
    {
        private void Update()
        {
            Debug.Log(FindObjectsOfType<Armor>().Length);
        }
    }
}