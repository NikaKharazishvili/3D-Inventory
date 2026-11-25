using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static GameConstants;

/// <summary>Manages the 3D chest inventory: slots, stored items, weight and UI state.</summary>
public class Inventory : MonoBehaviour
{
    enum ChestSound { Open, Close, Pickup, Drop }

    static readonly int IsOpenHash = Animator.StringToHash("IsOpen");

    [SerializeField] TMP_Text itemCountText, weightCountText;
    [SerializeField] Transform[] slots;
    [SerializeField] Image[] icons;
    [SerializeField] Animator animator;
    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioClip chestOpen, chestClose, itemPickup, itemDrop;
    [SerializeField] GameObject inventoryIcons;

    GlobalReferences global;
    float weightCount;
    bool isOnCd;

    public bool IsChestOpen { get; private set; }
    public int UsedSlots { get; private set; }

    void Awake()
    {
        global = GlobalReferences.Instance;
        LayoutSlots();
        RefreshStats();
    }

    /// <summary>Opens or closes the chest. Ignored while the toggle animation is still playing.</summary>
    public void ToggleChest()
    {
        if (isOnCd) return;
        isOnCd = true;
        this.Wait(ChestToggleCooldown, () => isOnCd = false);

        IsChestOpen = !IsChestOpen;
        animator.SetBool(IsOpenHash, IsChestOpen);
    }

    /// <summary>Puts the item into the first free slot. Returns false when the inventory is full.</summary>
    public bool TryAddItem(Item item)
    {
        if (UsedSlots >= TotalInventorySlots)
        {
            Utils.SendLogMessage("Your inventory is already full".Colored("red"));
            return false;
        }

        foreach (Transform slot in slots)
        {
            if (slot.childCount > 0) continue;
            item.transform.SetParent(slot, false);
            item.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.Euler(270, 0, 0));
            break;
        }

        PlaySound((int)ChestSound.Pickup);
        RefreshStats();
        Utils.SendLogMessage(item.Data.DisplayName.Colored("green") + " has been added into inventory");
        if (UsedSlots == TotalInventorySlots) Utils.SendLogMessage("Your inventory is full".Colored("yellow"));
        return true;
    }

    /// <summary>Takes the item out of its slot, places it in the world next to the chest and closes the gap it leaves.</summary>
    public void RemoveItem(Item item)
    {
        item.transform.SetParent(global.ItemSpawner.transform, false);
        item.transform.position = transform.position + RemovedItemOffset;

        CompactSlots();
        PlaySound((int)ChestSound.Drop);
        RefreshStats();
        Utils.SendLogMessage(item.Data.DisplayName.Colored("red") + " has been removed from inventory");
    }

    /// <summary>Drops every stored item at once. Only works while the chest is open.</summary>
    public void DropAllItems()
    {
        if (UsedSlots == 0 || !IsChestOpen) return;

        foreach (Transform slot in slots)
        {
            if (slot.childCount == 0) break;
            var item = slot.GetChild(0).GetComponent<Item>();
            item.transform.SetParent(global.ItemSpawner.transform, false);
            item.transform.position = transform.position + DroppedItemOffset;
            item.SetState(ItemState.Free);
        }

        PlaySound((int)ChestSound.Drop);
        RefreshStats();
        Utils.SendLogMessage("All items removed from inventory".Colored("red"));
    }

    /// <summary>Shows or hides the 2D icon preview of the stored items.</summary>
    public void ShowIcons(bool show) => inventoryIcons.SetActive(show);

    /// <summary>Recalculates item count and weight from the slots, then refreshes the counters and the icon preview.</summary>
    void RefreshStats()
    {
        UsedSlots = 0;
        weightCount = 0;

        for (int i = 0; i < slots.Length; i++)
        {
            bool isFilled = slots[i].childCount > 0;
            icons[i].gameObject.SetActive(isFilled);
            if (!isFilled) continue;

            var itemData = slots[i].GetChild(0).GetComponent<Item>().Data;
            icons[i].sprite = itemData.Icon;
            weightCount += itemData.Weight;
            UsedSlots++;
        }

        itemCountText.text = Utils.FormatCounter(UsedSlots, TotalInventorySlots);
        weightCountText.text = weightCount.ToString();
    }

    /// <summary>Plays a chest sound by ChestSound index. Public because the chest open/close animations call it through animation events.</summary>
    public void PlaySound(int sound)
    {
        var clip = (ChestSound)sound switch
        {
            ChestSound.Open => chestOpen,
            ChestSound.Close => chestClose,
            ChestSound.Pickup => itemPickup,
            _ => itemDrop
        };
        audioSource.PlayOneShot(clip);
    }

    /// <summary>Shifts items forward to fill any empty slots.</summary>
    void CompactSlots()
    {
        for (int i = 0; i < slots.Length - 1; i++)
        {
            if (slots[i].childCount > 0) continue;

            for (int j = i + 1; j < slots.Length; j++)
            {
                if (slots[j].childCount == 0) continue;
                slots[j].GetChild(0).SetParent(slots[i], false);
                slots[i].GetChild(0).localPosition = Vector3.zero;
                break;
            }
        }
    }

    /// <summary>Arranges the slots in a grid of InventoryColumns columns.</summary>
    void LayoutSlots()
    {
        for (int i = 0; i < slots.Length; i++)
        {
            int row = i / InventoryColumns;
            int column = i % InventoryColumns;
            slots[i].localPosition = SlotGridOffset + new Vector3(column * SlotSpacing.x, row * SlotSpacing.y, 0);
        }
    }
}
