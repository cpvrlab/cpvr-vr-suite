using TMPro;
using UnityEngine;

namespace cpvr_vr_suite.Scripts.Runtime.UI
{
    /// <summary>
    /// // Keeps the input with numerical string between 0 and 999
    /// </summary>
    public class NumericalInputField : MonoBehaviour
    {
        TMP_InputField _inputField;

        [SerializeField] int maxSize;

        void Start()
        {
            _inputField = GetComponent<TMP_InputField>();

            _inputField.onValidateInput += (_, index, addedChar) =>
            {
                if (index >= maxSize)
                {
                    return '\0';
                }

                if (!char.IsNumber(addedChar))
                {
                    return '\0';
                }

                return addedChar;
            };
        }
    }
}
