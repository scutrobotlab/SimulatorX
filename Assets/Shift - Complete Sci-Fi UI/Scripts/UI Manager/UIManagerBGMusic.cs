using UnityEngine;

namespace Michsky.UI.Shift
{
    [ExecuteInEditMode]
    public class UIManagerBGMusic : MonoBehaviour
    {
        [Header("Resources")]
        public UIManager UIManagerAsset;
        public AudioSource audioObject;

        bool dynamicUpdateEnabled;

        void OnEnable()
        {
            if (UIManagerAsset == null)
            {
                try { UIManagerAsset = Resources.Load<UIManager>("Shift UI Manager"); }
                catch { Debug.Log("No UI Manager found. Assign it manually, otherwise it won't work properly.", this); }
            }
        }

        void Awake()
        {
            if (dynamicUpdateEnabled == false)
            {
                this.enabled = true;
                UpdateSource();
            }

            if (audioObject == null)
                audioObject = gameObject.GetComponent<AudioSource>();
        }

        void LateUpdate()
        {
            if (UIManagerAsset != null)
            {
                if (UIManagerAsset.enableDynamicUpdate == true)
                    dynamicUpdateEnabled = true;
                else
                    dynamicUpdateEnabled = false;

                if (dynamicUpdateEnabled == true)
                    UpdateSource();
            }
        }

        void UpdateSource()
        {
            try { audioObject.clip = UIManagerAsset.backgroundMusic; }
            catch { }
        }
    }
}