using System.Collections;
using System.Collections.Generic;
using TMPro;
using UltraMan.Ui.ButtonConditions;
using UnityEngine;

namespace UltraMan
{
    public class DisableOnEmptyInputButtonCondition : ButtonCondition
    {
        public override void OnTextChanged(TMP_InputField inputField)
        {
            if(button != null && inputField != null)
                button.interactable = !string.IsNullOrEmpty(inputField.text);
        }
    }
}
