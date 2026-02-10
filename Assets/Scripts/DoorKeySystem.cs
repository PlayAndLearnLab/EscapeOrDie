using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DoorKeySystem : MonoBehaviour
{
    public string sceneToLoad;
    public bool isCorrectDoor = false;
    public float interactionDistance = 2f;
    public SceneFader sceneFader;
    public Transform player;

    [Header("UI")]
    public TextMeshProUGUI messageText;
    public GameObject messagePanel;

    void Start()
    {
        if (messagePanel != null)
            messagePanel.SetActive(false); // Hide UI at start
    }

    public void CheckKeyFromInventory()
    {
        // This method can be called after picking up a key
        if (PlayerHasKey())
        {
            Debug.Log("Key detected in inventory for this door.");
        }
    }

    bool PlayerHasKey()
    {
        InventorySystem inventory = FindFirstObjectByType<InventorySystem>();
        if (inventory != null)
        {
            foreach (Transform slot in inventory.inventoryPanel.transform)
            {
                Image icon = slot.Find("Icon").GetComponent<Image>();
                if (icon != null && icon.enabled && icon.sprite == inventory.keyIcon)
                {
                    return true;
                }
            }
        }
        return false;
    }

    void Update()
    {
        if (Vector3.Distance(player.position, transform.position) <= interactionDistance)
        {
            if (Input.GetKeyDown(KeyCode.O))
            {
                TryOpenDoor();
            }
        }
    }

    void TryOpenDoor()
    {
        if (!PlayerHasKey())
        {
            ShowMessage("This door is locked.");
        }
        else
        {
            if (isCorrectDoor)
            {
                ShowMessage("Door unlocked.");
                sceneFader.FadeToScene(sceneToLoad);
            }
            else
            {
                ShowMessage("This key cannot open this door.");
            }
        }
    }

    void ShowMessage(string text)
    {
        messageText.text = text;
        messagePanel.SetActive(true);
        CancelInvoke(nameof(HideMessage));
        Invoke(nameof(HideMessage), 2f);
    }

    void HideMessage()
    {
        messagePanel.SetActive(false);
    }
}
