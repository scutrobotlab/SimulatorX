using UnityEngine;

namespace Michsky.UI.Shift
{
    public class ServerBrowserManager : MonoBehaviour
    {
        Animator mWindowAnimator;
        bool isOn = false;

        void Start()
        {
            mWindowAnimator = gameObject.GetComponent<Animator>();
        }

        public void ManageServerList()
        {
            if (isOn == false)
            {
                mWindowAnimator.CrossFade("List Minimize", 0.1f);
                isOn = true;
            }

            else if (isOn == true)
            {
                mWindowAnimator.CrossFade("List Expand", 0.1f);
                isOn = false;
            }
        }

        public void ExpandServerList()
        {
            if (isOn == true)
            {
                mWindowAnimator.CrossFade("List Expand", 0.1f);
                isOn = false;
            }
        }

        public void MinimizeServerList()
        {
            if (isOn == false)
            {
                mWindowAnimator.CrossFade("List Minimize", 0.1f);
                isOn = true;
            }
        }
    }
}