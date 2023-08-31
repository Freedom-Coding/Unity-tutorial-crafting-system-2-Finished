using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Storage : MonoBehaviour
{
    public string name;
    public int size;
    public List<StorageItem> items = new List<StorageItem>();

    [SerializeField] RandomSpawningItems itemsToSpawn;
    [SerializeField] bool spawnItems;

    private bool itemsSpawned;

    void Start()
    {
        int itemsToAdd = size - items.Count;
        for (int i = 0; i < itemsToAdd; i++)
        {
            items.Add(new StorageItem(0, null));
        }

        if (spawnItems && !itemsSpawned)
        {
            int count = Random.Range(itemsToSpawn.minCount, itemsToSpawn.maxCount + 1);

            for (int i = 0; i < count; i++)
            {
                ItemSO itemToSpawn = itemsToSpawn.itemsToSpawn[Random.Range(0, itemsToSpawn.itemsToSpawn.Count)];

                items[i].itemScriptableObject = itemToSpawn;

                if (items[i].itemScriptableObject.stackMax > 1)
                {
                    int countToSpawn = Random.Range(1, items[i].itemScriptableObject.stackMax + 1);
                    items[i].currentStack = countToSpawn;
                }
                else
                {
                    items[i].currentStack = 1;
                }
            }
        }
    }
}
