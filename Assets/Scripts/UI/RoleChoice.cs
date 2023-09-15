using System;
using Infrastructure;
using TMPro;
using UnityEngine;

namespace UI
{
    /// <summary>
    /// 角色选择按钮、指示灯、用户名文本。
    /// </summary>
    public class RoleChoice : MonoBehaviour
    {
        public Identity role;
        public TMP_Text nicknameText;
        
        protected RoleSelectionPanel SelectionPanel;

        protected virtual void Start()
        {
            SelectionPanel = FindObjectOfType<RoleSelectionPanel>();
        }

        public void SetNickname(string nickname, bool highlighted = false)
        {
            nicknameText.text = nickname;
            nicknameText.color = highlighted ? Color.white : new Color(1, 1, 1, 0.4f);
        }

        //普通点击事件
        public void OnButtonClicked()
        {
            // 选择角色
            SelectionPanel.ChooseRole(role);
        }
    }
}