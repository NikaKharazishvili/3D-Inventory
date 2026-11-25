using UnityEngine;
using static GameConstants;

public enum ItemState { Free, Held, Stored }

/// <summary>A physical item in the world. Owns its physics state and its transitions in and out of the inventory.</summary>
public class Item : MonoBehaviour
{
    [SerializeField] ScriptableItem data;
    [SerializeField] Rigidbody rb;
    [SerializeField] AudioSource audioSource;

    public ScriptableItem Data => data;
    public Rigidbody Rb => rb;
    public ItemState State { get; private set; }

    /// <summary>Applies custom gravity so heavier items fall faster. Held and stored items are kinematic and skip it.</summary>
    void FixedUpdate()
    {
        if (State == ItemState.Free) rb.AddForce(Vector3.down * (ItemGravity * data.Weight), ForceMode.Acceleration);
    }

    /// <summary>Plays the item's sound on hard impacts.</summary>
    void OnCollisionEnter(Collision collision)
    {
        if (collision.relativeVelocity.magnitude > ItemCollisionSoundThreshold) audioSource.PlayOneShot(data.Sound);
    }

    /// <summary>Switches between free (physics driven), held (following the cursor) and stored (inside the chest).</summary>
    public void SetState(ItemState state)
    {
        State = state;
        rb.isKinematic = state != ItemState.Free;
    }

    /// <summary>Moves the item into the chest. Returns false (and leaves the item untouched) when the inventory is full.</summary>
    public bool TryAddToInventory()
    {
        if (!GlobalReferences.Instance.Inventory.TryAddItem(this)) return false;
        SetState(ItemState.Stored);
        return true;
    }

    /// <summary>Drops the item from the chest back into the world.</summary>
    public void DropFromChest()
    {
        if (State != ItemState.Stored) return;
        GlobalReferences.Instance.Inventory.RemoveItem(this);
        SetState(ItemState.Free);
    }
}
