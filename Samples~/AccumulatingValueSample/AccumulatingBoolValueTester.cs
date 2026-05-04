using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Calluna.Core.AccumulatingValueSample
{
    public class AccumulatingBoolValueTester : MonoBehaviour
    {
        [SerializeField] private List<Toggle> _toggles;
        [SerializeField] private TextMeshProUGUI _anyText;
        [SerializeField] private TextMeshProUGUI _allText;

        private AccumulatingBoolValue _anyBoolValue;
        private AccumulatingBoolValue _allBoolValue;

        private void Awake()
        {
            _anyBoolValue = new AccumulatingBoolValue(AccumulatingBoolValue.Mode.Any);
            _allBoolValue = new AccumulatingBoolValue(AccumulatingBoolValue.Mode.All);
        }

        private void Start()
        {
            foreach (Toggle toggle in _toggles)
            {
                UpdateBoolValue(toggle, toggle.isOn);
                toggle.onValueChanged.AddListener(v => UpdateBoolValue(toggle, v));
            }
        }

        private void OnDestroy()
        {
            foreach (Toggle toggle in _toggles)
            {
                toggle.onValueChanged.RemoveAllListeners();
            }
        }

        private void UpdateBoolValue(Toggle toggle, bool value)
        {
            _anyBoolValue[toggle.GetInstanceID().ToString()] = value;
            _anyText.text = $"Any: {_anyBoolValue.Value.Value}";
            _allBoolValue[toggle.GetInstanceID().ToString()] = value;
            _allText.text = $"All: {_allBoolValue.Value.Value}";
        }
    }
}
