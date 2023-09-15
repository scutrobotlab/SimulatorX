using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    /// <summary>
    /// 过热遮挡页面。
    /// </summary>
    public class OverheatPanel : MonoBehaviour
    {
        public Image panel;

        private void Start()
        {
            EndSession();
        }

        public void StartSession()
        {
            panel.gameObject.SetActive(true);
        }

        public void EndSession()
        {
            panel.gameObject.SetActive(false);
        }
    }
}