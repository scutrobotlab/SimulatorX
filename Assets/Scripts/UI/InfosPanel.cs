using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

namespace UI
{
    /// <summary>
    /// 左下角滚动文字信息。
    /// </summary>
    public class InfosPanel : MonoBehaviour
    {
        public List<TMP_Text> texts = new List<TMP_Text>();
        private readonly Queue<string> _infos = new Queue<string>();

        private void Start()
        {
            foreach (var text in texts)
            {
                text.text = "";
            }
        }

        /// <summary>
        /// 添加一条信息，将自动滚动。
        /// </summary>
        /// <param name="info">信息文本</param>
        public void AddInfo(string info)
        {
            _infos.Enqueue(info);
            var infosArray = _infos.ToArray().Reverse().ToArray();
            for (var i = 0; i < infosArray.Length; i++)
            {
                if (i >= texts.Count) break;
                texts[i].text = infosArray[i];
            }
        }
    }
}