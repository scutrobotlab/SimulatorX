using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Misc
{
    public static class RotateTweenHelper
    {
        private static readonly HashSet<Transform> Transforms = new HashSet<Transform>();

        public static bool Rotating(Transform transform)
        {
            return Transforms.Contains(transform);
        }

        public static IEnumerator RotateAroundLocal(Transform transform, Vector3 axis, float degree, float duration)
        {
            if (Transforms.Contains(transform)) yield break;
            Transforms.Add(transform);
            var step = degree / (duration / 0.01f);
            for (var i = 0; i < duration / 0.01f; i++)
            {
                transform.Rotate(axis, step, Space.Self);
                yield return new WaitForSeconds(0.01f);
            }

            Transforms.Remove(transform);
        }
    }
}