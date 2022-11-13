using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UltraMan.Ui.ButtonConditions
{
    [RequireComponent(typeof(Button))]
    public abstract class ButtonCondition : MonoBehaviour
    {
        protected Button button;

        void Start()
        {
            button = GetComponent<Button>();
        }

        public abstract void OnTextChanged(TMP_InputField inputField);
    }
}
