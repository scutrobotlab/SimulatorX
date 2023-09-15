// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

namespace Doozy.Runtime.Reactor.Easings
{
    public interface IEasing
    {
        /// <summary> Evaluate the easing function at the given progress </summary>
        /// <param name="progress"> Value clamped between 0 and 1 </param>
        float Evaluate(float progress);
    }
}
