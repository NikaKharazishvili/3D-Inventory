using UnityEngine;
using static GameConstants;

/// <summary>Periodically spawns random items above itself until the spawn limit is reached.</summary>
public class ItemSpawner : MonoBehaviour
{
    [SerializeField] GameObject[] itemPrefabs;
    float spawnCD;

    public int SpawnedItemsCount { get; private set; }

    void Awake() => Utils.SendLogMessage("Items spawn every few seconds — drag them with LMB or pick them up with E".Colored("yellow"));

    void Update()
    {
        spawnCD -= Time.deltaTime;
        if (spawnCD > 0) return;

        spawnCD = Random.Range(SpawnCDMin, SpawnCDMax);
        SpawnItem();

        if (SpawnedItemsCount < MaxSpawnedItems) return;
        Utils.SendLogMessage("All the items have been spawned".Colored("yellow"));
        enabled = false;
    }

    /// <summary>Spawns a random item prefab at a random position above the spawner.</summary>
    void SpawnItem()
    {
        var offset = new Vector3(Random.Range(-SpawnAreaHalfSize, SpawnAreaHalfSize), Random.Range(SpawnHeightMin, SpawnHeightMax), Random.Range(-SpawnAreaHalfSize, SpawnAreaHalfSize));
        Instantiate(itemPrefabs[Random.Range(0, itemPrefabs.Length)], transform.position + offset, Quaternion.identity, transform);
        SpawnedItemsCount++;
    }
}
