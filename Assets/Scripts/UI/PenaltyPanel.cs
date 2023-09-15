using Honeti;
using Infrastructure;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    /// <summary>
    /// 裁判判罚面板。 
    /// </summary>
    public class PenaltyPanel : MonoBehaviour
    {
        public delegate void GameOverCallback(Identity.Camps winner, string description);

        public delegate void PenaltyCallback(Identity robot, string description);

        public Image panel;
        public TMP_Text selectedRobotName;
        private PenaltyCallback _ejectedCallback;
        private GameOverCallback _gameOverCallback;
        private PenaltyCallback _penaltyCallback;

        private Identity _selectedRobot = new Identity();

        private void Start()
        {
            EndSession();
        }

        /// <summary>
        /// 显示面板。
        /// </summary>
        /// <param name="chosenRobot">选中的机器人</param>
        /// <param name="ejectedCallback">罚下回调</param>
        /// <param name="penaltyCallback">执行判罚的回调</param>
        /// <param name="gameOverCallback">游戏结束的回调</param>
        public void StartSession(
            Identity chosenRobot,
            PenaltyCallback ejectedCallback,
            PenaltyCallback penaltyCallback,
            GameOverCallback gameOverCallback)
        {
            _selectedRobot = chosenRobot;
            _ejectedCallback = ejectedCallback;
            _penaltyCallback = penaltyCallback;
            _gameOverCallback = gameOverCallback;
            panel.gameObject.SetActive(true);
            selectedRobotName.text = _selectedRobot.Describe();
        }

        public void OnPenaltyForBumping()
        {
            if (_selectedRobot == new Identity()) return;
            _penaltyCallback?.Invoke(_selectedRobot, I18N.instance.getValue("^collision"));
        }

        public void OnPenaltyForRestrictedArea()
        {
            if (_selectedRobot == new Identity()) return;
            _penaltyCallback?.Invoke(_selectedRobot, I18N.instance.getValue("^restricted_zone"));
        }

        public void OnPenaltyForDisturbance()
        {
            if (_selectedRobot == new Identity()) return;
            _penaltyCallback?.Invoke(_selectedRobot, I18N.instance.getValue("^interfere_with_the_resurrection"));
        }

        public void OnRobotEjected()
        {
            if (_selectedRobot == new Identity()) return;
            _ejectedCallback?.Invoke(_selectedRobot, I18N.instance.getValue("^red_send_off"));
        }

        public void OnCampAwardNegative()
        {
            if (_selectedRobot == new Identity()) return;
            _gameOverCallback?.Invoke(
                _selectedRobot.camp switch
                {
                    Identity.Camps.Red => Identity.Camps.Blue,
                    Identity.Camps.Blue => Identity.Camps.Red,
                    _ => Identity.Camps.Other
                },
                I18N.instance.getValue("^determine_failure")
            );
        }

        public void OnStopGame()
        {
            _gameOverCallback?.Invoke(Identity.Camps.Other, I18N.instance.getValue("^end_game"));
        }

        public void EndSession()
        {
            panel.gameObject.SetActive(false);
        }
    }
}