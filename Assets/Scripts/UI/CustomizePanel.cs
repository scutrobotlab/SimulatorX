using System.Collections;
using System.Collections.Generic;
using Doozy.Runtime.UIManager.Containers;
using Gameplay;
using Gameplay.Customize;
using UnityEngine;
using TMPro;

namespace UI
{
    /// <summary>
    /// 自定义参数面板。
    /// </summary>
    public class CustomizePanel : MonoBehaviour
    {
        public UIView propertiesView;

        public static readonly Hashtable CustomizePropertyMap = new Hashtable();

        public static void ResetAll()
        {
            foreach (CustomizeProperty customizeProperty in CustomizePropertyMap.Keys)
            {
                var value = (float)CustomizePropertyMap[customizeProperty];
                Debug.Log("newValue = " + value);
                customizeProperty.slider.value = value;
            }
        }

        public void StartSession()
        {
            propertiesView.Show();
        }

        public void EndSession()
        {
            propertiesView.Hide();
        }
    }
}