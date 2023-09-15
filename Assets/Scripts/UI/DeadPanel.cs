using System;
using System.Collections.Generic;
using Gameplay;
using Honeti;
using Infrastructure;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    /// <summary>
    /// 阵亡提示面板。
    /// </summary>
    public class DeadPanel : MonoBehaviour
    {
        public Image panel;
        public TMP_Text countDown;
        public TMP_Text title;
        public List<Image> materialPackage = new List<Image>();

        private void Start()
        {
            Hide();
        }

        public void Show(Enum map,string stage = "")
        {
            switch (map)
            {
                case MapType.RMUL2022:
                    materialPackage[0].gameObject.SetActive(true);
                    title.text = I18N.instance.getValue("^in_the_resurrection");
                    title.color = Color.green;
                    break;
                
            }
            countDown.gameObject.SetActive(true);
            panel.gameObject.SetActive(true);
        }

        public void Hide() => panel.gameObject.SetActive(false);
        public void Countdown(int seconds) => countDown.text = seconds + "s";
    }
}