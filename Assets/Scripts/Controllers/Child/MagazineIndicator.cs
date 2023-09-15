using System.Collections.Generic;
using System.Linq;
using Gameplay.Events.Child;
using Infrastructure.Child;
using UnityEngine;
namespace Controllers.Child
{
    public class MagazineIndicator : StoreChildBase
    {
        public Material on;
        public Material off;

        protected override void Identify()
        {
            id = new ChildIdentity(ChildType.MagazineIndicator);
        }

        public override List<string> InputActions()
        {
            return base.InputActions().Concat(new List<string>
            {
                ChildActionID.Indicator.MagazineStatus
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
                case ChildActionID.Indicator.MagazineStatus:
                    var magazineStatusAction = (MagazineStatus) action;
                    GetComponent<Renderer>().material = magazineStatusAction.magazine ? on : off;
                    break;
            }
        }
    }
}