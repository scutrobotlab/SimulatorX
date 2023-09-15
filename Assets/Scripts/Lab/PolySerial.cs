using Gameplay.Events;
using Gameplay.Attribute;
using Infrastructure;
using Misc;
using UnityEngine;

namespace Lab
{
    public class PolySerial : MonoBehaviour
    {
        private void Start()
        {
            Debug.Log(PolymorphicSerializer.Serialize(new AddBullet
            {
                Receiver = new Identity(),
                Type = MechanicType.CaliberType.Large
            }));
            Debug.Log(JsonUtility.ToJson(new Identity(), true));
        }
    }
}