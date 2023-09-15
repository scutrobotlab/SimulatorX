// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using System.Collections.Generic;
using Doozy.Runtime.Common.Attributes;
using Doozy.Runtime.Signals;
using UnityEngine;
// ReSharper disable MemberCanBePrivate.Global

namespace Doozy.Runtime.UIManager.Input
{
    public static class InputStream
    {
        public const string k_StreamCategory = "Input";
        public const string k_StreamName = nameof(InputStream);

        [ClearOnReload]
        private static SignalStream s_stream;
        public static SignalStream stream => s_stream ??= SignalsService.GetStream(k_StreamCategory, k_StreamName);

        public const string k_NavigateStreamCategory = "Navigate";
        public const string k_NavigateLeft = "Left";
        public const string k_NavigateRight = "Right";
        public const string k_NavigateUp = "Up";
        public const string k_NavigateDown = "Down";

        [ClearOnReload]
        private static SignalStream s_navigateLeftStream;
        public static SignalStream navigateLeftStream => s_navigateLeftStream ??= SignalsService.GetStream(k_NavigateStreamCategory, k_NavigateLeft);

        [ClearOnReload]
        private static SignalStream s_navigateRightStream;
        public static SignalStream navigateRightStream => s_navigateRightStream ??= SignalsService.GetStream(k_NavigateStreamCategory, k_NavigateRight);

        [ClearOnReload]
        private static SignalStream s_navigateUpStream;
        public static SignalStream navigateUpStream => s_navigateUpStream ??= SignalsService.GetStream(k_NavigateStreamCategory, k_NavigateUp);

        [ClearOnReload]
        private static SignalStream s_navigateDownStream;
        public static SignalStream navigateDownStream => s_navigateDownStream ??= SignalsService.GetStream(k_NavigateStreamCategory, k_NavigateDown);

        public const string k_CustomInputActionStreamCategory = "CustomInputAction";
        [ClearOnReload]
        private static Dictionary<string, SignalStream> s_customInputActionSignalStreams;
        private static Dictionary<string, SignalStream> customInputActionSignalStreams => s_customInputActionSignalStreams ??= new Dictionary<string, SignalStream>();

        [ClearOnReload]
        private static SignalReceiver inputStreamReceiver { get; set; }

        private static void ConnectToInputStream()
        {
            stream.ConnectReceiver(inputStreamReceiver);
            connected = true;
        }
        private static void DisconnectFromInputStream()
        {
            stream.DisconnectReceiver(inputStreamReceiver);
            connected = false;
        }

        private static bool connected { get; set; } = false;

        public static void Start()
        {
            if (connected)
                return;

            inputStreamReceiver = new SignalReceiver();

            inputStreamReceiver.SetOnSignalCallback(signal =>
            {
                if (!signal.hasValue) return;
                if (!(signal.valueAsObject is InputSignalData data)) return;
                #if INPUT_SYSTEM_PACKAGE
                switch (data.inputActionName)
                {
                    case UIInputActionName.Point:
                        break;
                    case UIInputActionName.Click:
                        break;
                    case UIInputActionName.MiddleClick:
                        break;
                    case UIInputActionName.RightClick:
                        break;
                    case UIInputActionName.ScrollWheel:
                        break;
                    case UIInputActionName.Navigate:
                        Navigate(data);
                        break;
                    case UIInputActionName.Submit:
                        break;
                    case UIInputActionName.Cancel:
                        break;
                    case UIInputActionName.TrackedDevicePosition:
                        break;
                    case UIInputActionName.TrackedDeviceOrientation:
                        break;
                    case UIInputActionName.CustomActionName:
                        CustomInputAction(data);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
                #elif LEGACY_INPUT_MANAGER
                if (data.inputMode == LegacyInputMode.None) return;
                switch (data.inputMode)
                {
                    case LegacyInputMode.None:
                        return;
                    case LegacyInputMode.KeyCode:
                        switch (data.keyCode)
                        {
                            case KeyCode.LeftArrow:
                            case KeyCode.UpArrow:
                            case KeyCode.RightArrow:
                            case KeyCode.DownArrow:
                                Navigate(data);
                                break;
                        }

                        break;
                    case LegacyInputMode.VirtualButton:
                        switch (data.virtualButtonName)
                        {
                            case "Left":
                            case "Up":
                            case "Right":
                            case "Down":
                                Navigate(data);
                                break;
                        }
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                #endif
            });

            ConnectToInputStream();
        }

        public static void Stop()
        {
            if (!connected)
                return;
            DisconnectFromInputStream();
            inputStreamReceiver = null;
        }

        private static void Navigate(InputSignalData data)
        {
            #if INPUT_SYSTEM_PACKAGE
            Vector2 direction = data.callbackContext.ReadValue<Vector2>();

            if (direction.x < 0)
            {
                navigateLeftStream.SendSignal(data);
                return;
            }

            if (direction.x > 0)
            {
                navigateRightStream.SendSignal(data);
                return;
            }

            if (direction.y < 0)
            {
                navigateDownStream.SendSignal(data);
                return;
            }

            if (direction.y > 0)
            {
                navigateUpStream.SendSignal(data);
                return;
            }
            #endif

            #if LEGACY_INPUT_MANAGER
            switch (data.inputMode)
            {
                case LegacyInputMode.None:
                    return;
                case LegacyInputMode.KeyCode:
                    switch (data.keyCode)
                    {
                        case KeyCode.LeftArrow:
                            navigateLeftStream.SendSignal(data);
                            break;
                        case KeyCode.UpArrow:
                            navigateUpStream.SendSignal(data);
                            break;
                        case KeyCode.RightArrow:
                            navigateRightStream.SendSignal(data);
                            break;
                        case KeyCode.DownArrow:
                            navigateDownStream.SendSignal(data);
                            break;
                    }

                    break;
                case LegacyInputMode.VirtualButton:
                    switch (data.virtualButtonName)
                    {
                        case "Left":
                            navigateLeftStream.SendSignal(data);
                            break;
                        case "Up":
                            navigateUpStream.SendSignal(data);
                            break;
                        case "Right":
                            navigateRightStream.SendSignal(data);
                            break;
                        case "Down":
                            navigateDownStream.SendSignal(data);
                            break;
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            #endif
        }

        private static void CustomInputAction(InputSignalData data)
        {
            #if INPUT_SYSTEM_PACKAGE
            string streamName = data.callbackContext.action.name;
            if (!customInputActionSignalStreams.TryGetValue(streamName, out SignalStream signalStream))
            {
                signalStream = SignalsService.GetStream(k_CustomInputActionStreamCategory, streamName);
                customInputActionSignalStreams.Add(streamName, signalStream);
            }

            signalStream.SendSignal(data);
            #endif
        }
    }
}
