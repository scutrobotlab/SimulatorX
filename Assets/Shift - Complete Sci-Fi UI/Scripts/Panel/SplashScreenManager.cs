using UnityEngine;

namespace Michsky.UI.Shift
{
    public class SplashScreenManager : MonoBehaviour
    {
        private static bool _isFirstTime = true;

        [Header("Resources")] public GameObject splashScreen;

        public GameObject mainPanels;

        [Header("Settings")] public bool disableSplashScreen;

        public bool enablePressAnyKeyScreen;
        public bool enableLoginScreen;
        private Animator mainPanelsAnimator;

        MainPanelManager mpm;

        private Animator splashScreenAnimator;
        private TimedEvent ssTimedEvent;

        void OnEnable()
        {
            if (splashScreenAnimator == null)
            {
                splashScreenAnimator = splashScreen.GetComponent<Animator>();
            }

            if (ssTimedEvent == null)
            {
                ssTimedEvent = splashScreen.GetComponent<TimedEvent>();
            }

            if (mainPanelsAnimator == null)
            {
                mainPanelsAnimator = mainPanels.GetComponent<Animator>();
            }

            if (mpm == null)
            {
                mpm = gameObject.GetComponent<MainPanelManager>();
            }

            if (disableSplashScreen == true || _isFirstTime == false)
            {
                splashScreen.SetActive(false);
                mainPanels.SetActive(true);

                mainPanelsAnimator.Play("Start");
                mpm.OpenFirstTab();
                return;
            }

            if (enableLoginScreen == false && enablePressAnyKeyScreen == true && disableSplashScreen == false)
            {
                splashScreen.SetActive(true);
                mainPanelsAnimator.Play("Invisible");
            }

            if (enableLoginScreen == true && enablePressAnyKeyScreen == true && disableSplashScreen == false)
            {
                splashScreen.SetActive(true);
                mainPanelsAnimator.Play("Invisible");
            }

            if (enableLoginScreen == true && enablePressAnyKeyScreen == false && disableSplashScreen == false)
            {
                splashScreen.SetActive(true);
                mainPanelsAnimator.Play("Invisible");
                splashScreenAnimator.Play("Login");
            }

            if (enableLoginScreen == false && enablePressAnyKeyScreen == false && disableSplashScreen == false)
            {
                splashScreen.SetActive(true);
                mainPanelsAnimator.Play("Invisible");
                splashScreenAnimator.Play("Loading");
                ssTimedEvent.StartIEnumerator();
            }

            _isFirstTime = false;
        }

        public void LoginScreenCheck()
        {
            if (enableLoginScreen == true && enablePressAnyKeyScreen == true)
                splashScreenAnimator.Play("Press Any Key to Login");

            else if (enableLoginScreen == false && enablePressAnyKeyScreen == true)
            {
                splashScreenAnimator.Play("Press Any Key to Loading");
                ssTimedEvent.StartIEnumerator();
            }

            else if (enableLoginScreen == false && enablePressAnyKeyScreen == false)
            {
                splashScreenAnimator.Play("Loading");
                ssTimedEvent.StartIEnumerator();
            }
        }
    }
}