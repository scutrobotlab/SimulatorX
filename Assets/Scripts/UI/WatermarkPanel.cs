using System;
using Config;
using TMPro;
using UnityEngine;

namespace UI
{
    public class WatermarkPanel : MonoBehaviour
    {
        public TMP_Text versionWatermark;

        // Start is called before the first frame update
        void Start()
        {
            try
            {
                versionWatermark.text = GlobalConfig.Version.GetVersionWatermark();
            }
            finally
            {
            }
        }

        // Update is called once per frame
        void Update()
        {
        }
    }
}