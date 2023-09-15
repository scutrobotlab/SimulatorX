// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using System.Reflection;
using Doozy.Editor.EditorUI;
using Doozy.Editor.Reactor.Internal;
using Doozy.Runtime.Nody;
using Doozy.Runtime.Reactor.Internal;
using Doozy.Runtime.Reactor.Reactions;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;
// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable MemberCanBePrivate.Global

namespace Doozy.Editor.Nody
{
    public class FlowEdgeView : Edge, IDisposable
    {
        public void Dispose()
        {
            inputColorReaction?.Recycle(); 
            outputColorReaction?.Recycle();
        }
        
        private FlowPortView m_OutputPortView;
        public FlowPortView outputPortView
        {
            get => m_OutputPortView;
            internal set
            {
                m_OutputPortView = value;
                m_OutputPortView.flowPort.ping += flowDirection =>
                {
                    outputColorReaction.Stop();
                    inputColorReaction.Stop();

                    switch (flowDirection)
                    {
                        case FlowDirection.Forward: //forward flow 
                            outputColorReaction.SetDuration(0.4f).SetStartDelay(0.2f);
                            outputColorReaction.SetFrom(EditorColors.Nody.StateActive);
                            outputColorReaction.SetValue(EditorColors.Nody.StateActive);
                            inputColorReaction.SetDuration(0.4f).SetStartDelay(0.4f);
                            inputColorReaction.SetFrom(EditorColors.Nody.StateActive);
                            inputColorReaction.SetValue(EditorColors.Nody.StateActive);
                            break;

                        case FlowDirection.Back: //back flow
                            outputColorReaction.SetDuration(0.4f).SetStartDelay(0.4f);
                            outputColorReaction.SetFrom(EditorColors.Nody.BackFlow);
                            outputColorReaction.SetValue(EditorColors.Nody.BackFlow);
                            inputColorReaction.SetDuration(0.4f).SetStartDelay(0.2f);
                            inputColorReaction.SetFrom(EditorColors.Nody.BackFlow);
                            inputColorReaction.SetValue(EditorColors.Nody.BackFlow);
                            break;
                        default:
                            throw new ArgumentOutOfRangeException(nameof(flowDirection), flowDirection, null);
                    }

                    inputColorReaction.SetTo(inputColor);
                    outputColorReaction.SetTo(outputColor);

                    inputColorReaction.Play();
                    outputColorReaction.Play();
                };
            }
        }

        private FlowPortView m_InputPortView;
        public FlowPortView inputPortView
        {
            get => m_InputPortView;
            internal set => m_InputPortView = value;
        }

        public ColorReaction outputColorReaction { get; internal set; }
        public ColorReaction inputColorReaction { get; internal set; }

        public static Color outputColor => EditorApplication.isPlayingOrWillChangePlaymode ? EditorColors.Nody.StateIdle : EditorColors.Nody.Output;
        public static Color inputColor => EditorApplication.isPlayingOrWillChangePlaymode ? EditorColors.Nody.StateIdle : EditorColors.Nody.Input;

        public FlowEdgeView()
        {
            outputColorReaction =
                Reaction.Get<ColorReaction>()
                    .SetEditorHeartbeat()
                    .SetDuration(0.4f)
                    .SetStartDelay(0.2f)
                    .SetSetter(value =>
                    {
                        if (edgeControl != null) edgeControl.outputColor = value;
                        if (outputPortView != null) outputPortView.portColor = value;
                    })
                    .SetOnPlayCallback(() =>
                    {       
                        edgeControl.edgeWidth = 3;
                    })
                    .SetOnStopCallback(() =>
                    {
                        edgeControl.edgeWidth = 2;
                        outputColorReaction?.SetTo(outputColor);
                    });

            inputColorReaction =
                Reaction.Get<ColorReaction>()
                    .SetEditorHeartbeat()
                    .SetDuration(0.4f)
                    .SetStartDelay(0.4f)
                    .SetOnStopCallback(() =>
                    {
                        inputColorReaction?.SetTo(inputColor);
                    })
                    .SetSetter(value =>
                    {
                        if (edgeControl != null) edgeControl.inputColor = value;
                        // inputPortView.portColor = value;
                    });


            RegisterCallback<CustomStyleResolvedEvent>(evt =>
            {
                typeof(Edge).GetField("m_SelectedColor", BindingFlags.NonPublic | BindingFlags.Instance)?
                    .SetValue(this, EditorColors.Nody.Selection);
            });
        }

        public FlowEdgeView SetOutputPortView(FlowPortView portView)
        {
            outputPortView = portView;
            return this;
        }

        public FlowEdgeView SetInputPortView(FlowPortView portView)
        {
            inputPortView = portView;
            return this;
        }
        
    }
}
