using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Michsky.UI.Shift
{
    public class LayoutGroupPositionFix : MonoBehaviour
    {
        [Header("Settings")]
        public bool fixOnEnable = true;
        public bool fixParent = false;
        public bool fixWithDelay = false;
        float fixDelay = 0.025f;

        void OnEnable()
        {
            if (fixWithDelay == false)
            {
                if (fixOnEnable == true && fixParent == false)
                    LayoutRebuilder.ForceRebuildLayoutImmediate(GetComponent<RectTransform>());
                else if (fixOnEnable == true && fixParent == true)
                    LayoutRebuilder.ForceRebuildLayoutImmediate(transform.parent.GetComponent<RectTransform>());
            }

            else { StartCoroutine("FixDelay"); }
        }

        public void FixLayout()
        {
            if (fixWithDelay == false)
                LayoutRebuilder.ForceRebuildLayoutImmediate(GetComponent<RectTransform>());
            else
                StartCoroutine("FixDelay");
        }

        IEnumerator FixDelay()
        {
            yield return new WaitForSeconds(fixDelay);
            LayoutRebuilder.ForceRebuildLayoutImmediate(GetComponent<RectTransform>());
        }
    }
}