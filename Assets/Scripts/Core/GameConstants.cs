using UnityEngine;

/// <summary>Centralized tuning values and configuration constants for the project.</summary>
public static class GameConstants
{
    public const string ItemTag = "Item";

    // Player & mouse
    public const float MoveSpeed = 5;
    public const float MovementThreshold = 0.3f;
    public const float MouseSensitivity = 2;
    public const float VerticalClamp = 80;

    // Inventory & chest
    public const int TotalInventorySlots = 16;
    public const int InventoryColumns = 4;
    public const float ChestToggleCooldown = 1; // Matches the chest open/close animation length
    public static readonly Vector3 SlotGridOffset = new(0, 0.8f, 0);
    public static readonly Vector2 SlotSpacing = new(1.1f, -1.15f);
    public static readonly Vector3 RemovedItemOffset = Vector3.forward; // Where an item appears relative to the chest when removed
    public static readonly Vector3 DroppedItemOffset = Vector3.up + Vector3.forward; // Same, when dropping everything at once

    // Item spawning
    public const int MaxSpawnedItems = 20;
    public const float SpawnCDMin = 3;
    public const float SpawnCDMax = 6;
    public const float SpawnAreaHalfSize = 6;
    public const float SpawnHeightMin = 2;
    public const float SpawnHeightMax = 6;

    // Item interaction & physics
    public const float ItemMaxPickupDistance = 5;
    public const float ItemMinGroundY = 0.2f;
    public const float ItemScrollSensitivity = 3;
    public const float ItemDistanceMin = 1;
    public const float ItemDistanceMax = 4;
    public const float ItemCollisionSoundThreshold = 6;
    public const float ItemRotationSensitivity = 5;
    public const float ItemThrowStrength = 30;
    public const float ItemGravity = 15; // Multiplied by the item's weight

    // Log messages
    public const float MessageLifespan = 15;
}
