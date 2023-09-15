using UnityEngine;

namespace Misc
{
    public static class IfVisualOnTarget
    {
        /// <summary>
        /// 判断是否能看到目标物体。
        /// </summary>
        /// <param name="cam">相机</param>
        /// <param name="target">目标</param>
        /// <returns></returns>
        public static bool HasVisualOnTarget(Camera cam, GameObject target, bool rayCheck = true)
        {
            var targetInViewport = cam.WorldToViewportPoint(target.transform.position);
            if (targetInViewport.x < 0 || targetInViewport.x > 1) return false;
            if (targetInViewport.y < 0 || targetInViewport.y > 1) return false;
            if (targetInViewport.z < cam.nearClipPlane || targetInViewport.z > cam.farClipPlane) return false;
            if (!rayCheck) return true;
            if (!Physics.Raycast(
                    new Ray(
                        cam.transform.position,
                        target.transform.position - cam.transform.position),
                    out var hit)) return false;
            return hit.collider.gameObject == target;
        }
    }
}