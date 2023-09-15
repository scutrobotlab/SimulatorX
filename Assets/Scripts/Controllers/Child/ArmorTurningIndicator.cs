using System.Collections.Generic;
using System.Linq;
using Gameplay.Events.Child;
using Infrastructure.Child;
using UnityEngine;
namespace Controllers.Child
{
    public class ArmorTurningIndicator : StoreChildBase
    {
        public Material on;
        public Material off;

        protected override void Identify()
        {
            id = new ChildIdentity(ChildType.ArmorTurningIndicator);
        }

        public override List<string> InputActions()
        {
            return base.InputActions().Concat(new List<string>
            {
                ChildActionID.Indicator.ArmorTurningStatus
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
                case ChildActionID.Indicator.ArmorTurningStatus:
                    var armorTurningStatusAction = (ArmorTurningStatus) action;
                    GetComponent<Renderer>().material = armorTurningStatusAction.ArmorTurning ? on : off;
                    break;
            }
        }
    }
}