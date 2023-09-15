using System.Collections;
using Honeti;
using Infrastructure;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    /// <summary>
    /// 击杀提示面板。
    /// </summary>
    public class KillPanel : MonoBehaviour
    {
        public Image panel;
        public TMP_Text killer;
        public TMP_Text victim;
        public TMP_Text method;

        private void Start()
        {
            EndSession();
        }

        /// <summary>
        /// 显示一次击杀提示。
        /// </summary>
        /// <param name="killerID">击杀者</param>
        /// <param name="victimID">受害者</param>
        /// <param name="methodStr">击杀方式</param>
        public void StartSession(Identity killerID, Identity victimID, string methodStr)
        {
            panel.gameObject.SetActive(true);
            panel.color = killerID.camp switch
            {
                Identity.Camps.Red => Color.red,
                Identity.Camps.Blue => Color.blue,
                _ => Color.yellow
            };
            killer.text = killerID.Describe();
            victim.text = victimID.Describe();
            method.text = methodStr;
            StartCoroutine(DelayEndSession());
        }

        /// <summary>
        /// 将 <c>Identity</c> 转换为文字描述。
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        private void EndSession()
        {
            panel.gameObject.SetActive(false);
        }

        private IEnumerator DelayEndSession()
        {
            yield return new WaitForSeconds(2.5f);
            EndSession();
        }
    }
}