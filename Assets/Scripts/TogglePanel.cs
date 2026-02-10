using UnityEngine;
using UnityEngine.UI;

public class TogglePanel : MonoBehaviour
{
    public GameObject panel;      // Assign your Panel here in Inspector
    public Button toggleButton;   // Assign your Button here in Inspector

    private bool isPanelOpen = false;

    void Start()
    {
        toggleButton.onClick.AddListener(TogglePanelVisibility);
        panel.SetActive(isPanelOpen);  // Optional: initialize panel hidden
    }

    void TogglePanelVisibility()
    {
        isPanelOpen = !isPanelOpen;
        panel.SetActive(isPanelOpen);
    }
}