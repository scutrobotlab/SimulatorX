using System.Collections.Generic;
using System.Linq;
using Gameplay.Events.Child;
using Infrastructure.Child;
using UnityEngine;
namespace Controllers.Child
{
    public class SuperBatteryIndicator : StoreChildBase
    {
        public Material on;
        public Material off;

        protected override void Identify()
        {
            id = new ChildIdentity(ChildType.SuperBatteryIndicator);
        }

        public override List<string> InputActions()
        {
            return base.InputActions().Concat(new List<string>
            {
                ChildActionID.Indicator.SuperBatteryStatus
            }).ToList();
        }

        protected override void Start()
        {
            base.Start();
            GetComponent<Renderer>().material = on;
        }

        public override void Receive(IChildAction action)
        {
            base.Receive(action);
            switch (action.ActionName())
            {
                case ChildActionID.Indicator.SuperBatteryStatus:
                    var superBatteryStatusAction = (SuperBatteryStatus) action;
                    GetComponent<Renderer>().material = superBatteryStatusAction.supperBattery ? on : off;
                    break;
            }
        }
    }
}