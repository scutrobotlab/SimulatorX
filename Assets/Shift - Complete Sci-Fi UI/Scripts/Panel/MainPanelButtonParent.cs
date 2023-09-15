using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Michsky.UI.Shift
{
#if !UNITY_2021_2_OR_NEWER
    public class MainPanelButtonParent : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        private List<Animator> mainButtons = new List<Animator>();

        void Awake()
        {
            foreach (Transform child in transform) { mainButtons.Add(child.GetComponent<Animator>()); }
            for (int i = 0; i < mainButtons.Count; ++i) { mainButtons[i].Play("Dissolve to Normal"); }
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            for (int i = 0; i < mainButtons.Count; ++i)
            {
                if (!mainButtons[i].GetCurrentAnimatorStateInfo(0).IsName("Normal to Pressed"))
                    mainButtons[i].Play("Normal to Dissolve");
            }
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            for (int i = 0; i < mainButtons.Count; ++i)
            {
                if (!mainButtons[i].GetCurrentAnimatorStateInfo(0).IsName("Normal to Pressed"))
                    mainButtons[i].Play("Dissolve to Normal");
            }
        }
    }
#endif
}