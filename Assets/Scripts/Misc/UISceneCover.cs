using System.Collections;
using UnityEngine;

namespace Misc
{
    public class UISceneCover : MonoBehaviour
    {
        public GameObject panel;

        private void Start()
        {
            StartCoroutine(DelayRemove());
        }

        private IEnumerator DelayRemove()
        {
            yield return new WaitForSeconds(0.2f);
            panel.SetActive(false);
        }
    }
}