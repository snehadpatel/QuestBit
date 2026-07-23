using Echo.Core.Input;
using UnityEngine;
using UnityEngine.UI;

namespace Echo.UI.Accessibility
{
    public class SwitchScanUIOverlay : MonoBehaviour
    {
        [Header("UI Highlight Settings")]
        [SerializeField] private SwitchScanController _scanController;
        [SerializeField] private Image[] _selectableElements;
        [SerializeField] private Color _highlightColor = Color.yellow;
        [SerializeField] private Color _normalColor = Color.white;

        private void OnEnable()
        {
            if (_scanController != null)
            {
                _scanController.OnScanElementHighlighted += HighlightElement;
                _scanController.OnScanElementSelected += SelectElement;
                
                if (_selectableElements != null && _selectableElements.Length > 0)
                {
                    _scanController.InitializeScan(_selectableElements.Length);
                }
            }
        }

        private void OnDisable()
        {
            if (_scanController != null)
            {
                _scanController.OnScanElementHighlighted -= HighlightElement;
                _scanController.OnScanElementSelected -= SelectElement;
            }
        }

        private void HighlightElement(int index)
        {
            for (int i = 0; i < _selectableElements.Length; i++)
            {
                if (_selectableElements[i] != null)
                {
                    _selectableElements[i].color = (i == index) ? _highlightColor : _normalColor;
                }
            }
        }

        private void SelectElement(int index)
        {
            if (index >= 0 && index < _selectableElements.Length && _selectableElements[index] != null)
            {
                Debug.Log($"[SwitchScanUI] Selected UI Element index: {index} ({_selectableElements[index].gameObject.name})");
                var button = _selectableElements[index].GetComponent<Button>();
                button?.onClick.Invoke();
            }
        }
    }
}
