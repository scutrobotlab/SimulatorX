using Gameplay;
using Gameplay.Attribute;
using Honeti;
using Infrastructure;
using Mirror;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace UI
{
    /// <summary>
    /// 机构类型选择页面。
    /// </summary>
    public class MechanicSelectionPanel : MonoBehaviour
    {
        public delegate void ChoiceChangeCallback(MechanicType.Chassis chassis, MechanicType.GunType gun);

        public delegate void SensitivityChangeCallback(float sensitivity);

        public Image panel;
        public TMP_Dropdown chassisType;
        public TMP_Dropdown gunType;
        public Slider sensitivity;
        private bool _canSelectCoolDown;

        private ChoiceChangeCallback _choiceChangeCallback;
        private SensitivityChangeCallback _sensitivityChangeCallback;

        public bool selecting => panel.gameObject.activeSelf;

        private void Start()
        {
            EndSession();
        }

        public void EndSession()
        {
            panel.gameObject.SetActive(false);
        }

        public void StartSession(ChoiceChangeCallback callback, SensitivityChangeCallback sensitivityChangeCallback,
            bool canSelectCoolDown = true)
        {
            chassisType.options.Clear();
            foreach (var typeName in MechanicType.ChassisTypeName)
            {
                chassisType.options.Add(new TMP_Dropdown.OptionData(I18N.instance.getValue(typeName)));
            }

            gunType.options.Clear();
            foreach (var typeName in MechanicType.GunTypeNames)
            {
                gunType.options.Add(new TMP_Dropdown.OptionData(I18N.instance.getValue(typeName)));
            }

            _choiceChangeCallback = callback;
            _sensitivityChangeCallback = sensitivityChangeCallback;
            _canSelectCoolDown = canSelectCoolDown;
            panel.gameObject.SetActive(true);
        }

        public void OnSensitivityChange()
        {
            _sensitivityChangeCallback?.Invoke(sensitivity.value);
        }

        public void OnChoiceChanged()
        {
            if (_choiceChangeCallback != null)
            {
                var chassisChoice = chassisType.value switch
                {
                    1 => MechanicType.Chassis.Power,
                    2 => MechanicType.Chassis.Armor,
                    3 => MechanicType.Chassis.Balance,
                    _ => MechanicType.Chassis.Default
                };


                // 不能选择冷却优先的情况
                if (gunType.value == 2 && !_canSelectCoolDown)
                {
                    gunType.value = 0;
                    return;
                }

                var gunChoice = gunType.value switch
                {
                    1 => MechanicType.GunType.Burst,
                    2 => MechanicType.GunType.CoolDown,
                    3 => MechanicType.GunType.Velocity,
                    _ => MechanicType.GunType.Default
                };

                //锁定选择
                if (chassisChoice != MechanicType.Chassis.Default && gunChoice != MechanicType.GunType.Default)
                {
                    //英雄不可选择弹速优先以及平衡底盘
                    if (EntityManager.Instance().initialized &&
                        EntityManager.Instance().LocalRobot().role == Identity.Roles.Hero)
                    {
                        if (gunChoice == MechanicType.GunType.Velocity || chassisChoice == MechanicType.Chassis.Balance)
                        {
                            chassisType.enabled = true;
                            gunType.enabled = true;
                        }
                    }
                    else
                    {
                        chassisType.enabled = false;
                        gunType.enabled = false;
                    }

                    _choiceChangeCallback(chassisChoice, gunChoice);
                }
            }
        }

        public void OnLogoutClicked()
        {
            if (NetworkClient.active)
                NetworkManager.singleton.StopClient();
            if (NetworkServer.active)
                NetworkManager.singleton.StopServer();
            SceneManager.LoadScene("Offline");
        }
    }
}