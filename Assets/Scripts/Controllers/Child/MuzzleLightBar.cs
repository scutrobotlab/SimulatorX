using System.Collections.Generic;
using System.Linq;
using Gameplay.Events.Child;
using Infrastructure;
using Infrastructure.Child;
using UnityEngine;


namespace Controllers.Child
{
    public class MuzzleLightBar : StoreChildBase
    {
        public Material materialTemplate;
        public List<Renderer> lights = new List<Renderer>();
        private List<float> _bullets = new List<float>();

        // 名称美化
        private static readonly int Bullets = Shader.PropertyToID("Vector4_a5c8b536a2db4be8b1040547a01fef96");
        private static readonly int HeatPercentage = Shader.PropertyToID("Vector1_dc429394840b4f9c83d1a22037874577");
        private static readonly int MainColor = Shader.PropertyToID("Color_2fef8bbf64384b4c8785dbf953558948");

        protected override void Identify()
        {
            id = new ChildIdentity(ChildType.MuzzleLightBar);
        }

        public override List<string> InputActions()
        {
            return base.InputActions().Concat(new List<string>
            {
                ChildActionID.Muzzle.Fire,
                ChildActionID.Muzzle.HeatStatus
            }).ToList();
        }

        protected override void Start()
        {
            base.Start();
            foreach (var lightBar in lights)
            {
                var material = new Material(materialTemplate);
                material.SetVector(Bullets, Vector4.one * -1);
                material.SetFloat(HeatPercentage, 0.3f);
                material.SetColor(MainColor, Color.green);
                lightBar.material = material;
            }
        }

        protected override void StartWithRoot()
        {
            base.StartWithRoot();
            foreach (var lightBar in lights)
            {
                lightBar.material.SetColor(MainColor, Root.id.camp == Identity.Camps.Red ? Color.red : Color.blue);
            }
        }

        public override void Receive(IChildAction action)
        {
            base.Receive(action);
            switch (action.ActionName())
            {
                case ChildActionID.Muzzle.Fire:
                    _bullets.Add(1);
                    break;

                case ChildActionID.Muzzle.HeatStatus:
                    var heatAction = (HeatStatus) action;
                    foreach (var lightBar in lights)
                    {
                        lightBar.material.SetFloat(HeatPercentage, heatAction.percentage);
                    }

                    break;
            }
        }

        protected override void FixedUpdate()
        {
            base.FixedUpdate();
            for (var i = 0; i < _bullets.Count; i++)
            {
                _bullets[i] -= Time.fixedDeltaTime * 3;
                if (_bullets[i] < 0) _bullets.RemoveAt(i);
            }

            var bulletData = Vector4.one * -1;
            if (_bullets.Count > 0) bulletData.x = _bullets[0];
            if (_bullets.Count > 1) bulletData.y = _bullets[1];
            if (_bullets.Count > 2) bulletData.z = _bullets[2];
            if (_bullets.Count > 3) bulletData.w = _bullets[3];
            foreach (var lightBar in lights)
            {
                lightBar.material.SetVector(Bullets, bulletData);
            }
        }
    }
}