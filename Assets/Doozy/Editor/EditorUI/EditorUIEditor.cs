using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Doozy.Editor.EditorUI.Components;
using Doozy.Editor.EditorUI.ScriptableObjects.Colors;
using Doozy.Editor.EditorUI.Utils;
using Doozy.Runtime.Pooler;
using Doozy.Runtime.UIElements.Extensions;
using UnityEngine;
using UnityEngine.UIElements;
namespace Doozy.Editor.EditorUI
{
    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
    [SuppressMessage("ReSharper", "MemberCanBeProtected.Global")]
    public abstract class EditorUIEditor<Tclass> : UnityEditor.Editor where Tclass : MonoBehaviour
    {
        public Tclass castedTarget => (Tclass)target;

        private List<IPoolable> m_Poolables;
        public List<IPoolable> poolables => m_Poolables ?? (m_Poolables = new List<IPoolable>());

        private List<IDisposable> m_Disposables;
        public List<IDisposable> disposables => m_Disposables ?? (m_Disposables = new List<IDisposable>());

        public const string k_DefaultManualURL = "www.bit.ly/DoozyKnowledgeBase4";
        public virtual string manualURL => k_DefaultManualURL;

        public const string k_DefaultYouTubeURL = "www.youtube.com/c/DoozyEntertainment";
        public virtual string youTubeURL => k_DefaultYouTubeURL;

        public virtual Color accentColor => EditorColors.EditorUI.Amber;
        public virtual EditorSelectableColorInfo selectableAccentColor => EditorSelectableColors.EditorUI.Amber;

        public virtual Texture2D iconTexture => null;
        public virtual List<Texture2D> iconTextures => null;

        public virtual Texture2D secondaryIconTexture => null;
        public virtual List<Texture2D> secondaryIconTextures => null;

        private FluidComponentHeader m_FluidHeader;
        public FluidComponentHeader fluidHeader =>
            m_FluidHeader ??= FluidComponentHeader.Get()
                .AddManualButton()
                .AddYouTubeButton();

        public VisualElement root { get; private set; }

        public override void OnInspectorGUI() {}

        protected virtual void CreateEditor()
        {
            fluidHeader.SetAccentColor(accentColor);
            fluidHeader.SetComponentNameText(typeof(Tclass).Name);

            root =
                new VisualElement()
                    .AddChild(fluidHeader)
                    .AddSpace(0, DesignUtils.k_Spacing);
        }

        private void SetupHeaderIcon()
        {
            if (iconTextures != null)
            {
                fluidHeader.SetIcon(iconTextures);
                return;
            }

            if (iconTexture == null)
                return;

            fluidHeader.SetIcon(iconTexture);
        }

        private void SetupHeaderSecondaryIcon()
        {
            if (secondaryIconTextures != null)
            {
                fluidHeader.SetSecondaryIcon(secondaryIconTextures);
                return;
            }

            if (secondaryIconTexture == null)
                return;

            fluidHeader.SetSecondaryIcon(secondaryIconTexture);
        }

        protected virtual void OnDestroy()
        {
            if (m_Poolables != null)
                foreach (IPoolable poolable in m_Poolables)
                    poolable?.Recycle();

            if (m_Disposables != null)
                foreach (IDisposable disposable in m_Disposables)
                    disposable?.Dispose();
        }

    }
}
