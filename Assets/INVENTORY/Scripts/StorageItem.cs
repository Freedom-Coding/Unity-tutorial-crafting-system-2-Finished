using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class StorageItem
{
    public int currentStack;
    public ItemSO itemScriptableObject;

    public StorageItem(int _currentStack, ItemSO _itemScriptableObject)
    {
        currentStack = _currentStack;
        itemScriptableObject = _itemScriptableObject;
    }
}
