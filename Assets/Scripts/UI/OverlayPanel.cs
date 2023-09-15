using System;
using System.Collections;
using Controllers;
using Gameplay;
using Infrastructure;
using Mirror;
using TMPro;
using UnityEngine;

namespace UI
{
    public class OverlayPanel : MonoBehaviour
    {
        public TMP_Text pitchAngle;
        public TMP_Text baseUnderAttack;

        private void Start()
        {
            baseUnderAttack.color = new Color(0, 0, 0, 0);
        }

        private void FixedUpdate()
        {
            if (EntityManager.Instance() != null && EntityManager.Instance().initialized)
            {
                if (NetworkClient.active)
                {
                    var localRobot = EntityManager.Instance().LocalRobot();
                    if (localRobot == null) return;
                    float pitch;

                    switch (localRobot.role)
                    {
                        case Identity.Roles.Infantry:
                            var infantryController = (InfantryStore)EntityManager.Instance().Ref(localRobot);
                            pitch = infantryController.pitch.localEulerAngles.x;
                            if (pitch > 180) pitch -= 360;
                            pitchAngle.text = "Pitch: " + Math.Round(pitch, 2);
                            break;

                        case Identity.Roles.BalanceInfantry:
                            var balanceController = (BalancedInfantryStore)EntityManager.Instance().Ref(localRobot);
                            pitch = balanceController.pitch.localEulerAngles.x;
                            if (pitch > 180) pitch -= 360;
                            pitchAngle.text = "Pitch: " + Math.Round(pitch, 2);
                            break;

                        case Identity.Roles.Hero:
                            var heroController = (HeroStore)EntityManager.Instance().Ref(localRobot);
                            pitch = heroController.pitch.localEulerAngles.x;
                            if (pitch > 180) pitch -= 360;
                            pitchAngle.text = "Pitch: " + Math.Round(pitch, 2);
                            break;
                        default:
                            pitchAngle.text = "";
                            break;
                    }
                }
            }
        }

        public void BaseUnderAttack()
        {
            StartCoroutine(BaseUnderAttackBlink());
        }

        private IEnumerator BaseUnderAttackBlink()
        {
            for (var i = 0; i < 5; i++)
            {
                baseUnderAttack.color = Color.red;
                yield return new WaitForSeconds(0.2f);
                baseUnderAttack.color = new Color(0, 0, 0, 0);
                yield return new WaitForSeconds(0.2f);
            }
        }
    }
}