using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Infrastructure.Input
{
    /// <summary>
    /// <c>InputManager</c> 用于管理用户输入。
    /// <br/>其从 InputSystem 接收输入信息并映射为状态。
    /// <br/>这个单例只！能！在！客！户！端！使！用！
    /// </summary>
    public class InputManager : Singleton<InputManager>
    {
        // 功能键映射方式
        public static readonly Dictionary<string, InputButtonConfig> ButtonControlPatterns =
            new Dictionary<string, InputButtonConfig>
            {
                { InputActionID.Fire, InputButtonConfig.Push },
                { InputActionID.SecondaryFire, InputButtonConfig.Push },
                { InputActionID.FunctionA, InputButtonConfig.Toggle },
                { InputActionID.FunctionB, InputButtonConfig.Toggle },
                { InputActionID.FunctionC, InputButtonConfig.Toggle },
                { InputActionID.FunctionD, InputButtonConfig.Toggle },
                { InputActionID.FunctionE, InputButtonConfig.Toggle },
                { InputActionID.FunctionF, InputButtonConfig.Toggle },
                { InputActionID.FunctionG, InputButtonConfig.Toggle },
                { InputActionID.FunctionH, InputButtonConfig.Toggle },
                { InputActionID.FunctionI, InputButtonConfig.Toggle },
                { InputActionID.FunctionJ, InputButtonConfig.Toggle },
                { InputActionID.FunctionK, InputButtonConfig.Toggle },
                { InputActionID.FunctionL, InputButtonConfig.Toggle },
                { InputActionID.FunctionM, InputButtonConfig.Toggle },
                { InputActionID.FunctionN, InputButtonConfig.Toggle },
            };

        // 功能状态初始值
        public readonly Dictionary<string, bool> ButtonStatus = new Dictionary<string, bool>
        {
            { InputActionID.Fire, false },
            { InputActionID.SecondaryFire, false },
            { InputActionID.FunctionA, false },
            { InputActionID.FunctionB, false },
            { InputActionID.FunctionC, false },
            { InputActionID.FunctionD, false },
            { InputActionID.FunctionE, false },
            { InputActionID.FunctionF, false },
            { InputActionID.FunctionG, false },
            { InputActionID.FunctionH, false },
            { InputActionID.FunctionI, false },
            { InputActionID.FunctionJ, false },
            { InputActionID.FunctionK, false },
            { InputActionID.FunctionL, false },
            { InputActionID.FunctionM, false },
            { InputActionID.FunctionN, false }
        };

        private Vector2 _primaryAxisCache;
        private Vector2 _primaryAxisGravity;
        private Vector2 _secondaryAxisCache;
        private Vector2 _secondaryAxisGravity;

        private Vector2 _viewCache;
        public Vector2 primaryAxis { get; private set; }

        public Vector2 secondaryAxis { get; private set; }

        /// <summary>
        /// 每帧对数字轴向输入进行处理。
        /// </summary>
        private void FixedUpdate()
        {
            primaryAxis = ApplyGravity(ref _primaryAxisGravity, _primaryAxisCache);
            secondaryAxis = ApplyGravity(ref _secondaryAxisGravity, _secondaryAxisCache);
        }

        /// <summary>
        /// InputSystem 轴向回调函数。
        /// </summary>
        /// <param name="context"></param>
        public void AxisCb(InputAction.CallbackContext context)
        {
            var axis = context.ReadValue<Vector2>();
            switch (context.action.name)
            {
                case InputActionID.PrimaryAxis:
                    _primaryAxisCache = axis;
                    break;
                case InputActionID.SecondaryAxis:
                    _secondaryAxisCache = axis;
                    break;
            }
        }

        /// <summary>
        /// InputSystem 视角控制回调函数。
        /// </summary>
        /// <param name="context"></param>
        public void ViewCb(InputAction.CallbackContext context)
        {
            _viewCache = context.ReadValue<Vector2>();
        }

        /// <summary>
        /// InputSystem 功能键回调函数。
        /// </summary>
        /// <param name="context"></param>
        public void ButtonCb(InputAction.CallbackContext context)
        {
            var buttonName = context.action.name;
            if (!ButtonControlPatterns.ContainsKey(buttonName)) return;
            switch (ButtonControlPatterns[buttonName])
            {
                case InputButtonConfig.Push:
                    ButtonStatus[buttonName] = context.ReadValue<float>() != 0;
                    break;
                case InputButtonConfig.Toggle:
                    if (context.ReadValue<float>() != 0)
                    {
                        if (!context.performed) return;
                        ButtonStatus[buttonName] = !ButtonStatus[buttonName];
                    }

                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        /// <summary>
        /// 获取视角控制轴原始数据。
        /// </summary>
        /// <returns>视角控制数据</returns>
        public Vector2 RawView() => _viewCache;

        /// <summary>
        /// 对两轴数字输入进行模拟插值处理（JoyStick Gravity）。
        /// </summary>
        /// <param name="accumulator">积分器</param>
        /// <param name="delta">输入数据</param>
        /// <param name="gravity">衰减系数</param>
        /// <param name="maximum">积分上限</param>
        /// <returns>模拟输出</returns>
        private static Vector2 ApplyGravity(
            ref Vector2 accumulator,
            Vector2 delta,
            float gravity = 0.6f,
            float maximum = 5.0f)
        {
            accumulator += delta;
            var maxAbs = maximum + gravity;
            if (Mathf.Abs(accumulator.x) > maxAbs)
                accumulator.x = accumulator.x > 0 ? maxAbs : -maxAbs;
            if (Mathf.Abs(accumulator.y) > maxAbs)
                accumulator.y = accumulator.y > 0 ? maxAbs : -maxAbs;
            if (gravity <= 0) Debug.Log("Invalid gravity value.");
            else
            {
                if (Mathf.Abs(accumulator.x) < gravity) accumulator.x = 0;
                else accumulator.x += accumulator.x > 0 ? -gravity : gravity;
                if (Mathf.Abs(accumulator.y) < gravity) accumulator.y = 0;
                else accumulator.y += accumulator.y > 0 ? -gravity : gravity;
            }

            return accumulator / maximum;
        }
    }
}