using System;
using UnityEngine;

namespace Misc
{
    [Serializable]
    public class SerializedPolymorphicObject
    {
        public string fullTypeName;
        public string serializedJson;
    }

    public static class PolymorphicSerializer
    {
        public static string Serialize(object value)
        {
            var typeName = value.GetType().FullName;
            if (typeName == null)
            {
                throw new Exception("Null type name.");
            }

            return JsonUtility.ToJson(
                new SerializedPolymorphicObject
                {
                    fullTypeName = typeName,
                    serializedJson = JsonUtility.ToJson(value)
                });
        }

        public static object Deserialize(string value)
        {
            var polyObject = JsonUtility.FromJson<SerializedPolymorphicObject>(value);

            if (polyObject.fullTypeName == null)
            {
                throw new Exception("Null type name.");
            }

            var type = Type.GetType(polyObject.fullTypeName);
            return JsonUtility.FromJson(polyObject.serializedJson, type);
        }
    }
}