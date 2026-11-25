using UnityEngine;

/// <summary>Singleton that provides global access to core scene references.</summary>
[DefaultExecutionOrder(-100)] // Runs its Awake before every other script, so Instance is ready for them
public class GlobalReferences : MonoBehaviour
{
    public static GlobalReferences Instance { get; private set; }

    [SerializeField] Camera mainCamera;
    [SerializeField] Inventory inventory;
    [SerializeField] ItemSpawner itemSpawner;
    [SerializeField] LogMessage[] logMessages;

    public Camera MainCamera => mainCamera;
    public Inventory Inventory => inventory;
    public ItemSpawner ItemSpawner => itemSpawner;
    public LogMessage[] LogMessages => logMessages;

    void Awake()
    {
        if (!Instance) Instance = this;
        else Destroy(gameObject);
    }
}
