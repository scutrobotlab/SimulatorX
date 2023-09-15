using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    /// <summary>
    /// 用于展示异常状态的面板。
    /// </summary>
    public class AbnormalPanel:MonoBehaviour
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
        /// <param name="seconds">显示时间</param>
        public void StartSession(string titleText, string desc, float seconds)
        {
            panel.gameObject.SetActive(true);
            title.text = titleText;
            description.text = desc;
            StartCoroutine(DelayEndSession(seconds));
        }

        private void EndSession()
        {
            panel.gameObject.SetActive(false);
        }

        private IEnumerator DelayEndSession(float seconds)
        {
            yield return new WaitForSeconds(seconds);
            EndSession();
        }
    }
}