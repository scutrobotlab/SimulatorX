using System;
using System.Collections;
using System.Collections.Generic;
using Controllers;
using UnityEngine;

namespace Controllers
{
    public class OreTrigger : MonoBehaviour
    {
        public ExchangeStore triggerBack;

        private void OnTriggerEnter(Collider other)
        {
            triggerBack.OnTriggerEnter(other);
        }

    }
}
