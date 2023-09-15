using System.Collections.Generic;
using System.Linq;
using Gameplay.Events.Child;
using Infrastructure.Child;
using UnityEngine;

namespace Controllers.Child
{
    public class BulletproofIndicator : StoreChildBase
    {
        public Material on;
        public Material off;

        protected override void Identify()
        {
            id = new ChildIdentity(ChildType.BulletproofIndicator);
        }

        public override List<string> InputActions()
        {
            return base.InputActions().Concat(new List<string>
            {
                ChildActionID.Indicator.BulletproofStatus
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
                case ChildActionID.Indicator.BulletproofStatus:
                    var bulletproofStatusAction = (BulletproofStatus) action;
                    GetComponent<Renderer>().material = bulletproofStatusAction.bulletproof ? on : off;
                    break;
            }
        }
    }
}