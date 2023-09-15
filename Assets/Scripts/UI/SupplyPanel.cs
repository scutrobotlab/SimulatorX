using System;
using Gameplay.Events;
using Infrastructure;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    /// <summary>
    /// <c>SupplyPanel</c> 控制补给相关UI。
    /// 根据目前金币数和弹丸单价更新UI状态。
    /// 用户确认选择后执行回调。
    /// </summary>
    public class SupplyPanel : MonoBehaviour
    {
        public Image selectPanel;
        public TMP_Text maximumPurchase;
        public Image confirmPanel;
        public TMP_Text price;
        public Image warnPanel;

        public GameObject heroButton;
        public GameObject infantryButton;

        public int CurrentCoin { private get; set; }
        public int SinglePrice { private get; set; }
        public int MaxcanSupply { private get; set; }
        private int _currentSelection;

        private bool notNature; //人为增加金币
        public int StartCoin = 0;

        public delegate void ConfirmCallback(int number);

        private ConfirmCallback _callback;

        /// <summary>
        /// 面板默认隐藏。
        /// </summary>
        private void Start()
        {
            selectPanel.gameObject.SetActive(false);
        }

        /// <summary>
        /// 显示面板，开始新一次补给选择。
        /// </summary>
        /// <param name="callback">选择确认回调</param>
        /// <param name="identity"></param>
        public void StartSession(ConfirmCallback callback, Identity identity)
        {
            _callback = callback;
            warnPanel.gameObject.SetActive(false);
            confirmPanel.gameObject.SetActive(false);
            selectPanel.gameObject.SetActive(true);
            switch (identity.role)
            {
                case Identity.Roles.Hero:
                    heroButton.SetActive(true);
                    infantryButton.SetActive(false);
                    break;
                case Identity.Roles.Infantry:
                case Identity.Roles.BalanceInfantry:
                    heroButton.SetActive(false);
                    infantryButton.SetActive(true);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        /// <summary>
        /// 关闭面板。
        /// </summary>
        public void EndSession() => selectPanel.gameObject.SetActive(false);

        /// <summary>
        /// 根据金币数量和弹丸单价更新UI状态。
        /// </summary>
        private void Update()
        {
            if (SinglePrice == 0) return;
            var pricemax = CurrentCoin / SinglePrice;
            var maximum = pricemax > MaxcanSupply ? MaxcanSupply : pricemax;
            maximumPurchase.text = maximum.ToString();
            if (maximum < 5)
            {
                confirmPanel.gameObject.SetActive(false);
                warnPanel.gameObject.SetActive(true);
                maximumPurchase.color = Color.red;
            }
            else
            {
                warnPanel.gameObject.SetActive(false);
                maximumPurchase.color = Color.white;
                var currentPrice = _currentSelection * SinglePrice;
                price.text = currentPrice.ToString();
            }

            if (_currentSelection * SinglePrice > CurrentCoin)
            {
                confirmPanel.gameObject.SetActive(false);
            }
        }

        /// <summary>
        /// 点击选项回调。
        /// </summary>
        /// <param name="number">选择的弹丸数量</param>
        public void Select(int number)
        {
            _currentSelection = number;
            if (warnPanel.gameObject.activeSelf) return;
            if (!notNature && number > 1000)
            {
                /*  Dispatcher.Instance().Send(new CoinNaturalGrowth()
                  {
                      MinuteCoin = number-1000,
                      notNature = true    //服务器只允许人为增加一次
                  });*/
                StartCoin = number - 1000;
                notNature = true; //每个人最多只能点一次
                _callback(-StartCoin);
            }
            else StartCoin = 0;

            if (_currentSelection * SinglePrice <= CurrentCoin)
            {
                confirmPanel.gameObject.SetActive(true);
            }
        }

        /// <summary>
        /// “否”按钮回调，隐藏确认面板。
        /// </summary>
        public void Cancel() => confirmPanel.gameObject.SetActive(false);

        /// <summary>
        /// “是”按钮回调，隐藏确认面板之后调用回调。
        /// </summary>
        public void Confirm()
        {
            confirmPanel.gameObject.SetActive(false);
            _callback(_currentSelection);
        }
    }
}