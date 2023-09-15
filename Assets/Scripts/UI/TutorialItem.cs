using System.Collections;
using System.Linq;
using Gameplay;
using Infrastructure;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

namespace UI
{
    public class TutorialItem : MonoBehaviour
    {
        public Identity.Roles[] roles;
        public int serial;
        public TMP_Text keyLayer;
        public InputActionReference actionReference;

        private IEnumerator Start()
        {
            yield return new WaitUntil(() => EntityManager.Instance()?.initialized == true);
            var id = EntityManager.Instance().LocalRobot();
            if (id != null && (roles[0] == Identity.Roles.Nothing || roles.Contains(id.role)) && (serial == -1 || serial == id.serial))
            {
                var displayString = string.Empty;

                // Get display string from action.
                var action = actionReference.action;
                if (action != null)
                {
                    var bindingIndex = action.GetBindingIndexForControl(action.controls[0]);
                    if (bindingIndex != -1)
                        displayString = action.GetBindingDisplayString(bindingIndex);
                }
                
                keyLayer.text = displayString;
                yield break;
            }
            
            gameObject.SetActive(false);
        }
    }
}