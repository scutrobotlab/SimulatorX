using System;
using System.Collections;
using AdditionalAssets.RobotArm.Scripts;
using Controllers;
using Gameplay.Customize;
using Infrastructure;
using TMPro;
using UnityEngine;

namespace Gameplay.Networking
{
    public class GradeSelection : MonoBehaviour
    {
        public TMP_Dropdown selectionDropdown;
        public Grade selectedGrade;
        public Identity player;
        public ExchangeStore exchanger;

        private IEnumerator Start()
        {
            yield return new WaitUntil(() => EntityManager.Instance()?.initialized == true);
            selectionDropdown = GetComponentInChildren<TMP_Dropdown>();
            player = EntityManager.Instance().LocalRobot();
            foreach (var store in FindObjectsOfType<ExchangeStore>())
            {
                if (store.id.camp == player.camp)
                {
                    exchanger = store;
                    break;
                }
            }
        }

        public void ChangeGrade()
        {
            //防喂初始化
            if (selectionDropdown == null || player == null|| exchanger == null)
            {
                Start();
            }
            
            //防其他机器人捣乱
            if (player.role != Identity.Roles.Engineer)
            {
                Debug.Log("grade return");
                return;
            }

            selectedGrade = selectionDropdown.value switch
            {
                0 => Grade.Zero,
                1 => Grade.One,
                2 => Grade.Two,
                3 => Grade.Three,
                4 => Grade.Four,
                _ => throw new ArgumentOutOfRangeException()
            };
            
            //兑换限制
            if (exchanger.accumulatedCoin > 1625  && selectionDropdown.value <= 3 )
                selectedGrade = Grade.Four;
            else if (exchanger.accumulatedCoin > 1100 && selectionDropdown.value <= 2)
                selectedGrade = Grade.Three;
            else if (exchanger.accumulatedCoin > 750 && selectionDropdown.value <= 1)
                selectedGrade = Grade.Two;
            else if (exchanger.accumulatedCoin > 575 &&selectedGrade ==  Grade.Zero)
                selectedGrade = Grade.One;


            // exchanger.currentState.currentState = selectedGrade;
            
            CustomizeManager.Instance().CmdChangeGrade(exchanger.id,selectedGrade);
        }

    }
}
