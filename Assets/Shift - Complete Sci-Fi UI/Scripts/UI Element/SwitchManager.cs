using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Michsky.UI.Shift
{
    public class SwitchManager : MonoBehaviour
    {
        [Header("Settings")] [Tooltip("Each switch must have a different tag")]
        public string switchTag = "Switch";

        public bool isOn = true;
        public bool saveValue = true;
        public bool invokeAtStart = true;

        [Header("Events")] public UnityEvent OnEvents;

        public UnityEvent OffEvents;

        Animator switchAnimator;
        Button switchButton;

        private void Awake()
        {
            if (invokeAtStart == true && isOn == true)
            {
                OnEvents.Invoke();
            }

            if (invokeAtStart == true && isOn == false)
            {
                OffEvents.Invoke();
            }
        }

        private void OnEnable()
        {
            if (switchAnimator == null)
            {
                switchAnimator = gameObject.GetComponent<Animator>();
            }

            if (switchButton == null)
            {
                switchButton = gameObject.GetComponent<Button>();
                switchButton.onClick.AddListener(AnimateSwitch);
            }

            if (saveValue == true)
            {
                if (PlayerPrefs.GetString(switchTag + "Switch") == "")
                {
                    if (isOn == true)
                    {
                        switchAnimator.Play("Switch On");
                        isOn = true;
                        PlayerPrefs.SetString(switchTag + "Switch", "true");
                        PlayerPrefs.Save();
                    }

                    else
                    {
                        switchAnimator.Play("Switch Off");
                        isOn = false;
                        PlayerPrefs.SetString(switchTag + "Switch", "false");
                        PlayerPrefs.Save();
                    }
                }

                else if (PlayerPrefs.GetString(switchTag + "Switch") == "true")
                {
                    switchAnimator.Play("Switch On");
                    isOn = true;
                }

                else if (PlayerPrefs.GetString(switchTag + "Switch") == "false")
                {
                    switchAnimator.Play("Switch Off");
                    isOn = false;
                }
            }

            else
            {
                if (isOn == true)
                {
                    switchAnimator.Play("Switch On");
                    isOn = true;
                }
                else
                {
                    switchAnimator.Play("Switch Off");
                    isOn = false;
                }
            }
        }


        public void AnimateSwitch()
        {
            if (isOn == true)
            {
                switchAnimator.Play("Switch Off");
                isOn = false;
                OffEvents.Invoke();
                if (saveValue == true)
                {
                    PlayerPrefs.SetString(switchTag + "Switch", "false");
                    PlayerPrefs.Save();
                }
            }

            else
            {
                switchAnimator.Play("Switch On");
                isOn = true;
                OnEvents.Invoke();
                if (saveValue == true)
                {
                    PlayerPrefs.SetString(switchTag + "Switch", "true");
                    PlayerPrefs.Save();
                }
            }
        }
    }
}