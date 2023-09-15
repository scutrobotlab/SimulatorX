// Copyright (c) 2015 - 2022 Doozy Entertainment. All Rights Reserved.
// This code can only be used under the standard Unity Asset Store End User License Agreement
// A Copy of the EULA APPENDIX 1 is available at http://unity3d.com/company/legal/as_terms

using System;
using System.Collections.Generic;
using Doozy.Runtime.Reactor.Easings.Eases;

namespace Doozy.Runtime.Reactor.Easings
{
    public static class EaseFactory
    {
        private static readonly Dictionary<Ease, IEasing> Easings = new Dictionary<Ease, IEasing>();
        
        public static IEasing GetEase(Ease ease)
        {
            if (Easings.ContainsKey(ease)) return Easings[ease];

            IEasing easing = ease switch
                             {
                                 Ease.Linear       => new LinearEasing(),
                                 Ease.Easy         => new EasyEaseEasing(),
                                 Ease.InEasy       => new EasyEaseInEasing(),
                                 Ease.OutEasy      => new EasyEaseOutEasing(),
                                 Ease.InOutEasy    => new EasyEaseInOutEasing(),
                                 Ease.InSine       => new SineEaseInEasing(),
                                 Ease.OutSine      => new SineEaseOutEasing(),
                                 Ease.InOutSine    => new SineEaseInOutEasing(),
                                 Ease.InQuad       => new QuadraticEaseInEasing(),
                                 Ease.OutQuad      => new QuadraticEaseOutEasing(),
                                 Ease.InOutQuad    => new QuadraticEaseInOutEasing(),
                                 Ease.InCubic      => new CubicEaseInEasing(),
                                 Ease.OutCubic     => new CubicEaseOutEasing(),
                                 Ease.InOutCubic   => new CubicEaseInOutEasing(),
                                 Ease.InQuart      => new QuarticEaseInEasing(),
                                 Ease.OutQuart     => new QuarticEaseOutEasing(),
                                 Ease.InOutQuart   => new QuarticEaseInOutEasing(),
                                 Ease.InQuint      => new QuinticEaseInEasing(),
                                 Ease.OutQuint     => new QuinticEaseOutEasing(),
                                 Ease.InOutQuint   => new QuinticEaseInOutEasing(),
                                 Ease.InExpo       => new ExponentialEaseInEasing(),
                                 Ease.OutExpo      => new ExponentialEaseOutEasing(),
                                 Ease.InOutExpo    => new ExponentialEaseInOutEasing(),
                                 Ease.InCirc       => new CircularEaseInEasing(),
                                 Ease.OutCirc      => new CircularEaseOutEasing(),
                                 Ease.InOutCirc    => new CircularEaseInOutEasing(),
                                 Ease.InBack       => new BackEaseInEasing(),
                                 Ease.OutBack      => new BackEaseOutEasing(),
                                 Ease.InOutBack    => new BackEaseInOutEasing(),
                                 Ease.InElastic    => new ElasticEaseInEasing(),
                                 Ease.OutElastic   => new ElasticEaseOutEasing(),
                                 Ease.InOutElastic => new ElasticEaseInOutEasing(),
                                 Ease.InBounce     => new BounceEaseInEasing(),
                                 Ease.OutBounce    => new BounceEaseOutEasing(),
                                 Ease.InOutBounce  => new BounceEaseInOutEasing(),
                                 Ease.Spring       => new SpringEasing(),
                                 _                 => throw new ArgumentOutOfRangeException(nameof(ease), ease, null)
                             };

            Easings.Add(ease, easing);
            return easing;
        }
    }
}
