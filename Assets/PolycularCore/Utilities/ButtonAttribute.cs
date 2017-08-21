using UnityEngine;

namespace Polycular.Utilities
{
    [System.AttributeUsage(System.AttributeTargets.Field)]
    public class ButtonAttribute : PropertyAttribute
    {
        public bool PlayModeOnly { get; set; }

        public string FunctionName { get; set; }

        public string ButtonName { get; set; }

        public ButtonAttribute(bool playModeOnly, string functionName, string buttonName = null)
        {
            PlayModeOnly = playModeOnly;
            FunctionName = functionName;

            if (buttonName == null)
                buttonName = functionName;

            ButtonName = buttonName;
        }
    }
}