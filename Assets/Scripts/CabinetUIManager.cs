using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static StairTrigger;

public class CabinetUIManager : MonoBehaviour
{
    public enum ItemType { Cheese, Battery, Key }

    [System.Serializable]
    public class CabinetItemSlot
    {
        public ItemType itemType;
        public Image icon;
    }

    public GameObject cabinetUI;
    public GameObject KillerRobots;
    public GameObject slotPrefab;
    public Transform slotContainer;
    public Sprite cheeseIcon;
    public Sprite batteryIcon;

    public Button closeButton;
    public Button transferToCabinetButton;
    public Button transferAllToBagButton;
    public Button transferOneButton;

    public int maxCabinetCapacity = 10;

    private List<CabinetItemSlot> cabinetSlots = new List<CabinetItemSlot>();
    private InventorySystem playerInventory;
    private bool lockedAfterTransfer = false;

    void Start()
    {
        playerInventory = FindObjectOfType<InventorySystem>();

        // Subscribe to bag pickup
        if (playerInventory != null)
            playerInventory.OnBagAcquired += EnableTransferButton;

        if (transferAllToBagButton != null)
            transferAllToBagButton.onClick.AddListener(TransferToInventory);

        if (transferOneButton != null)
            transferOneButton.onClick.AddListener(TransferOneToInventory);

        if (closeButton != null)
            closeButton.onClick.AddListener(OnExitClicked);

        // Disable button initially if no bag
        if (transferAllToBagButton != null)
            transferAllToBagButton.interactable = playerInventory.HasBag();
    }

    public void EnableTransferButton()
    {
        if (transferAllToBagButton != null)
            transferAllToBagButton.interactable = true;
    }

    private void UpdateBagButtonState()
    {
        if (transferAllToBagButton != null)
        {
            bool canUse = playerInventory.HasBag();
            transferAllToBagButton.interactable = canUse;
            Debug.Log($"[CabinetUIManager] Transfer button interactable = {canUse}");
        }
    }

    public void ToggleCabinetUI(bool isActive)
    {
        cabinetUI.SetActive(isActive);
    }

    public void TransferToCabinet()
    {
        if (lockedAfterTransfer) return;

        int added = 0;

        while (cabinetSlots.Count < maxCabinetCapacity)
        {
            if (playerInventory.RemoveItem(ItemType.Cheese))
                AddCabinetItem(ItemType.Cheese);
            else if (playerInventory.RemoveItem(ItemType.Battery))
                AddCabinetItem(ItemType.Battery);
            else if (playerInventory.RemoveItem(ItemType.Key))
                AddCabinetItem(ItemType.Key); // <-- handle keys
            else
                break;
        }

        Debug.Log($"Transferred {added} items to cabinet.");
    }

    public void TransferToInventory()
    {
        Debug.Log("TransferToInventory() called. HasBag = " + playerInventory.HasBag());

        var killerRobotManager = FindObjectOfType<KillerRobotManager>();
        Debug.Log("KillerRobotManager found? " + (killerRobotManager != null));

        if (!playerInventory.HasBag())
        {
            Debug.LogWarning("Tried to transfer to inventory without a bag.");
            return;
        }

        foreach (var slot in cabinetSlots)
        {
            if (slot.itemType == ItemType.Cheese)
                playerInventory.AddCheese();
            else if (slot.itemType == ItemType.Battery)
                playerInventory.AddBattery();
            else if (slot.itemType == ItemType.Key)
                playerInventory.AddKey(); // <-- handle keys

            Destroy(slot.icon.transform.parent.gameObject);
        }
        cabinetSlots.Clear();
        lockedAfterTransfer = true;
        ToggleCabinetUI(false);

        // Trigger killer robots
        Debug.Log("Robots are active");
        StartCoroutine(ShowRobotsFullWarning());
        if (killerRobotManager != null)
        {
            killerRobotManager.SpawnRobots(PlayerFloorManager.CurrentFloor);
        }

        FindObjectOfType<CabinetSystem>()?.CloseCabinet();
    }

    public void TransferOneToInventory()
    {
        if (cabinetSlots.Count == 0) return;

        var item = cabinetSlots[0];
        bool added = false;

        if (item.itemType == ItemType.Cheese)
            added = playerInventory.AddCheese();
        else if (item.itemType == ItemType.Battery)
            added = playerInventory.AddBattery();

        if (added)
        {
            Destroy(item.icon.transform.parent.gameObject);
            cabinetSlots.RemoveAt(0);
        }
    }

    private void AddCabinetItem(ItemType type)
    {
        GameObject slot = Instantiate(slotPrefab, slotContainer);
        Image icon = slot.transform.Find("Icon").GetComponent<Image>();
        icon.enabled = true;

        switch (type)
        {
            case ItemType.Cheese:
                icon.sprite = cheeseIcon;
                break;
            case ItemType.Battery:
                icon.sprite = batteryIcon;
                break;
            case ItemType.Key:
                icon.sprite = playerInventory.keyIcon; // <- add a reference in CabinetUIManager
                break;
        }

        CabinetItemSlot newSlot = new CabinetItemSlot
        {
            itemType = type,
            icon = icon
        };
        cabinetSlots.Add(newSlot);
    }

    private IEnumerator ShowRobotsFullWarning()
    {
        KillerRobots.SetActive(true);
        yield return new WaitForSeconds(5f);
        KillerRobots.SetActive(false);
    }

    private void OnExitClicked()
    {
        ToggleCabinetUI(false);
        FindObjectOfType<CabinetSystem>()?.CloseCabinet();
    }

    public bool IsCabinetLocked() => lockedAfterTransfer;
}
