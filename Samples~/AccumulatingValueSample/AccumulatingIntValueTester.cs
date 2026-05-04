using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Calluna.Core.AccumulatingValueSample
{
    public class AccumulatingIntValueTester : MonoBehaviour
    {
        [SerializeField] private List<TMP_InputField> _inputFields;
        [SerializeField] private TextMeshProUGUI _valueText;

        private AccumulatingIntValue _value;

        private void Awake()
        {
            _value = new AccumulatingIntValue();
        }

        private void Start()
        {
            foreach (TMP_InputField inputField in _inputFields)
            {
                UpdateIntValue(inputField, int.Parse(inputField.text));
                inputField.onSubmit.AddListener(s => UpdateIntValue(inputField, int.Parse(s)));
            }
        }

        private void OnDestroy()
        {
            foreach (TMP_InputField inputField in _inputFields)
            {
                inputField.onSubmit.RemoveAllListeners();
            }
        }

        private void UpdateIntValue(TMP_InputField inputField, int value)
        {
            _value[inputField.GetInstanceID().ToString()] = value;
            _valueText.text = $"Value: {_value.Value.Value}";
        }
    }
}
