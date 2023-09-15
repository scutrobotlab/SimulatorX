// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using Doozy.Runtime.Nody;
using Doozy.Runtime.Signals;
using Doozy.Runtime.UIManager.Nodes.Listeners.Internal;
using UnityEngine;
using UnityEngine.Events;

namespace Doozy.Runtime.UIManager.Nodes.Listeners
{
    public class StreamNodyListener : BaseNodyListener
    {
        private SignalPayload payload { get; }
        private UnityAction callback { get; }
        private SignalStream stream { get; set; }

        public StreamNodyListener(FlowNode node, SignalPayload payload, UnityAction callback) : base(node)
        {
            this.payload = payload;
            this.callback = callback;
        }

        protected override void ConnectReceiver()
        {
            stream = SignalStream.Get(payload.streamId.Category, payload.streamId.Name);
            stream.ConnectReceiver(receiver);
        }

        protected override void DisconnectReceiver()
        {
            stream.DisconnectReceiver(receiver);
        }

        protected override void ProcessSignal(Signal signal)
        {
            switch (payload.signalValueType)
            {
                case SignalPayload.ValueType.Integer:
                    if (signal.valueType == typeof(int))
                        if (signal.GetValueUnsafe<int>() == payload.integerValue)
                            callback?.Invoke();
                    break;
                case SignalPayload.ValueType.Boolean:
                    if (signal.valueType == typeof(bool))
                        if (signal.GetValueUnsafe<bool>() == payload.booleanValue)
                            callback?.Invoke();
                    break;
                case SignalPayload.ValueType.Float:
                    if (signal.valueType == typeof(float))
                        if (Mathf.Approximately(signal.GetValueUnsafe<float>(), payload.floatValue))
                            callback?.Invoke();
                    break;
                case SignalPayload.ValueType.String:
                    if (signal.valueType == typeof(string))
                        if (signal.GetValueUnsafe<string>() == payload.stringValue)
                            callback?.Invoke();
                    break;
                case SignalPayload.ValueType.Color:
                    if (signal.valueType == typeof(Color))
                        if (signal.GetValueUnsafe<Color>() == payload.colorValue)
                            callback?.Invoke();
                    break;
                case SignalPayload.ValueType.Vector2:
                    if (signal.valueType == typeof(Vector2))
                        if (signal.GetValueUnsafe<Vector2>() == payload.vector2Value)
                            callback?.Invoke();
                    break;
                case SignalPayload.ValueType.Vector3:
                    if (signal.valueType == typeof(Vector3))
                        if (signal.GetValueUnsafe<Vector3>() == payload.vector3Value)
                            callback?.Invoke();
                    break;
                case SignalPayload.ValueType.Vector4:
                    if (signal.valueType == typeof(Vector4))
                        if (signal.GetValueUnsafe<Vector4>() == payload.vector4Value)
                            callback?.Invoke();
                    break;
                
                case SignalPayload.ValueType.None:
                    // if (!signal.hasValue)
                    callback?.Invoke();
                    break;
                
                // ReSharper disable once RedundantEmptySwitchSection
                default:
                    //the signal value didn't match any of the types defined in the payload
                    break;
                
            }
        }
    }
}
