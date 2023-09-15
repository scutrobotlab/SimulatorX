using Misc;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Lab
{
    /// <summary>
    /// <c>CameraControl</c> 用于控制摄像机切换。
    /// <br/>此工具类只用于调试用途。
    /// </summary>
    public class CameraControl : MonoBehaviour
    {
        public GameObject fireCamera;
        public GameObject followCamera;

        private ToggleHelper _cameraHelper;

        /// <summary>
        /// 初始化相机状态。
        /// </summary>
        private void Start()
        {
            _cameraHelper = new ToggleHelper();
            fireCamera.SetActive(true);
            followCamera.SetActive(false);
        }

        /// <summary>
        /// 检测到按下Z键就翻转摄像机状态。
        /// </summary>
        private void FixedUpdate()
        {
            if (_cameraHelper.Toggle(Keyboard.current.zKey.isPressed) != ToggleHelper.State.Re) return;
            fireCamera.SetActive(!fireCamera.activeSelf);
            followCamera.SetActive(!followCamera.activeSelf);
        }
    }
}