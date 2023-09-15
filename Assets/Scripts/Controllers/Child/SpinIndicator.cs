using System.Collections.Generic;
using System.Linq;
using Gameplay.Events.Child;
using Infrastructure.Child;
using UnityEngine;

namespace Controllers.Child
{
    public class SpinIndicator : StoreChildBase
    {
        public Material on;
        public Material off;

        protected override void Identify()
        {
            id = new ChildIdentity(ChildType.SpinIndicator);
        }

        public override List<string> InputActions()
        {
            return base.InputActions().Concat(new List<string>
            {
                ChildActionID.Indicator.SpinStatus
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
                case ChildActionID.Indicator.SpinStatus:
                    var spinStatusAction = (SpinStatus) action;
                    GetComponent<Renderer>().material = spinStatusAction.spinning ? on : off;
                    break;
            }
        }
    }
}