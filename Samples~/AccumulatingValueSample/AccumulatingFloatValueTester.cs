using System.Collections.Generic;
using System.Globalization;
using TMPro;
using UnityEngine;

namespace Calluna.Core.AccumulatingValueSample
{
    public class AccumulatingFloatValueTester : MonoBehaviour
    {
        [SerializeField] private List<TMP_InputField> _inputFields;
        [SerializeField] private TextMeshProUGUI _addUpValueText;
        [SerializeField] private TextMeshProUGUI _multiplyValueText;

        private AccumulatingFloatValue _addUpValue;
        private AccumulatingFloatValue _multiplyValue;
        private CultureInfo _cultureInfo;

        private void Awake()
        {
            _addUpValue = new AccumulatingFloatValue();
            _multiplyValue = new AccumulatingFloatValue(AccumulatingFloatValue.Mode.Multiply);
            _cultureInfo = new CultureInfo("en");
        }

        private void Start()
        {
            foreach (TMP_InputField inputField in _inputFields)
            {
                UpdateIntValue(inputField, inputField.text);
                inputField.onSubmit.AddListener(s => UpdateIntValue(inputField, s));
            }
        }

        private void OnDestroy()
        {
            foreach (TMP_InputField inputField in _inputFields)
            {
                inputField.onSubmit.RemoveAllListeners();
            }
        }

        private void UpdateIntValue(TMP_InputField inputField, string value)
        {
            if (inputField.text.Contains(","))
                inputField.SetTextWithoutNotify(value.Replace(',', '.'));
            float num = float.Parse(inputField.text, _cultureInfo);
            _addUpValue[inputField.GetInstanceID().ToString()] = num;
            _addUpValueText.text = $"Add Up Value: {_addUpValue.Value.Value.ToString(_cultureInfo)}";
            _multiplyValue[inputField.GetInstanceID().ToString()] = num;
            _multiplyValueText.text = $"Multiply Value: {_multiplyValue.Value.Value.ToString(_cultureInfo)}";
        }
    }
}
