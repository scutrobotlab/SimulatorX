using TMPro;
using UnityEngine;
using UnityEngine.UI;


namespace UI
{
    /// <summary>
    /// 无人机遮挡页面。
    /// </summary>
    public class DroneCoverPanel : MonoBehaviour
    {
        public Image panel;
        public TMP_Text countDown;

        private void Start()
        {
            //StartSession();
            EndSession();
        }

        public void StartSession()
        {
            panel.gameObject.SetActive(true);
            countDown.text = "";
        }

        public void EndSession()
        {
            panel.gameObject.SetActive(false);
        }

        public void CountDown(int seconds) => countDown.text = seconds + "s";
    }
    
}
