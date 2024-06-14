using System;
using TMPro;
using UnityEngine;

namespace UI {
    /// <summary>
    /// // Keeps the input with numerical string between 0 and 999
    /// </summary>
    public class NumericalInputField : MonoBehaviour {
        private TMP_InputField _inputField;

        [SerializeField] private int maxSize;

        private void Start() {
            _inputField = GetComponent<TMP_InputField>();

            _inputField.onValidateInput += (_, index, addedChar) => {
                if (index >= maxSize) {
                    return '\0';
                }
                
                if (!Char.IsNumber(addedChar)) {
                    return '\0';
                }
                
                return addedChar;
            };
        }
    }
}
