using System;
using UnityEngine;
using UnityEngine.UI;

namespace Sja_UI
{
    class Sja_InputFieldString : MonoBehaviour
    {
        // You will want to edit the name. This is the name the VNyanParameter will have in VNyan!
        public string fieldName;
        // If no other string was set we have the field set to "nothingHere". 
        public string fieldValue = "nothingHere";
        private InputField mainField;
        private Button mainButton;

        public void Start()
        {
            // We add the inputfield as the mainfield
            mainField = GetComponent(typeof(InputField)) as InputField;
            // We add a button as confirmation to change the inputted string
            mainButton = GetComponentInChildren(typeof(Button)) as Button;

            // We add a listener that will run ButtonPressCheck if the button is pressed.
            mainButton.onClick.AddListener(delegate { ButtonPressCheck(); });

            // Here we either want to load a existing parameter or add one!
            // A loaded parameter comes from Sja_UICore when it loads the setting Json into the dictionary!
            if (Sja_UICore.VNyanParameters.ContainsKey(fieldName))
            {
                // If the parameter exist, set fieldValue to that string.
                fieldValue = Sja_UICore.VNyanParameters[fieldName];
                // We want to try to show the user the current string.
                // We set the field text to it.
                mainField.text = fieldValue;
                // Lastly we set the VNyanParameter to this value!
                VNyanInterface.VNyanInterface.VNyanParameter.setVNyanParameterString(fieldName, fieldValue);
            }
            else
            {
                // If it was the first time and there is no parameter we want to add it!
                // field name is the name of the parameter and fieldvalue will be the string.
                Sja_UICore.VNyanParameters.Add(fieldName, fieldValue);
                // We want to try to show the value at all times.
                mainField.text = fieldValue;
                // Set the Vnyan parameter to this name and value!
                VNyanInterface.VNyanInterface.VNyanParameter.setVNyanParameterString(fieldName, fieldValue);
            }
        }

        public void ButtonPressCheck()
        {
            // Get the new string!
            fieldValue = mainField.text;
            // Set the new string!
            Sja_UICore.VNyanParameters[fieldName] = fieldValue;
            // Set the VNyanparameter!
            VNyanInterface.VNyanInterface.VNyanParameter.setVNyanParameterString(fieldName, fieldValue);
        }
    }
}
