using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ArmBehavior
{

        // Start is called before the first frame update
        [Serializable]
        public class  ArmDig
        {
            public enum Direction
            {
                X,
                Y,
                Z,
                unsure
            };
        

            [SerializeField] public Direction RotateAxis;
            [SerializeField] public float CurrenteAngle;
            [SerializeField] public float ConstrainLow;
            [SerializeField] public float ConstrainHigh;
            [SerializeField] public float target;
            [SerializeField] public bool ArmOn;
        
            public ArmDig()
            {
                RotateAxis = Direction.unsure;
                CurrenteAngle = 0;
                ConstrainLow = 0;
                ConstrainHigh = 0;
                target = 0;
                ArmOn = true;
            }

            public ArmDig(Direction dir, float RA, float CL, float CH,float TA,bool AO)
            {
                RotateAxis = dir;
                CurrenteAngle = RA;
                ConstrainLow = CL;
                ConstrainHigh = CH;
                target = TA;
                ArmOn = AO;
            }
        }
    }

    

