using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventorySystem : MonoBehaviour
{
    public GameObject inventoryPanel;
    public GameObject slotPrefab;
    public Sprite cheeseIcon;
    public Sprite batteryIcon;
    public Sprite keyIcon;
    public GameObject bagOnBack;

    [SerializeField] private StaminaBar staminaBar;
    [SerializeField] private BatteryBar batteryBar;

    public int baseSlots = 2;
    public int extendedSlots = 4;
    public int maxBagSlots = 8;

    private bool hasBag = false;
    private bool hasCider = false;
    private bool hasKey = false;

    private List<GameObject> slots = new List<GameObject>();

    // Event to notify bag pickup
    public event Action OnBagAcquired;

    void Start()
    {
        inventoryPanel.SetActive(false);
        InitializeSlots(baseSlots);
    }

    // -------------------- Bag & Cider --------------------
    public bool AddBag()
    {
        if (hasBag) return false;

        hasBag = true;
        if (bagOnBack != null) bagOnBack.SetActive(true);

        UpdateSlotCount();
        OnBagAcquired?.Invoke();
        Debug.Log("Bag acquired! Inventory expanded.");
        return true;
    }

    public void AddCider()
    {
        if (!hasCider)
        {
            hasCider = true;
            Debug.Log("Cider acquired. Extra slots unlocked.");
            UpdateSlotCount();
        }
    }

    public bool HasBag() => hasBag;

    private void UpdateSlotCount()
    {
        int target = baseSlots;
        if (hasCider) target = Mathf.Max(target, extendedSlots);
        if (hasBag) target = Mathf.Max(target, maxBagSlots);
        ExpandSlots(target);
    }

    // -------------------- Inventory UI --------------------
    private void InitializeSlots(int count)
    {
        slots.Clear();
        foreach (Transform child in inventoryPanel.transform) Destroy(child.gameObject);

        for (int i = 0; i < count; i++) AddSlot();
    }

    private void ExpandSlots(int total)
    {
        int currentCount = slots.Count;
        for (int i = currentCount; i < total; i++) AddSlot();
        Debug.Log($"Inventory slots: {slots.Count}");
    }

    private void AddSlot()
    {
        GameObject slot = Instantiate(slotPrefab, inventoryPanel.transform);
        Image icon = slot.transform.Find("Icon").GetComponent<Image>();
        if (icon != null) icon.enabled = false;
        slots.Add(slot);
    }

    public void ToggleInventory()
    {
        inventoryPanel.SetActive(!inventoryPanel.activeSelf);
    }

    // -------------------- Items --------------------
    public bool AddCheese() => AddItem(cheeseIcon, UseCheese);
    public bool AddBattery() => AddItem(batteryIcon, UseBattery);

    public bool AddKey()
    {
        if (hasKey)
            return false; // Already have a key

        bool added = AddItem(keyIcon, UseKey);
        if (added)
        {
            hasKey = true;
            Debug.Log("Key added to inventory!");
        }
        else
        {
            Debug.Log("Inventory full! Cannot add key.");
        }
        return added;
    }

    public bool HasKey() => hasKey;

    private bool AddItem(Sprite iconSprite, Action<Image> useCallback)
    {
        foreach (var slot in slots)
        {
            Image icon = slot.transform.Find("Icon").GetComponent<Image>();
            if (!icon.enabled)
            {
                icon.sprite = iconSprite;
                icon.enabled = true;
                Button button = icon.GetComponent<Button>();
                if (button != null)
                {
                    button.onClick.RemoveAllListeners();
                    button.interactable = true;
                    button.onClick.AddListener(() => useCallback(icon));
                }
                return true;
            }
        }
        return false; // Inventory full
    }

    private void UseCheese(Image icon)
    {
        staminaBar?.RestoreStamina(5f);
        ClearSlot(icon);
    }

    private void UseBattery(Image icon)
    {
        batteryBar?.RestoreBattery(3f);
        ClearSlot(icon);
    }

    private void UseKey(Image icon)
    {
        Debug.Log("Key used.");
        hasKey = false;
        ClearSlot(icon);
    }

    private void ClearSlot(Image icon)
    {
        icon.sprite = null;
        icon.enabled = false;
        Button button = icon.GetComponent<Button>();
        if (button != null)
        {
            button.onClick.RemoveAllListeners();
            button.interactable = false;
        }
    }

    // -------------------- Remove / Check --------------------
    public bool RemoveItem(CabinetUIManager.ItemType type)
    {
        foreach (var slot in slots)
        {
            Image icon = slot.transform.Find("Icon").GetComponent<Image>();
            if (icon.enabled &&
               ((type == CabinetUIManager.ItemType.Cheese && icon.sprite == cheeseIcon) ||
                (type == CabinetUIManager.ItemType.Battery && icon.sprite == batteryIcon)))
            {
                ClearSlot(icon);
                return true;
            }
        }
        return false;
    }

    public bool HasFreeSlot()
    {
        foreach (var slot in slots)
        {
            Image icon = slot.transform.Find("Icon").GetComponent<Image>();
            if (!icon.enabled) return true;
        }
        return false;
    }
}

