// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using UnityEngine;
// ReSharper disable UnusedMember.Global

namespace Doozy.Runtime.Common
{
	public interface ILogger
	{
		void Log(object message);
		void Log(object message, Object context);
		void LogWarning(object message);
		void LogWarning(object message, Object context);
		void LogError(object message);
		void LogError(object message, Object context);
	}
}