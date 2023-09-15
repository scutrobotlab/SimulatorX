// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using Doozy.Runtime.Common.Extensions;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Doozy.Runtime.Mody
{
    [Serializable]
    public class ModyActionRunner
    {
        /// <summary> Reference to the target Module </summary>
        public ModyModule Module;

        /// <summary> Name of the Action on the target Module </summary>
        public string ActionName;
        
        public bool BoolValue;
        public Color ColorValue;
        public double DoubleValue;
        public float FloatValue;
        public GameObject GameObjectValue;
        public int IntValue;
        public long LongValue;
        public MonoBehaviour MonoBehaviourValue;
        public Object GenericValue;
        public ScriptableObject ScriptableObjectValue;
        public Sprite SpriteValue;
        public string StringValue;
        public Texture TextureValue;
        public Texture2D Texture2DValue;
        public Vector2 Vector2Value;
        public Vector3 Vector3Value;
        public Vector4 Vector4Value;

        /// <summary> Operation to run </summary>
        public RunAction Run;

        /// <summary> If the Action is in cooldown, should it be ignored and run anyway </summary>
        public bool IgnoreCooldown;

        public ModyActionRunner()
        {
            Reset();
        }

        public void Reset()
        {
            Module = null;
            ActionName = string.Empty;
            Run = RunAction.Start;
            IgnoreCooldown = false;
            
            BoolValue = default;
            ColorValue = default;
            DoubleValue = default;
            FloatValue = default;
            GameObjectValue = default;
            GenericValue = default;
            IntValue = default;
            LongValue = default;
            MonoBehaviourValue = default;
            ScriptableObjectValue = default;
            SpriteValue = default;
            StringValue = default;
            Texture2DValue = default;
            TextureValue = default;
            Vector2Value = default;
            Vector3Value = default;
            Vector4Value = default;
        }

        /// <summary> Execute Operation </summary>
        /// <param name="debug"> Print relevant debug messages in the console </param>
        public void Execute(bool debug = false)
        {
            bool canExecute;
            string message;
            (canExecute, message) = CanExecute();
            if (debug) Debug.Log(message);
            if (!canExecute) return;
            ModyAction action = Module.GetAction(ActionName);
            bool reactToAnySignal = action.ReactToAnySignal;
            action.ReactToAnySignal = true;
            bool ignoreSignalValue = action.IgnoreSignalValue;
            action.IgnoreSignalValue = true;
            if (action.HasValue)
            {
                if (action.ValueType == typeof(int)) action.SetValue(IntValue);
                else if (action.ValueType == typeof(float)) action.SetValue(FloatValue);
                else if (action.ValueType == typeof(double)) action.SetValue(DoubleValue);
                else if (action.ValueType == typeof(long)) action.SetValue(LongValue);
                else if (action.ValueType == typeof(string)) action.SetValue(StringValue);
                else if (action.ValueType == typeof(bool)) action.SetValue(BoolValue);
                else if (action.ValueType == typeof(Color) || action.ValueType == typeof(Color32)) action.SetValue(ColorValue);
                else if (action.ValueType == typeof(Vector2)) action.SetValue(Vector2Value);
                else if (action.ValueType == typeof(Vector3)) action.SetValue(Vector3Value);
                else if (action.ValueType == typeof(Vector4)) action.SetValue(Vector4Value);
                else if (action.ValueType == typeof(GameObject)) action.SetValue(GameObjectValue);
                else if (action.ValueType == typeof(MonoBehaviour)) action.SetValue(MonoBehaviourValue);
                else if (action.ValueType == typeof(Sprite)) action.SetValue(SpriteValue);
                else if (action.ValueType == typeof(Texture)) action.SetValue(TextureValue);
                else if (action.ValueType == typeof(Texture2D)) action.SetValue(Texture2DValue);
                else if (action.ValueType == typeof(ScriptableObject)) action.SetValue(ScriptableObjectValue);
                else action.SetValue(GenericValue);
            }
            action.ExecuteMethod(Run, IgnoreCooldown, true);
            action.ReactToAnySignal = reactToAnySignal;
            action.IgnoreSignalValue = ignoreSignalValue;
        }

        /// <summary> Check if this action runner has the proper settings to execute </summary>
        public (bool, string) CanExecute() =>
            Module == null
                ? (false, $"{nameof(Module)} reference is null! Cannot Execute!")
                : ActionName.IsNullOrEmpty()
                    ? (false, $"{nameof(ActionName)} cannot be null or empty! Cannot Execute!")
                    : !Module.ContainsAction(ActionName)
                        ? (false, $"The Module {Module.moduleName} does not contain an Action named {ActionName}. Cannot Execute!")
                        : (true, $"Success! Action {ActionName} can be executed!");
    }
}
