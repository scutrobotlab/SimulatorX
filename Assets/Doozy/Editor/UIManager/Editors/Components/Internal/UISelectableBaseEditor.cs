// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System.Collections.Generic;
using System.Linq;
using Doozy.Editor.EditorUI;
using Doozy.Editor.EditorUI.Components;
using Doozy.Editor.EditorUI.Components.Internal;
using Doozy.Editor.EditorUI.ScriptableObjects.Colors;
using Doozy.Editor.EditorUI.Utils;
using Doozy.Runtime.UIElements.Extensions;
using Doozy.Runtime.UIManager;
using Doozy.Runtime.UIManager.Components;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable MemberCanBeProtected.Global

namespace Doozy.Editor.UIManager.Editors.Components.Internal
{
    public abstract class UISelectableBaseEditor : UnityEditor.Editor
    {
        protected const string k_ShowNavigationKey = "SelectableEditor.ShowNavigation";
        protected static bool showNavigation { get; set; }

        protected static IEnumerable<Texture2D> navigationIconTextures => EditorMicroAnimations.EditorUI.Icons.Navigation;
        protected static IEnumerable<Texture2D> statesIconTextures => EditorMicroAnimations.EditorUI.Icons.SelectableStates;
        protected static IEnumerable<Texture2D> hideIconTextures => EditorMicroAnimations.EditorUI.Icons.Hide;
        protected static IEnumerable<Texture2D> showIconTextures => EditorMicroAnimations.EditorUI.Icons.Show;
        protected static IEnumerable<Texture2D> settingsIconTextures => EditorMicroAnimations.EditorUI.Icons.Settings;

        public virtual Color accentColor => EditorColors.UIManager.UIComponent;
        public virtual EditorSelectableColorInfo selectableAccentColor => EditorSelectableColors.UIManager.UIComponent;

        public UISelectable selectable => (UISelectable)target;
        public IEnumerable<UISelectable> selectables => targets.Cast<UISelectable>();

        protected VisualElement root { get; set; }

        protected EnumField navigationModeEnumField { get; set; }

        protected FluidAnimatedContainer explicitNavigationAnimatedContainer { get; set; }
        protected FluidAnimatedContainer navigationAnimatedContainer { get; set; }
        protected FluidAnimatedContainer statesAnimatedContainer { get; set; }
        protected FluidAnimatedContainer settingsAnimatedContainer { get; set; }

        protected FluidButton visualizeNavigationButton { get; set; }

        protected FluidComponentHeader componentHeader { get; set; }

        protected FluidToggleButtonTab callbacksTabButton { get; set; }
        protected FluidToggleButtonTab navigationTabButton { get; set; }
        protected FluidToggleButtonTab settingsTabButton { get; set; }
        protected FluidToggleButtonTab statesTabButton { get; set; }

        protected FluidToggleCheckbox interactableCheckbox { get; set; }
        protected FluidToggleCheckbox deselectAfterPressCheckbox { get; set; }

        protected FluidToggleGroup toggleGroup { get; set; }

        protected FluidToggleButtonTab normalStateButton { get; set; }
        protected FluidToggleButtonTab highlightedStateButton { get; set; }
        protected FluidToggleButtonTab pressedStateButton { get; set; }
        protected FluidToggleButtonTab selectedStateButton { get; set; }
        protected FluidToggleButtonTab disabledStateButton { get; set; }

        protected SerializedProperty propertyInteractable { get; set; }
        protected SerializedProperty propertyNavigationMode { get; set; }
        protected SerializedProperty propertyNavigation { get; set; }
        protected SerializedProperty propertyNavigationSelectOnDown { get; set; }
        protected SerializedProperty propertyNavigationSelectOnLeft { get; set; }
        protected SerializedProperty propertyNavigationSelectOnRight { get; set; }
        protected SerializedProperty propertyNavigationSelectOnUp { get; set; }

        protected SerializedProperty propertyCurrentUISelectionState { get; set; }
        protected SerializedProperty propertyCurrentStateName { get; set; }
        protected SerializedProperty propertyNormalState { get; set; }
        protected SerializedProperty propertyHighlightedState { get; set; }
        protected SerializedProperty propertyPressedState { get; set; }
        protected SerializedProperty propertySelectedState { get; set; }
        protected SerializedProperty propertyDisabledState { get; set; }
        protected SerializedProperty propertyDeselectAfterPress { get; set; }


        public override VisualElement CreateInspectorGUI()
        {
            InitializeEditor();
            Compose();
            return root;
        }

        protected virtual void OnEnable()
        {
            showNavigation = EditorPrefs.GetBool(k_ShowNavigationKey);
            if (showNavigation) RegisterToSceneView();
        }

        protected virtual void OnDisable()
        {
            UnregisterFromSceneView();
        }

        protected virtual void OnDestroy()
        {
            componentHeader?.Recycle();

            interactableCheckbox?.Recycle();
            deselectAfterPressCheckbox?.Recycle();

            toggleGroup?.Recycle();
            settingsTabButton?.Recycle();
            statesTabButton?.Recycle();
            navigationTabButton?.Recycle();
            callbacksTabButton?.Recycle();

            normalStateButton?.Recycle();
            highlightedStateButton?.Recycle();
            pressedStateButton?.Recycle();
            selectedStateButton?.Recycle();
            disabledStateButton?.Recycle();

            statesAnimatedContainer?.Dispose();
            navigationAnimatedContainer?.Dispose();
            explicitNavigationAnimatedContainer?.Dispose();
            settingsAnimatedContainer?.Dispose();
        }

        protected virtual void FindProperties()
        {
            propertyInteractable = serializedObject.FindProperty("m_Interactable");

            propertyCurrentUISelectionState = serializedObject.FindProperty("CurrentUISelectionState");
            propertyCurrentStateName = serializedObject.FindProperty("CurrentStateName");
            propertyNormalState = serializedObject.FindProperty("NormalState");
            propertyHighlightedState = serializedObject.FindProperty("HighlightedState");
            propertyPressedState = serializedObject.FindProperty("PressedState");
            propertySelectedState = serializedObject.FindProperty("SelectedState");
            propertyDisabledState = serializedObject.FindProperty("DisabledState");

            propertyNavigation = serializedObject.FindProperty("m_Navigation");
            propertyNavigationMode = propertyNavigation.FindPropertyRelative("m_Mode");
            propertyNavigationSelectOnUp = propertyNavigation.FindPropertyRelative("m_SelectOnUp");
            propertyNavigationSelectOnDown = propertyNavigation.FindPropertyRelative("m_SelectOnDown");
            propertyNavigationSelectOnLeft = propertyNavigation.FindPropertyRelative("m_SelectOnLeft");
            propertyNavigationSelectOnRight = propertyNavigation.FindPropertyRelative("m_SelectOnRight");

            propertyDeselectAfterPress = serializedObject.FindProperty("DeselectAfterPress");
        }

        protected virtual void InitializeEditor()
        {
            FindProperties();

            root = new VisualElement();

            componentHeader =
                FluidComponentHeader.Get()
                    .SetElementSize(ElementSize.Large);

            interactableCheckbox = FluidToggleCheckbox.Get()
                .SetLabelText("Interactable")
                .SetTooltip("Can the Selectable be interacted with?")
                .BindToProperty(propertyInteractable);

            deselectAfterPressCheckbox = FluidToggleCheckbox.Get()
                .SetLabelText("Deselect after Press")
                .BindToProperty(propertyDeselectAfterPress);

            statesAnimatedContainer =
                new FluidAnimatedContainer()
                    .SetName("States")
                    .SetClearOnHide(false)
                    .Hide(false);

            navigationAnimatedContainer =
                new FluidAnimatedContainer()
                    .SetName("Navigation")
                    .SetClearOnHide(true)
                    .Hide(false);

            settingsAnimatedContainer =
                new FluidAnimatedContainer()
                    .SetName("Settings")
                    .SetClearOnHide(false)
                    .Show(false);

            toggleGroup =
                FluidToggleGroup.Get()
                    .SetControlMode(FluidToggleGroup.ControlMode.OneToggleOnEnforced);

            settingsTabButton =
                DesignUtils.GetTabButtonForComponentSection(settingsIconTextures, selectableAccentColor);

            settingsTabButton
                .SetLabelText("Settings")
                .SetOnValueChanged(evt => settingsAnimatedContainer.Toggle(evt.newValue))
                .SetIsOn(true, false)
                .AddToToggleGroup(toggleGroup);

            statesTabButton = DesignUtils.GetTabButtonForComponentSection(statesIconTextures, selectableAccentColor);
            statesTabButton
                .SetLabelText("States")
                .SetOnValueChanged(evt => statesAnimatedContainer.Toggle(evt.newValue))
                .AddToToggleGroup(toggleGroup);

            InitializeStates();
            InitializeNavigation();
        }

        protected VisualElement GetStateButtons() =>
            EditorApplication.isPlayingOrWillChangePlaymode
                ? DesignUtils.row
                    .SetName("State Buttons")
                    .AddChild(DesignUtils.flexibleSpace)
                    .AddChild(DesignUtils.spaceBlock)
                    .AddChild(normalStateButton)
                    .AddChild(highlightedStateButton)
                    .AddChild(pressedStateButton)
                    .AddChild(selectedStateButton)
                    .AddChild(disabledStateButton)
                    .AddChild(DesignUtils.spaceBlock)
                : DesignUtils.flexibleSpace;

        protected virtual void Compose()
        {

            root
                .AddChild(componentHeader)
                .AddChild
                (
                    DesignUtils.row
                        .SetStyleMargins(50, -4, DesignUtils.k_Spacing2X, DesignUtils.k_Spacing2X)
                        .AddChild(settingsTabButton)
                        .AddChild(DesignUtils.spaceBlock2X)
                        .AddChild(statesTabButton)
                        .AddChild(DesignUtils.spaceBlock)
                        .AddChild(navigationTabButton)
                        .AddChild(DesignUtils.flexibleSpace)
                        .AddChild(DesignUtils.spaceBlock2X)
                        .AddChild
                        (
                            DesignUtils.SystemButton_SortComponents
                            (
                                ((UISelectable)target).gameObject,
                                nameof(RectTransform),
                                nameof(Canvas),
                                nameof(CanvasGroup),
                                nameof(GraphicRaycaster),
                                nameof(UISelectable)
                            )
                        )
                )
                .AddChild(DesignUtils.spaceBlock)
                .AddChild
                (
                    settingsAnimatedContainer
                        .AddContent
                        (
                            DesignUtils.column
                                .AddChild
                                (
                                    DesignUtils.row
                                        .AddChild(interactableCheckbox)
                                        .AddChild(DesignUtils.spaceBlock)
                                        .AddChild(deselectAfterPressCheckbox)
                                        .AddChild(DesignUtils.spaceBlock)
                                        .AddChild(GetStateButtons())
                                )
                        )
                )
                .AddChild(statesAnimatedContainer)
                .AddChild(navigationAnimatedContainer)
                .AddChild(DesignUtils.endOfLineBlock);
        }

        private void InitializeStates()
        {
            EnumField currentStateEnumField = DesignUtils.NewEnumField(propertyCurrentUISelectionState, true);
            root.Add(currentStateEnumField);

            if (EditorApplication.isPlayingOrWillChangePlaymode)
            {
                FluidToggleButtonTab GetStateButton(UISelectionState selectionState) =>
                    FluidToggleButtonTab.Get()
                        .SetLabelText(selectionState.ToString())
                        .SetTabPosition(TabPosition.FloatingTab)
                        .SetElementSize(ElementSize.Tiny)
                        .SetToggleAccentColor(selectableAccentColor)
                        .SetStyleMarginTop(DesignUtils.k_Spacing / 2f)
                        .SetStyleMarginRight(DesignUtils.k_Spacing / 2f)
                        .SetContainerColorOff(DesignUtils.tabButtonColorOff)
                        .SetOnClick(() =>
                        {
                            if (serializedObject.isEditingMultipleObjects)
                            {
                                foreach (UISelectable s in selectables)
                                    s.SetState(selectionState);
                                return;
                            }

                            selectable.SetState(selectionState);
                        });


                normalStateButton = GetStateButton(UISelectionState.Normal);
                highlightedStateButton = GetStateButton(UISelectionState.Highlighted);
                pressedStateButton = GetStateButton(UISelectionState.Pressed);
                selectedStateButton = GetStateButton(UISelectionState.Selected);
                disabledStateButton = GetStateButton(UISelectionState.Disabled);

                void UpdateStateButtons(UISelectionState state)
                {
                    normalStateButton.SetIsOn(state == UISelectionState.Normal);
                    highlightedStateButton.SetIsOn(state == UISelectionState.Highlighted);
                    pressedStateButton.SetIsOn(state == UISelectionState.Pressed);
                    selectedStateButton.SetIsOn(state == UISelectionState.Selected);
                    disabledStateButton.SetIsOn(state == UISelectionState.Disabled);
                }

                currentStateEnumField.RegisterValueChangedCallback(evt =>
                {
                    if(evt?.newValue == null) return;
                    UpdateStateButtons((UISelectionState)evt.newValue);
                });
                UpdateStateButtons((UISelectionState)propertyCurrentUISelectionState.enumValueIndex);
            }

            statesAnimatedContainer.AddContent
            (
                DesignUtils.column
                    .AddChild(DesignUtils.spaceBlock)
                    .AddChild(DesignUtils.NewPropertyField(propertyNormalState))
                    .AddChild(DesignUtils.spaceBlock)
                    .AddChild(DesignUtils.NewPropertyField(propertyHighlightedState))
                    .AddChild(DesignUtils.spaceBlock)
                    .AddChild(DesignUtils.NewPropertyField(propertyPressedState))
                    .AddChild(DesignUtils.spaceBlock)
                    .AddChild(DesignUtils.NewPropertyField(propertySelectedState))
                    .AddChild(DesignUtils.spaceBlock)
                    .AddChild(DesignUtils.NewPropertyField(propertyDisabledState))
                    .AddChild(DesignUtils.endOfLineBlock)
            );
        }

        private void InitializeNavigation()
        {
            navigationTabButton =
                DesignUtils.GetTabButtonForComponentSection(navigationIconTextures, selectableAccentColor);

            navigationTabButton
                .SetLabelText("Navigation")
                .SetOnValueChanged(evt => navigationAnimatedContainer.Toggle(evt.newValue))
                .AddToToggleGroup(toggleGroup);


            FluidField NavigationSelectField(string text, IEnumerable<Texture2D> textures, SerializedProperty property) =>
                FluidField.Get()
                    .SetLabelText(text)
                    .SetIcon(textures)
                    .SetElementSize(ElementSize.Small)
                    .AddFieldContent
                    (
                        DesignUtils.NewObjectField(property, typeof(Selectable))
                            .SetStyleFlexGrow(1)
                    );

            navigationAnimatedContainer.SetOnShowCallback(() =>
            {
                navigationModeEnumField = DesignUtils.NewEnumField(propertyNavigationMode).SetStyleFlexGrow(1).SetStyleHeight(26);
                navigationModeEnumField.RegisterValueChangedCallback(evt =>
                {
                    if (evt?.newValue == null) return;
                    explicitNavigationAnimatedContainer?.Toggle((Navigation.Mode)evt.newValue == Navigation.Mode.Explicit);
                });

                visualizeNavigationButton = FluidButton.Get()
                    .SetLabelText("Navigation")
                    .SetButtonStyle(ButtonStyle.Contained)
                    .SetElementSize(ElementSize.Small)
                    .SetStyleFlexShrink(0)
                    .SetOnClick(() =>
                    {
                        showNavigation = !showNavigation;
                        UpdateVisualizeNavigationButton();
                    });

                UpdateVisualizeNavigationButton();


                explicitNavigationAnimatedContainer = new FluidAnimatedContainer().SetClearOnHide(true);
                explicitNavigationAnimatedContainer
                    .SetOnShowCallback(() =>
                    {
                        explicitNavigationAnimatedContainer.AddContent(DesignUtils.spaceBlock);
                        explicitNavigationAnimatedContainer.AddContent(NavigationSelectField("Select On Up", EditorMicroAnimations.EditorUI.Arrows.ArrowUp, propertyNavigationSelectOnUp));
                        explicitNavigationAnimatedContainer.AddContent(DesignUtils.spaceBlock);
                        explicitNavigationAnimatedContainer.AddContent(NavigationSelectField("Select On Down", EditorMicroAnimations.EditorUI.Arrows.ArrowDown, propertyNavigationSelectOnDown));
                        explicitNavigationAnimatedContainer.AddContent(DesignUtils.spaceBlock);
                        explicitNavigationAnimatedContainer.AddContent(NavigationSelectField("Select On Left", EditorMicroAnimations.EditorUI.Arrows.ArrowLeft, propertyNavigationSelectOnLeft));
                        explicitNavigationAnimatedContainer.AddContent(DesignUtils.spaceBlock);
                        explicitNavigationAnimatedContainer.AddContent(NavigationSelectField("Select On Right", EditorMicroAnimations.EditorUI.Arrows.ArrowRight, propertyNavigationSelectOnRight));
                        explicitNavigationAnimatedContainer.Bind(serializedObject);
                    })
                    .Toggle((Navigation.Mode)propertyNavigationMode.enumValueIndex == Navigation.Mode.Explicit, false);

                navigationAnimatedContainer
                    .AddContent(DesignUtils.spaceBlock)
                    .AddContent
                    (
                        FluidField.Get("Navigation Mode")
                            .SetIcon(navigationIconTextures)
                            .AddFieldContent(navigationModeEnumField)
                            .AddInfoElement
                            (
                                DesignUtils.column
                                    .AddChild(DesignUtils.flexibleSpace)
                                    .AddChild(visualizeNavigationButton)
                            )
                    )
                    .AddContent(explicitNavigationAnimatedContainer)
                    .AddContent(DesignUtils.endOfLineBlock);

                navigationAnimatedContainer.Bind(serializedObject);
            });
        }

        private void UpdateVisualizeNavigationButton()
        {
            if (showNavigation)
            {
                RegisterToSceneView();
                visualizeNavigationButton
                    .SetLabelText("Hide Navigation")
                    .SetTooltip("Hide selectable navigation flow in Scene View")
                    .SetIcon(hideIconTextures);
            }
            else
            {
                UnregisterFromSceneView();
                visualizeNavigationButton
                    .SetLabelText("Show Navigation")
                    .SetTooltip("Show selectable navigation flow in Scene View")
                    .SetIcon(showIconTextures);
            }
            EditorPrefs.SetBool(k_ShowNavigationKey, showNavigation);
            SceneView.RepaintAll();
        }

        #region Visualize Navigation

        private static void DrawNavigation(SceneView sceneView)
        {
            if (!showNavigation)
                return;

            foreach (Selectable s in Selectable.allSelectablesArray.Where(s => s != null))
            {
                if (StageUtility.IsGameObjectRenderedByCamera(s.gameObject, Camera.current))
                    DrawNavigationForSelectable(s);
            }
        }

        private static void DrawNavigationForSelectable(Selectable s)
        {
            if (s == null)
                return;

            Transform transform = s.transform;
            bool active = Selection.transforms.Any(e => e == transform);
            Handles.color = new Color(1.0f, 0.9f, 0.1f, active ? 1.0f : 0.4f);

            Selectable findSelectableOnLeft = s.FindSelectableOnLeft();
            if (findSelectableOnLeft != null)
                DrawNavigationArrow(-Vector2.right, s, findSelectableOnLeft);

            Selectable findSelectableOnRight = s.FindSelectableOnRight();
            if (findSelectableOnRight != null)
                DrawNavigationArrow(Vector2.right, s, findSelectableOnRight);

            Selectable findSelectableOnUp = s.FindSelectableOnUp();
            if (findSelectableOnUp)
                DrawNavigationArrow(Vector2.up, s, findSelectableOnUp);

            Selectable findSelectableOnDown = s.FindSelectableOnDown();
            if (findSelectableOnDown)
                DrawNavigationArrow(-Vector2.up, s, findSelectableOnDown);
        }

        private const float K_ARROW_THICKNESS = 2.5f;
        private const float K_ARROW_HEAD_SIZE = 1.2f;

        private static void DrawNavigationArrow(Vector2 direction, Selectable fromObj, Selectable toObj)
        {
            if (fromObj == null || toObj == null)
                return;
            Transform fromTransform = fromObj.transform;
            Transform toTransform = toObj.transform;

            Vector2 sideDir = new Vector2(direction.y, -direction.x);
            Vector3 fromPoint = fromTransform.TransformPoint(GetPointOnRectEdge(fromTransform as RectTransform, direction));
            Vector3 toPoint = toTransform.TransformPoint(GetPointOnRectEdge(toTransform as RectTransform, -direction));
            float fromSize = HandleUtility.GetHandleSize(fromPoint) * 0.05f;
            float toSize = HandleUtility.GetHandleSize(toPoint) * 0.05f;
            fromPoint += fromTransform.TransformDirection(sideDir) * fromSize;
            toPoint += toTransform.TransformDirection(sideDir) * toSize;
            float length = Vector3.Distance(fromPoint, toPoint);

            Quaternion fromObjRotation = fromTransform.rotation;
            Quaternion toObjRotation = toTransform.rotation;

            Vector3 fromTangent = fromObjRotation * direction * length * 0.3f;
            Vector3 toTangent = toObjRotation * -direction * length * 0.3f;

            Handles.DrawBezier(fromPoint, toPoint, fromPoint + fromTangent, toPoint + toTangent, Handles.color, null, K_ARROW_THICKNESS);
            Handles.DrawAAPolyLine(K_ARROW_THICKNESS, toPoint, toPoint + toObjRotation * (-direction - sideDir) * toSize * K_ARROW_HEAD_SIZE);
            Handles.DrawAAPolyLine(K_ARROW_THICKNESS, toPoint, toPoint + toObjRotation * (-direction + sideDir) * toSize * K_ARROW_HEAD_SIZE);
        }

        private static Vector3 GetPointOnRectEdge(RectTransform rect, Vector2 dir)
        {
            if (rect == null) return Vector3.zero;
            if (dir != Vector2.zero) dir /= Mathf.Max(Mathf.Abs(dir.x), Mathf.Abs(dir.y));
            Rect rectRect = rect.rect;
            dir = rectRect.center + Vector2.Scale(rectRect.size, dir * 0.5f);
            return dir;
        }

        #endregion

        #region SceneView

        private bool registeredToSceneView { get; set; }
        private void RegisterToSceneView()
        {
            if (registeredToSceneView) return;
            SceneView.duringSceneGui += DrawNavigation;
            registeredToSceneView = true;
        }
        private void UnregisterFromSceneView()
        {
            if (!registeredToSceneView) return;
            SceneView.duringSceneGui -= DrawNavigation;
            registeredToSceneView = false;
        }

        #endregion
    }
}
