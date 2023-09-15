// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using Doozy.Runtime.UIElements.Extensions;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace Doozy.Editor.UIElements
{
	public static class BindableElementExtensions
	{

		/// <summary>
		/// Query the target's children for a Label. If one is found, schedule its DisplayStyle to None
		/// <para/> Useful to hide a PropertyField's label
		/// </summary>
		/// <param name="target"> BindableElement </param>
		public static T TryToHideLabel<T>(this T target) where T : VisualElement, IBindable
		{
			target.visible = false;
			target.schedule.Execute(() =>
			{
				target.Q<Label>()?.SetStyleDisplay(DisplayStyle.None);
				target.visible = true;
			});
			return target;
		}
		
		#region Bind / Unbind
		
		/// <summary> Bind a SerializedObject to fields in the element hierarchy </summary>
		/// <param name="target"> Target BindableElement </param>
		/// <param name="serializedObject"> Data object </param>
		public static T BindToSerializedObject<T>(this T target, SerializedObject serializedObject) where T : BindableElement
		{
			target.Bind(serializedObject);
			return target;
		}

		/// <summary> Bind a property to a field and synchronize their values </summary>
		/// <param name="target"> Target BindableElement </param>
		/// <param name="property"> The SerializedProperty to bind </param>
		public static T BindToProperty<T>(this T target, SerializedProperty property) where T : BindableElement
		{
			target.BindProperty(property);
			return target;
		}

		/// <summary> Disconnect all properties bound to fields in the element's hierarchy </summary>
		/// <param name="target"> Target BindableElement </param>
		public static T UnbindElement<T>(this T target) where T : BindableElement
		{
			target.Unbind();
			return target;
		}
		
		#endregion

		#region BindingPath
		
		// /// <summary> Set the path to the target property to be bound </summary>
		// /// <param name="target"> Target BindableElement </param>
		// /// <param name="bindingPath"> Path to the target property </param>
		// public static T SetBindingPath<T>(this T target, string bindingPath) where T : BindableElement
		// {
		// 	target.bindingPath = bindingPath;
		// 	return target;
		// }
		
		/// <summary> Set the path to the target property to be bound </summary>
		/// <param name="target"> Target BindableElement </param>
		/// <param name="bindingPath"> Path to the target property </param>
		public static T SetBindingPath<T>(this T target, string bindingPath) where T : IBindable
		{
			target.bindingPath = bindingPath;
			return target;
		}

		/// <summary> Get the path to the target property to be bound </summary>
		/// <param name="target"> Target BindableElement </param>
		public static string GetBindingPath<T>(this T target) where T : IBindable => target.bindingPath;
		
		#endregion
	}
}