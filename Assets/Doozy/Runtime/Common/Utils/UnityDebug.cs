// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using UnityEngine;

namespace Doozy.Runtime.Common.Utils
{
	public class UnityDebug : ILogger
	{
		public void Log(object message) =>
			Debug.Log(message);

		public void Log(object message, Object context) =>
			Debug.Log(message, context);

		public void LogWarning(object message) =>
			Debug.LogWarning(message);

		public void LogWarning(object message, Object context) =>
			Debug.Log(message, context);

		public void LogError(object message) =>
			Debug.Log(message);

		public void LogError(object message, Object context) =>
			Debug.Log(message, context);
	}
}