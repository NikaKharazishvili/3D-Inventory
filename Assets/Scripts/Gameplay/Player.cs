using UnityEngine;
using static GameConstants;

/// <summary>First-person player. Handles all input: movement, looking, dragging, throwing and storing items, and the chest controls.</summary>
public class Player : MonoBehaviour
{
    static readonly int IsMovingHash = Animator.StringToHash("IsMoving");

    [SerializeField] Transform cameraTransform;
    [SerializeField] Rigidbody rb;
    [SerializeField] Animator animator;
    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioClip[] footstepSounds;
    [SerializeField] GameCursor cursor;

    GlobalReferences global;
    Item heldItem;
    bool? isMouseFree; // Null forces the mouse state to be re-applied
    float rotationX, rotationY, dragDistance;

    bool IsChestOpen => global.Inventory.IsChestOpen;

    void Awake() => global = GlobalReferences.Instance;

    /// <summary>Re-applies the mouse state when the window regains focus (for example after pressing Esc in the editor).</summary>
    void OnApplicationFocus(bool hasFocus)
    {
        if (hasFocus) isMouseFree = null;
    }

    /// <summary>Gives the mouse back to the OS so the cursor is never left hidden or locked after play mode or a scene change.</summary>
    void OnDisable()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    void Update()
    {
        RotateCamera();
        Move();
        DragItem();
        TakeItem();
        ThrowHeldItem();
        PreviewInventoryIcons();
        UpdateCursor();
        ToggleChest();
        DropAllItems();
        ClickStoredItem();
    }

    /// <summary>Rotates the camera with the mouse. Suppressed while rotating an item or while the chest is open.</summary>
    void RotateCamera()
    {
        if (heldItem && Input.GetMouseButton(1) || IsChestOpen) return;

        rotationX = Mathf.Clamp(rotationX - Input.GetAxis("Mouse Y") * MouseSensitivity, -VerticalClamp, VerticalClamp);
        rotationY += Input.GetAxis("Mouse X") * MouseSensitivity;

        cameraTransform.localRotation = Quaternion.Euler(rotationX, 0, 0);
        transform.rotation = Quaternion.Euler(0, rotationY, 0);
    }

    /// <summary>Moves the player along the camera's flat axes. Stops while the chest is open or an item is being rotated.</summary>
    void Move()
    {
        animator.SetBool(IsMovingHash, rb.linearVelocity.magnitude > MovementThreshold);

        if (IsChestOpen || (heldItem && Input.GetMouseButton(1)))
        {
            rb.linearVelocity = Vector3.zero;
            return;
        }

        Vector3 flatForward = new Vector3(cameraTransform.forward.x, 0, cameraTransform.forward.z).normalized;
        Vector3 flatRight = new Vector3(cameraTransform.right.x, 0, cameraTransform.right.z).normalized;
        Vector3 direction = flatRight * Input.GetAxis("Horizontal") + flatForward * Input.GetAxis("Vertical");
        rb.linearVelocity = direction.normalized * MoveSpeed;
    }

    /// <summary>Grabs an item with LMB and moves it in front of the camera. While RMB is held, mouse movement rotates the item instead.</summary>
    void DragItem()
    {
        if (IsChestOpen) return;

        if (Input.GetMouseButtonDown(0) && TryGetAimedItem(out Item aimedItem) && IsInReach(aimedItem))
        {
            heldItem = aimedItem;
            dragDistance = Mathf.Clamp(Vector3.Distance(heldItem.transform.position, cameraTransform.position), ItemDistanceMin, ItemDistanceMax);
            heldItem.SetState(ItemState.Held);
        }

        if (Input.GetMouseButtonUp(0) && heldItem) ReleaseHeldItem();
        if (!heldItem) return;

        if (Input.GetMouseButton(1))
        {
            heldItem.transform.Rotate(cameraTransform.up, -Input.GetAxis("Mouse X") * ItemRotationSensitivity, Space.World);
            heldItem.transform.Rotate(cameraTransform.right, Input.GetAxis("Mouse Y") * ItemRotationSensitivity, Space.World);
            return; // The item stays in place while rotating
        }

        dragDistance = Mathf.Clamp(dragDistance + Input.GetAxis("Mouse ScrollWheel") * ItemScrollSensitivity, ItemDistanceMin, ItemDistanceMax);
        Vector3 targetPosition = cameraTransform.position + cameraTransform.forward * dragDistance;
        targetPosition.y = Mathf.Max(targetPosition.y, ItemMinGroundY);
        heldItem.Rb.MovePosition(targetPosition);
    }

    /// <summary>Stores the held or aimed item in the chest on E. Blocked while the chest is open.</summary>
    void TakeItem()
    {
        if (!Input.GetKeyDown(KeyCode.E) || IsChestOpen) return;

        if (heldItem)
        {
            if (heldItem.TryAddToInventory()) heldItem = null; // Otherwise the inventory is full and the item stays in hand
        }
        else if (TryGetAimedItem(out Item aimedItem))
        {
            if (IsInReach(aimedItem)) aimedItem.TryAddToInventory();
            else Utils.SendLogMessage("Item is too far away".Colored("red"));
        }
    }

    /// <summary>Throws the held item forward on G. Blocked while the chest is open.</summary>
    void ThrowHeldItem()
    {
        if (!Input.GetKeyDown(KeyCode.G) || !heldItem || IsChestOpen) return;

        Rigidbody itemBody = heldItem.Rb;
        ReleaseHeldItem();
        itemBody.AddForce(cameraTransform.forward * ItemThrowStrength, ForceMode.Impulse);
    }

    /// <summary>Shows the 2D icon preview of the stored items while U is held.</summary>
    void PreviewInventoryIcons()
    {
        if (Input.GetKeyDown(KeyCode.U)) global.Inventory.ShowIcons(true);
        if (Input.GetKeyUp(KeyCode.U)) global.Inventory.ShowIcons(false);
    }

    /// <summary>
    /// Chest open: the cursor follows the mouse. Chest closed: it stays at the screen center as a crosshair,
    /// showing a hand over a reachable item and a closed hand while dragging.
    /// </summary>
    void UpdateCursor()
    {
        SetMouseFree(IsChestOpen);

        if (IsChestOpen)
        {
            cursor.FollowMouse();
            cursor.SetType(CursorType.Dot);
            return;
        }

        cursor.CenterOnScreen();
        if (heldItem) cursor.SetType(CursorType.HandDrag);
        else if (TryGetAimedItem(out Item aimedItem) && IsInReach(aimedItem)) cursor.SetType(CursorType.Hand);
        else cursor.SetType(CursorType.Dot);
    }

    /// <summary>
    /// Locks the mouse for first-person looking, or frees it (confined to the window) while the chest is open.
    /// The system cursor stays hidden because the game draws its own. Does nothing when the mode is unchanged.
    /// </summary>
    void SetMouseFree(bool free)
    {
        if (isMouseFree == free) return;
        isMouseFree = free;

        Cursor.lockState = free ? CursorLockMode.Confined : CursorLockMode.Locked;
        Cursor.visible = false;
    }

    /// <summary>Opens or closes the chest on I. An item in hand is released first.</summary>
    void ToggleChest()
    {
        if (!Input.GetKeyDown(KeyCode.I)) return;
        if (!IsChestOpen && heldItem) ReleaseHeldItem();
        global.Inventory.ToggleChest();
    }

    /// <summary>Drops everything from the chest on G (the inventory ignores it while the chest is closed).</summary>
    void DropAllItems()
    {
        if (Input.GetKeyDown(KeyCode.G)) global.Inventory.DropAllItems();
    }

    /// <summary>While the chest is open, clicking a stored item drops it back into the world.</summary>
    void ClickStoredItem()
    {
        if (!IsChestOpen || !Input.GetMouseButtonDown(0)) return;
        if (TryGetAimedItem(out Item item)) item.DropFromChest();
    }

    /// <summary>Plays a random footstep sound. Called by an animation event of the walk animation.</summary>
    public void Footsteps() => audioSource.PlayOneShot(footstepSounds[Random.Range(0, footstepSounds.Length)]);

    /// <summary>Returns the item under the mouse pointer, if any.</summary>
    bool TryGetAimedItem(out Item item)
    {
        item = null;
        Ray ray = global.MainCamera.ScreenPointToRay(Input.mousePosition);
        return Physics.Raycast(ray, out RaycastHit hit) && hit.collider.CompareTag(ItemTag) && hit.collider.TryGetComponent(out item);
    }

    bool IsInReach(Item item) => (item.transform.position - transform.position).sqrMagnitude <= ItemMaxPickupDistance * ItemMaxPickupDistance;

    /// <summary>Lets go of the held item and hands it back to physics.</summary>
    void ReleaseHeldItem()
    {
        heldItem.SetState(ItemState.Free);
        heldItem = null;
    }
}
