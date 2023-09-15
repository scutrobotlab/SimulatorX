using UnityEngine;

namespace Michsky.UI.Shift
{
    [ExecuteInEditMode]
    public class UIManagerParticle : MonoBehaviour
    {
        [Header("Resources")]
        public UIManager UIManagerAsset;
        public ParticleSystem particleObject;

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
                UpdateColor();
            }

            if (particleObject == null)
                particleObject = gameObject.GetComponent<ParticleSystem>();
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
                    UpdateColor();
            }
        }

        void UpdateColor()
        {
            try
            {
                var main = particleObject.main;
                main.startColor = new Color(UIManagerAsset.particleColor.r, UIManagerAsset.particleColor.g, UIManagerAsset.particleColor.b, main.startColor.color.a);
            }

            catch { }
        }
    }
}