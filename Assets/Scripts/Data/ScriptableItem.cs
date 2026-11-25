using UnityEngine;

/// <summary>Static data of an item type: display name, weight, drop sound and inventory icon.</summary>
[CreateAssetMenu(fileName = "Item", menuName = "3D Inventory/Item")]
public class ScriptableItem : ScriptableObject
{
    [SerializeField] string displayName;
    [SerializeField] float weight; // Heavier items fall more forcefully
    [SerializeField] AudioClip sound; // Played when the item hits something hard
    [SerializeField] Sprite icon; // Shown in the 2D inventory preview

    public string DisplayName => displayName;
    public float Weight => weight;
    public AudioClip Sound => sound;
    public Sprite Icon => icon;
}
