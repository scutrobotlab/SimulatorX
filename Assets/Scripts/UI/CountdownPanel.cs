using Gameplay;
using TMPro;
using UnityEngine;

namespace UI
{
    /// <summary>
    /// 赛前倒计时数字面板。
    /// </summary>
    public class CountdownPanel : MonoBehaviour
    {
        public TMP_Text countdown;

        private void FixedUpdate()
        {
            if (ClockStore.Instance() == null) return;
            if (!ClockStore.Instance().playing)
            {
                countdown.text = ClockStore.Instance().Countdown() == 0
                    ? ""
                    : ClockStore.Instance().Countdown().ToString();
            }
            else
            {
                countdown.text = "";
            }
        }
    }
}