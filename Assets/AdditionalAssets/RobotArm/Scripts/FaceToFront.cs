using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ArmBehavior
{
    public class FaceToFront : MonoBehaviour
    {
        public Transform arm;
        private ArmDig _relateTo;

        private void Start()
        {
            _relateTo = GetComponent<RobotArmBehavior>().dig;
        }

        void Update()
        {
            if(!_relateTo.ArmOn)
                transform.rotation = arm.rotation;
        }

    }
}

