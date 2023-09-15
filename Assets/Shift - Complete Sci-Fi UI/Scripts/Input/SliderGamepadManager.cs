using UnityEngine;
using UnityEngine.UI;

namespace Michsky.UI.Shift
{
    public class SliderGamepadManager : MonoBehaviour
    {
        [Header("Slider")]
        public float changeValue = 0.5f;
        Slider sliderObject;

        [Header("Input")]
        public string horizontalAxis = "Xbox Right Stick Horizontal";

        void Start()
        {
            try { sliderObject = gameObject.GetComponent<Slider>(); }
            catch { Debug.LogWarning("Slider is missing. Sliding via gamepad won't work."); }
        }

        void Update()
        {
            if (sliderObject != null)
            {
                float h = Input.GetAxis(horizontalAxis);

                if (h == 1)
                    sliderObject.value += changeValue;
                else if (h == -1)
                    sliderObject.value -= changeValue;
            }
        }
    }
}