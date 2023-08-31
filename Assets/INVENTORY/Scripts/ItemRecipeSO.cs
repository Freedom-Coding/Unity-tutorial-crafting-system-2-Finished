using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Item Recipe", menuName = "Scriptable Objects/Item Recipe")]
public class ItemRecipeSO : ScriptableObject
{
    public string recipeName;
    public RecipeType recipeType;

    public ItemTypeAndCount[] input;
    public ItemTypeAndCount[] output;
}

public enum RecipeType { basic, weapons, armour };


[System.Serializable]
public class ItemTypeAndCount
{
    public ItemSO item;
    public int count;

    public ItemTypeAndCount(ItemSO i, int c)
    {
        item = i;
        count = c;
    }
}