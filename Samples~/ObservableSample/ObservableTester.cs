using TMPro;
using UnityEngine;

namespace Calluna.Core.Samples
{
    public class ObservableTester : MonoBehaviour
    {
        [SerializeField] private TMP_InputField _inputField;
        [SerializeField] private TextMeshProUGUI _label;

        private Observable<string> _text = new();

        private void Start()
        {
            _inputField.onValueChanged.AddListener(OnInputChanged);
            _text.OnChangedWithValues += UpdateLabel;
            UpdateLabel(string.Empty, _inputField.text);
        }

        private void OnDestroy()
        {
            _inputField.onValueChanged.RemoveListener(OnInputChanged);
            _text.OnChangedWithValues -= UpdateLabel;
        }

        private void OnInputChanged(string arg0)
        {
            _text.Value = arg0;
        }

        private void UpdateLabel(string formervalue, string newvalue)
        {
            _label.text = newvalue;
        }
    }
}
