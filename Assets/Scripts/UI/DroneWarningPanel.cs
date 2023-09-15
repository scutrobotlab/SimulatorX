using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    /// <summary>
    /// 云台手提示
    /// </summary>
    public class DroneWarningPanel : MonoBehaviour
    {
        public Image panel;
        public TMP_Text title;
        public TMP_Text description;

        private void Start()
        {
            EndSession();
        }

        /// <summary>
        /// 显示面板。
        /// </summary>
        /// <param name="titleText">异常状态</param>
        /// <param name="desc">状态描述</param>
        /// <param name="seconds">显示时间(默认为-1，即不进行读秒)</param>
        public void StartSession(string titleText, string desc, float seconds = -1)
        {
            panel.gameObject.SetActive(true);
            title.text = titleText;
            description.text = desc;
            if (seconds > 0)
                StartCoroutine(DelayEndSession(seconds));
        }
        
        /// <summary>
        /// 关闭面板。
        /// </summary>
        public void EndSession()
        {
            panel.gameObject.SetActive(false);
        }
        
        /// <summary>
        /// 协程实现面板读秒后消失。
        /// </summary>
        /// <param name="seconds">面板读的秒数</param>
        /// <returns></returns>
        private IEnumerator DelayEndSession(float seconds)
        {
            yield return new WaitForSeconds(seconds);
            EndSession();
        }
        
    }
}