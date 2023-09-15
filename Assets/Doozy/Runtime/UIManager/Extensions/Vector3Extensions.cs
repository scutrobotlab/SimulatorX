// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using UnityEngine;

namespace Doozy.Runtime.UIManager.Extensions
{
    public static class Vector3Extensions
    {
        /// <summary> Get new Vector3(x, y, 1f) </summary>
        /// <param name="target"> Target Vector3 </param>
        public static Vector3 SetZToOne(this Vector3 target) =>
            new Vector3(target.x, target.y, 1f);
        
        /// <summary> Get new Vector3(x, y, 0f) </summary>
        /// <param name="target"> Target Vector3 </param>
        public static Vector3 SetZToZero(this Vector3 target) =>
            new Vector3(target.x, target.y, 0f);
    }
}
