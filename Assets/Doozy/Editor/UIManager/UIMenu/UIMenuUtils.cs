// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using System.Collections.Generic;
using System.Linq;
using Doozy.Editor.EditorUI;
using UnityEditor;
using UnityEditor.Experimental.SceneManagement;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Object = UnityEngine.Object;
// ReSharper disable MemberCanBePrivate.Global

namespace Doozy.Editor.UIManager.UIMenu
{
    public static class UIMenuUtils
    {
        public const string k_UILayerName = "UI";

        public static void CreateEventSystem(bool select, GameObject parent = null)
        {
            StageHandle stage = parent == null ? StageUtility.GetCurrentStageHandle() : StageUtility.GetStageHandle(parent);
            EventSystem system = stage.FindComponentOfType<EventSystem>();
            if (system == null)
            {
                GameObject eventSystem = ObjectFactory.CreateGameObject("EventSystem");
                if (parent == null)
                    StageUtility.PlaceGameObjectInCurrentStage(eventSystem);
                else
                    SetParentAndAlign(eventSystem, parent);
                system = ObjectFactory.AddComponent<EventSystem>(eventSystem);
                ObjectFactory.AddComponent<StandaloneInputModule>(eventSystem);

                Undo.RegisterCreatedObjectUndo(eventSystem, "Create " + eventSystem.name);
            }

            if (select && system != null)
            {
                Selection.activeGameObject = system.gameObject;
            }
        }

        public static void SetParentAndAlign(GameObject child, GameObject parent)
        {
            if (parent == null)
                return;

            Undo.SetTransformParent(child.transform, parent.transform, "");

            var rectTransform = child.transform as RectTransform;
            if (rectTransform)
            {
                rectTransform.anchoredPosition = Vector2.zero;
                Vector3 localPosition = rectTransform.localPosition;
                localPosition.z = 0;
                rectTransform.localPosition = localPosition;
            }
            else
            {
                child.transform.localPosition = Vector3.zero;
            }
            child.transform.localRotation = Quaternion.identity;
            child.transform.localScale = Vector3.one;

            SetLayerRecursively(child, parent.layer);
        }

        public static void SetLayerRecursively(GameObject go, int layer)
        {
            go.layer = layer;
            Transform t = go.transform;
            for (int i = 0; i < t.childCount; i++)
                SetLayerRecursively(t.GetChild(i).gameObject, layer);
        }

        public static GameObject CreateNewUI()
        {
            // Root for the UI
            var root = ObjectFactory.CreateGameObject("Canvas", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
            root.layer = LayerMask.NameToLayer(k_UILayerName);
            Canvas canvas = root.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;

            // Works for all stages.
            StageUtility.PlaceGameObjectInCurrentStage(root);
            bool customScene = false;
            PrefabStage prefabStage = PrefabStageUtility.GetCurrentPrefabStage();
            if (prefabStage != null)
            {
                Undo.SetTransformParent(root.transform, prefabStage.prefabContentsRoot.transform, "");
                customScene = true;
            }

            Undo.SetCurrentGroupName("Create " + root.name);

            // If there is no event system add one...
            // No need to place event system in custom scene as these are temporary anyway.
            // It can be argued for or against placing it in the user scenes,
            // but let's not modify scene user is not currently looking at.
            if (!customScene)
                CreateEventSystem(false);
            return root;
        }

        // Helper function that returns a Canvas GameObject; preferably a parent of the selection, or other existing Canvas.
        public static GameObject GetOrCreateCanvasGameObject()
        {
            GameObject selectedGo = Selection.activeGameObject;

            // Try to find a gameobject that is the selected GO or one if its parents.
            Canvas canvas = (selectedGo != null) ? selectedGo.GetComponentInParent<Canvas>() : null;
            if (IsValidCanvas(canvas))
                return canvas.gameObject;

            // No canvas in selection or its parents? Then use any valid canvas.
            // We have to find all loaded Canvases, not just the ones in main scenes.
            Canvas[] canvasArray = StageUtility.GetCurrentStageHandle().FindComponentsOfType<Canvas>();
            for (int i = 0; i < canvasArray.Length; i++)
                if (IsValidCanvas(canvasArray[i]))
                    return canvasArray[i].gameObject;

            // No canvas in the scene at all? Then create a new one.
            return CreateNewUI();
        }

        public static bool IsValidCanvas(Canvas canvas)
        {
            if (canvas == null || !canvas.gameObject.activeInHierarchy)
                return false;

            // It's important that the non-editable canvas from a prefab scene won't be rejected,
            // but canvases not visible in the Hierarchy at all do. Don't check for HideAndDontSave.
            if (EditorUtility.IsPersistent(canvas) || (canvas.hideFlags & HideFlags.HideInHierarchy) != 0)
                return false;

            return StageUtility.GetStageHandle(canvas.gameObject) == StageUtility.GetCurrentStageHandle();
        }

        private static void SetPositionVisibleInSceneView(RectTransform canvasRTransform, RectTransform itemTransform)
        {
            SceneView sceneView = SceneView.lastActiveSceneView;

            // Couldn't find a SceneView. Don't set position.
            if (sceneView == null || sceneView.camera == null)
                return;

            // Create world space Plane from canvas position.
            Vector2 localPlanePosition;
            Camera camera = sceneView.camera;
            Vector3 position = Vector3.zero;
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRTransform, new Vector2(camera.pixelWidth / 2f, camera.pixelHeight / 2f), camera, out localPlanePosition))
            {
                // Adjust for canvas pivot
                localPlanePosition.x = localPlanePosition.x + canvasRTransform.sizeDelta.x * canvasRTransform.pivot.x;
                localPlanePosition.y = localPlanePosition.y + canvasRTransform.sizeDelta.y * canvasRTransform.pivot.y;

                localPlanePosition.x = Mathf.Clamp(localPlanePosition.x, 0, canvasRTransform.sizeDelta.x);
                localPlanePosition.y = Mathf.Clamp(localPlanePosition.y, 0, canvasRTransform.sizeDelta.y);

                // Adjust for anchoring
                position.x = localPlanePosition.x - canvasRTransform.sizeDelta.x * itemTransform.anchorMin.x;
                position.y = localPlanePosition.y - canvasRTransform.sizeDelta.y * itemTransform.anchorMin.y;

                Vector3 minLocalPosition;
                minLocalPosition.x = canvasRTransform.sizeDelta.x * (0 - canvasRTransform.pivot.x) + itemTransform.sizeDelta.x * itemTransform.pivot.x;
                minLocalPosition.y = canvasRTransform.sizeDelta.y * (0 - canvasRTransform.pivot.y) + itemTransform.sizeDelta.y * itemTransform.pivot.y;

                Vector3 maxLocalPosition;
                maxLocalPosition.x = canvasRTransform.sizeDelta.x * (1 - canvasRTransform.pivot.x) - itemTransform.sizeDelta.x * itemTransform.pivot.x;
                maxLocalPosition.y = canvasRTransform.sizeDelta.y * (1 - canvasRTransform.pivot.y) - itemTransform.sizeDelta.y * itemTransform.pivot.y;

                position.x = Mathf.Clamp(position.x, minLocalPosition.x, maxLocalPosition.x);
                position.y = Mathf.Clamp(position.y, minLocalPosition.y, maxLocalPosition.y);
            }

            itemTransform.anchoredPosition = position;
            itemTransform.localRotation = Quaternion.identity;
            itemTransform.localScale = Vector3.one;
        }

        public static UIMenuItem GetPrefabMenuItem(string prefabTypeName, string prefabCategory, string prefabName) =>
            UIMenuItemsDatabase.GetMenuItem(prefabTypeName, prefabCategory, prefabName);

        public static GameObject GetPrefab(string prefabTypeName, string prefabCategory, string prefabName)
        {
            UIMenuItem menuItem = GetPrefabMenuItem(prefabTypeName, prefabCategory, prefabName);
            return menuItem == null ? null : menuItem.prefab;
        }

        public static void AddToScene(string prefabTypeName, string prefabCategory, string prefabName) =>
            AddToScene(GetPrefabMenuItem(prefabTypeName, prefabCategory, prefabName));

        public static void AddToScene(UIMenuItem menuItem)
        {
            if (menuItem == null) return;
            PrefabInstantiateMode instantiateMode = menuItem.instantiateMode;
            if (!menuItem.lockInstantiateMode)
            {
                switch (UIMenuSettings.instance.InstantiateMode)
                {
                    case PrefabInstantiateModeSetting.Default:
                        //ignored
                        break;
                    case PrefabInstantiateModeSetting.Clone:
                        instantiateMode = PrefabInstantiateMode.Clone;
                        break;
                    case PrefabInstantiateModeSetting.Link:
                        instantiateMode = PrefabInstantiateMode.Link;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            bool autoSelect = UIMenuSettings.instance.SelectNewlyCreatedObjects;

            AddToScene(menuItem.prefab, menuItem.prefabName, instantiateMode, autoSelect);

            CreateEventSystem(false);
        }

        public static void AddToScene(GameObject prefab, string prefabName, PrefabInstantiateMode instantiateMode = PrefabInstantiateMode.Clone, bool selectNewlyCreatedObject = false)
        {
            if (prefab == null) return;

            bool isUIPrefab = prefab.GetComponent<RectTransform>() != null;

            GameObject parent = Selection.activeGameObject;

            bool explicitParentChoice = true;

            if (isUIPrefab)
            {
                if (parent == null)
                {
                    parent = GetOrCreateCanvasGameObject();
                    explicitParentChoice = false;

                    // If in Prefab Mode, Canvas has to be part of Prefab contents,
                    // otherwise use Prefab root instead.
                    PrefabStage prefabStage = PrefabStageUtility.GetCurrentPrefabStage();
                    if (prefabStage != null && !prefabStage.IsPartOfPrefabContents(parent))
                        parent = prefabStage.prefabContentsRoot;
                }

                if (parent.GetComponentsInParent<Canvas>(true).Length == 0)
                {
                    // Create canvas under context GameObject,
                    // and make that be the parent which UI element is added under.
                    GameObject canvas = CreateNewUI();
                    Undo.SetTransformParent(canvas.transform, parent.transform, "");
                    parent = canvas;
                }
            }

            GameObject element;
            switch (instantiateMode)
            {
                case PrefabInstantiateMode.Clone:
                    element = parent != null ? Object.Instantiate(prefab, parent.transform) : Object.Instantiate(prefab);
                    // clone.name = clone.name.Replace("(Clone)", "");
                    element.name = prefabName;
                    Undo.RegisterCreatedObjectUndo(element, $"Create {element.name}");
                    break;
                case PrefabInstantiateMode.Link:
                    element = parent != null ? (GameObject)PrefabUtility.InstantiatePrefab(prefab, parent.transform) : (GameObject)PrefabUtility.InstantiatePrefab(prefab);
                    element.name = prefabName;
                    Undo.RegisterCreatedObjectUndo(element, $"Create {element.name}");
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(instantiateMode), instantiateMode, null);
            }

            GameObjectUtility.EnsureUniqueNameForSibling(element);

            if (parent != null)
                SetParentAndAlign(element, parent);

            if (isUIPrefab)
                if (!explicitParentChoice) // not a context click, so center in scene view
                    SetPositionVisibleInSceneView(parent.GetComponent<RectTransform>(), element.GetComponent<RectTransform>());

            // This call ensure any change made to created Objects after they where registered will be part of the Undo.
            Undo.RegisterFullObjectHierarchyUndo(parent == null ? element : parent, "");

            // We have to fix up the undo name since the name of the object was only known after re-parenting it
            Undo.SetCurrentGroupName("Create " + element.name);

            if (selectNewlyCreatedObject)
                Selection.activeGameObject = element;
        }

        public static IEnumerable<Texture2D> GetIconTextures(UIMenuItem item)
        {
            List<Texture2D> textures = EditorMicroAnimations.EditorUI.Icons.QuestionMark;
            if (item == null) return textures;

            if (item.hasAnimatedIcon)
            {
                return item.icon;
            }

            if (item.hasIcon)
            {
                return new[] { item.icon.First() };
            }

            switch (item.prefabType)
            {
                case UIPrefabType.Component:
                    textures = EditorMicroAnimations.UIManager.UIMenu.Component;

                    break;
                case UIPrefabType.Container:
                    textures = EditorMicroAnimations.UIManager.UIMenu.Container;

                    break;
                case UIPrefabType.Content:
                    textures = EditorMicroAnimations.UIManager.UIMenu.Content;

                    break;
                case UIPrefabType.Custom:
                    textures = EditorMicroAnimations.UIManager.UIMenu.Custom;

                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            //component
            if (item.tags.Contains("button")) textures = EditorMicroAnimations.UIManager.UIMenu.Button;
            if (item.tags.Contains("dropdown")) textures = EditorMicroAnimations.UIManager.UIMenu.Dropdown;
            if (item.tags.Contains("toggle")) textures = EditorMicroAnimations.UIManager.UIMenu.Checkbox;
            if (item.tags.Contains("checkbox")) textures = EditorMicroAnimations.UIManager.UIMenu.Checkbox;
            if (item.tags.Contains("switch")) textures = EditorMicroAnimations.UIManager.UIMenu.Switch;
            if (item.tags.Contains("radio")) textures = EditorMicroAnimations.UIManager.UIMenu.RadioButton;
            if (item.tags.Contains("inputfield")) textures = EditorMicroAnimations.UIManager.UIMenu.InputField;
            if (item.tags.Contains("inputField")) textures = EditorMicroAnimations.UIManager.UIMenu.InputField;
            if (item.tags.Contains("input field")) textures = EditorMicroAnimations.UIManager.UIMenu.InputField;
            if (item.tags.Contains("scrollbar")) textures = EditorMicroAnimations.UIManager.UIMenu.Scollbar;
            if (item.tags.Contains("scrollview")) textures = EditorMicroAnimations.UIManager.UIMenu.ScrollView;
            if (item.tags.Contains("slider")) textures = EditorMicroAnimations.UIManager.UIMenu.Slider;

            //container
            if (item.tags.Contains("view")) textures = EditorMicroAnimations.UIManager.Icons.Views;

            //layout
            if (item.tags.Contains("layout") & item.tags.Contains("grid")) textures = EditorMicroAnimations.UIManager.UIMenu.GridLayout;
            if (item.tags.Contains("layout") & item.tags.Contains("horizontal")) textures = EditorMicroAnimations.UIManager.UIMenu.HorizontalLayout;
            if (item.tags.Contains("layout") & item.tags.Contains("vertical")) textures = EditorMicroAnimations.UIManager.UIMenu.VerticalLayout;
            if (item.tags.Contains("layout") & item.tags.Contains("radial")) textures = EditorMicroAnimations.UIManager.UIMenu.RadialLayout;


            return textures;
        }
    }
}
