using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Michsky.UI.Shift
{
    [ExecuteInEditMode]
    public class EditorDemoPositionFix : MonoBehaviour
    {
        public List<RectTransform> objectToRepaint;

        private void Awake()
        {
            if (Application.isPlaying == true || objectToRepaint.Count == 0)
                return;

            // Rebuilding the rect in case of incorrect layout calculation
            foreach (var t in objectToRepaint.Where(t => t != null))
            {
                LayoutRebuilder.ForceRebuildLayoutImmediate(t.GetComponentInParent<RectTransform>());
                LayoutRebuilder.ForceRebuildLayoutImmediate(t);
                LayoutRebuilder.ForceRebuildLayoutImmediate(t);
            }
        }
    }
}