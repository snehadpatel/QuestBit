using UnityEngine;
using UnityEngine.UI;

namespace Echo.UI.HUD
{
    public class PlayerHUD : MonoBehaviour
    {
        [Header("HUD Elements")]
        [SerializeField] private Text _threatStatusText;
        [SerializeField] private Text _interactionPromptText;
        [SerializeField] private Image _crosshairImage;

        public void SetThreatLevelText(string threatText, Color statusColor)
        {
            if (_threatStatusText != null)
            {
                _threatStatusText.text = threatText;
                _threatStatusText.color = statusColor;
            }
        }

        public void ShowInteractionPrompt(string prompt)
        {
            if (_interactionPromptText != null)
            {
                _interactionPromptText.text = prompt;
                _interactionPromptText.gameObject.SetActive(true);
            }
        }

        public void HideInteractionPrompt()
        {
            if (_interactionPromptText != null)
            {
                _interactionPromptText.gameObject.SetActive(false);
            }
        }
    }
}
