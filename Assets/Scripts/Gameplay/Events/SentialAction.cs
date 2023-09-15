using Gameplay.Attribute;
using Infrastructure;
using UnityEngine;

namespace Infrastructure
{
    public static partial class ActionID
    {
        public static class SentialControl
        {
            public const string StopSential = "StopSential";
            public const string ChangeSential = "ChangeSential";
        }
    }
}

namespace Gameplay.Events
{
    public class StopSential : IAction
    {
        public string ActionName() => ActionID.SentialControl.StopSential;
        public Identity.Camps Camp;
        public bool stop;
    }
    public class ChangeSential : IAction
    {
        public string ActionName() => ActionID.SentialControl.ChangeSential;
        public Identity.Camps Camp;
        
    }
}