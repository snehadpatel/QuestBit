using System;
using UnityEngine;

namespace Echo.Core.Input
{
    public class SwitchScanController : MonoBehaviour
    {
        [Header("Scan Parameters")]
        [SerializeField] private float _scanInterval = 1.0f; // Target <16.6ms window for response per GDD Ch. 15
        [SerializeField] private bool _autoScanEnabled = true;

        private int _currentIndex = 0;
        private int _elementCount = 0;
        private float _scanTimer = 0f;

        public event Action<int> OnScanElementHighlighted;
        public event Action<int> OnScanElementSelected;

        public void InitializeScan(int elementCount)
        {
            _elementCount = elementCount;
            _currentIndex = 0;
            _scanTimer = 0f;
            if (_elementCount > 0)
            {
                OnScanElementHighlighted?.Invoke(_currentIndex);
            }
        }

        private void Update()
        {
            if (!_autoScanEnabled || _elementCount <= 0) return;

            _scanTimer += Time.deltaTime;
            if (_scanTimer >= _scanInterval)
            {
                _scanTimer = 0f;
                _currentIndex = (_currentIndex + 1) % _elementCount;
                OnScanElementHighlighted?.Invoke(_currentIndex);
            }
        }

        public void TriggerSelect()
        {
            if (_elementCount > 0)
            {
                OnScanElementSelected?.Invoke(_currentIndex);
            }
        }
    }
}
