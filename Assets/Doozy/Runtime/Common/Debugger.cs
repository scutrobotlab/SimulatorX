// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using Doozy.Runtime.Common.Utils;
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable UnusedMember.Local

namespace Doozy.Runtime.Common
{
	public static class Debugger
	{
		private static ILogger loggingSolution => new UnityDebug();

		private static ILogger s_logger;
		private static ILogger logger => s_logger ??= loggingSolution;

		private const string ERROR_COLOR_CODE = "#D9534F";
		private const string INFO_COLOR_CODE = "#1C7CD5";
		private const string OK_COLOR_CODE = "#5CB85C";
		private const string WARNING_COLOR_CODE = "#EE9800";

		public enum LogType
		{
			Assert,
			Error,
			Warning,
			Log,
			Exception,
		}

		private static string DoozyPrefix(LogType logType)
		{
			string colorCode = "#121212";
			switch (logType)
			{
				case LogType.Log:
					colorCode = INFO_COLOR_CODE;
					break;
				case LogType.Warning:
					colorCode = WARNING_COLOR_CODE;
					break;
				case LogType.Error:
					colorCode = ERROR_COLOR_CODE;
					break;
				case LogType.Exception:
					colorCode = ERROR_COLOR_CODE;
					break;
				case LogType.Assert:
					colorCode = OK_COLOR_CODE;
					break;
				default: throw new ArgumentOutOfRangeException(nameof(logType), logType, null);
			}

			return $"<color={colorCode}><b>DOOZY ››› </b></color>";
		}

		public static void Log(object message, UnityEngine.Object context = null)
		{
			message = DoozyPrefix(LogType.Log) + message;
			logger.Log(message, context);
		}

		public static void LogWarning(object message, UnityEngine.Object context = null)
		{
			message = DoozyPrefix(LogType.Warning) + message;
			logger.LogWarning(message, context);
		}

		public static void LogError(object message, UnityEngine.Object context = null)
		{
			message = DoozyPrefix(LogType.Error) + message;
			logger.LogError(message, context);
		}
	}
}