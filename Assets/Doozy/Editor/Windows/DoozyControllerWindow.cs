using System.Collections.Generic;
using Doozy.Editor.Common;
using Doozy.Editor.Common.ScriptableObjects;
using Doozy.Editor.EditorUI;
using Doozy.Editor.EditorUI.Components;
using Doozy.Editor.EditorUI.ScriptableObjects.Colors;
using Doozy.Editor.EditorUI.Utils;
using Doozy.Editor.EditorUI.Windows.Internal;
using Doozy.Editor.Reactor.Internal;
using Doozy.Runtime.Reactor.Extensions;
using Doozy.Runtime.Reactor.Internal;
using Doozy.Runtime.Reactor.Reactions;
using Doozy.Runtime.UIElements.Extensions;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
namespace Doozy.Editor.Windows
{
    public class DoozyControllerWindow : FluidWindow<DoozyControllerWindow>
    {
        private const string WINDOW_TITLE = "Doozy Controller";
        private const string WINDOW_MENU_PATH = "Tools/42/";

        #if DOOZY_42
        [MenuItem(WINDOW_MENU_PATH + WINDOW_TITLE)]
        public static void Open() => InternalOpenWindow(WINDOW_TITLE);
        #endif

        private static string[] pathsToDelete =>
            new[]
            {
                //--------------
                //--- Editor ---
                //--------------

                //Editor databases .asset
                // "Assets/Doozy/Editor/Data/EditorDataColorDatabase.asset",
                // "Assets/Doozy/Editor/Data/EditorDataFontDatabase.asset",
                // "Assets/Doozy/Editor/Data/EditorDataLayoutDatabase.asset",
                // "Assets/Doozy/Editor/Data/EditorDataMicroAnimationDatabase.asset",
                // "Assets/Doozy/Editor/Data/EditorDataSelectableColorDatabase.asset",
                // "Assets/Doozy/Editor/Data/EditorDataStyleDatabase.asset",
                // "Assets/Doozy/Editor/Data/EditorDataTextureDatabase.asset",

                //EditorUI Settings .asset
                "Assets/Doozy/Editor/Data/EditorUISettings.asset",

                //Category Names databases .asset
                "Assets/Doozy/Editor/Data/StreamIdDatabase.asset",
                "Assets/Doozy/Editor/Data/UIButtonIdDatabase.asset",
                "Assets/Doozy/Editor/Data/UISliderIdDatabase.asset",
                "Assets/Doozy/Editor/Data/UIToggleIdDatabase.asset",
                "Assets/Doozy/Editor/Data/UIViewIdDatabase.asset",

                //UIMenu Items Database .asset
                "Assets/Doozy/Editor/Data/UIMenuItemsDatabase.asset",

                //UIMenu Settings .asset
                "Assets/Doozy/Editor/Data/UIMenuSettings.asset",

                //---------------
                //--- Runtime ---
                //---------------

                //Id Extensions .cs
                "Assets/Doozy/Runtime/Signals/StreamIdExtension.cs",
                "Assets/Doozy/Runtime/UIManager/Ids/UIButtonIdExtension.cs",
                "Assets/Doozy/Runtime/UIManager/Ids/UISliderIdExtension.cs",
                "Assets/Doozy/Runtime/UIManager/Ids/UIToggleIdExtension.cs",
                "Assets/Doozy/Runtime/UIManager/Ids/UIViewIdExtension.cs",

                //Reactor Settings .asset
                "Assets/Doozy/Runtime/Data/Resources/ReactorSettings.asset",

                //UIManager Input Settings .asset
                "Assets/Doozy/Runtime/Data/Resources/UIManagerInputSettings.asset",
                
                //Assembly Definitions
                "Assets/Doozy/Editor/Doozy.Editor.asmdef",
                "Assets/Doozy/Runtime/Doozy.Runtime.asmdef"
            };

        private Color accentColor => EditorColors.Default.UnityThemeInversed;
        private EditorSelectableColorInfo selectableAccentColor => EditorSelectableColors.Default.UnityThemeInversed;

        private FluidButton cleanProjectButton { get; set; }
        private FluidButton runUpdateBotButton { get; set; }
        private FluidButton updateVersionButton { get; set; }

        private Image doozyUiIconImage { get; set; }
        private Texture2DReaction doozyUiIconReaction { get; set; }
        private Label copyrightLabel { get; set; }
        private Label doozyUiVersionLabel { get; set; }

        private TextField versionTextField { get; set; }

        private ProductInfo m_DoozyProductInfo;
        private ProductInfo doozyProductInfo =>
            m_DoozyProductInfo ? m_DoozyProductInfo : m_DoozyProductInfo = ProductInfo.Get("Doozy UI Manager");

        protected override void OnEnable()
        {
            base.OnEnable();
            minSize = maxSize = new Vector2(300, 360);
        }

        protected override void CreateGUI()
        {
            Initialize();
            Compose();
        }

        private void Initialize()
        {
            root
                .SetStyleBackgroundColor(EditorColors.Default.BoxBackground)
                .SetStylePadding(DesignUtils.k_Spacing2X);

            doozyUiIconImage =
                new Image()
                    .ResetLayout()
                    .SetStyleSize(128);

            doozyUiIconReaction =
                doozyUiIconImage.GetTexture2DReaction().SetEditorHeartbeat()
                    .SetDuration(0.6f)
                    .SetTextures(EditorMicroAnimations.UIManager.Icons.DoozyUI);

            doozyUiVersionLabel =
                DesignUtils.fieldLabel
                    .SetText(doozyProductInfo.nameAndVersion)
                    .SetStyleTextAlign(TextAnchor.MiddleCenter)
                    .SetStyleFontSize(11);

            copyrightLabel =
                DesignUtils.fieldLabel
                    .SetText("Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.");

            doozyUiIconImage.RegisterCallback<PointerEnterEvent>(evt => doozyUiIconReaction?.Play());
            doozyUiIconImage.AddManipulator(new Clickable(() => doozyUiIconReaction?.Play()));

            const int buttonWidth = 200;

            cleanProjectButton =
                FluidButton.Get()
                    .SetLabelText("Clean Project")
                    .SetIcon(EditorMicroAnimations.EditorUI.Icons.Clear)
                    .SetButtonStyle(ButtonStyle.Contained)
                    .SetStyleWidth(buttonWidth)
                    .SetOnClick(CleanProject);

            runUpdateBotButton =
                FluidButton.Get()
                    .SetLabelText("Run Update Bot")
                    .SetIcon(EditorMicroAnimations.EditorUI.Icons.Unity)
                    .SetButtonStyle(ButtonStyle.Contained)
                    .SetStyleWidth(buttonWidth)
                    .SetOnClick(RunUpdateBot);

            versionTextField = 
                new TextField()
                    .ResetLayout()
                    .SetStyleWidth(50)
                    .SetStyleFlexGrow(0)
                    .SetStyleHeight(30);
            versionTextField.value = $"{doozyProductInfo.Major}.{doozyProductInfo.Minor}.{doozyProductInfo.RevisionVersion}";

            updateVersionButton =
                FluidButton.Get()
                    .SetLabelText("Update Version")
                    .SetIcon(EditorMicroAnimations.EditorUI.Icons.Save)
                    .SetButtonStyle(ButtonStyle.Contained)
                    .SetStyleWidth(buttonWidth - DesignUtils.k_Spacing - 50)
                    .SetOnClick(UpdateVersion);
        }

        private void Compose()
        {
            root
                .AddChild
                (
                    DesignUtils.row
                        .AddChild(DesignUtils.flexibleSpace)
                        .AddChild
                        (
                            DesignUtils.column
                                .SetStyleJustifyContent(Justify.Center)
                                .SetStyleAlignItems(Align.Center)
                                .AddChild(doozyUiIconImage)
                                .AddChild(DesignUtils.spaceBlock2X)
                                .AddChild(doozyUiVersionLabel)
                                .AddChild(DesignUtils.spaceBlock4X)
                                .AddChild(DesignUtils.spaceBlock4X)
                                .AddChild(cleanProjectButton)
                                .AddChild(DesignUtils.spaceBlock)
                                .AddChild
                                (
                                    DesignUtils.row
                                        .SetStyleFlexGrow(0)
                                        .AddChild(versionTextField)
                                        .AddChild(DesignUtils.spaceBlock)
                                        .AddChild(updateVersionButton)
                                )
                                .AddChild(DesignUtils.spaceBlock4X)
                                .AddChild(runUpdateBotButton)
                        )
                        .AddChild(DesignUtils.flexibleSpace)
                );
        }

        private static void CleanProject()
        {
            AssetDatabase.MoveAssetsToTrash
            (
                pathsToDelete,
                new List<string>()
            );

            UpdateBot.instance.Update = true;
            UpdateBot.Save();
        }

        private static void RunUpdateBot()
        {
            UpdateBot.Execute();
        }

        private void UpdateVersion()
        {
            if (versionTextField.value.Length != 5)
            {
                EditorUtility.DisplayDialog
                (
                    "Wrong version",
                    "Version should be {Major}.{Minor}.{Revision}" +
                    "\n" +
                    "eg. 1.2.3",
                    "Ok"
                );
                return;
            }

            if (!int.TryParse(versionTextField.value[0].ToString(), out int major))
            {
                EditorUtility.DisplayDialog
                (
                    "Attention required",
                    "Major version is not a number",
                    "Ok"
                );

                return;
            }

            if (!int.TryParse(versionTextField.value[2].ToString(), out int minor))
            {
                EditorUtility.DisplayDialog
                (
                    "Attention required",
                    "Minor version is not a number",
                    "Ok"
                );

                return;
            }

            if (!int.TryParse(versionTextField.value[4].ToString(), out int revision))
            {
                EditorUtility.DisplayDialog
                (
                    "Attention required",
                    "Patch version is not a number",
                    "Ok"
                );

                return;
            }

            if (!EditorUtility.DisplayDialog
                (
                    "Confirm",
                    $"Update version to {major}.{minor}.{revision}?",
                    "Update",
                    "Cancel"
                )
               )
                return;

            doozyProductInfo.Major = major;
            doozyProductInfo.Minor = minor;
            doozyProductInfo.RevisionVersion = revision;
            EditorUtility.SetDirty(doozyProductInfo);
            AssetDatabase.SaveAssetIfDirty(doozyProductInfo);

            doozyUiVersionLabel
                .SetText(doozyProductInfo.nameAndVersion);
        }

    }
}
