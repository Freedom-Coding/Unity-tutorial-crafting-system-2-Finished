using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CraftingManager : MonoBehaviour
{
    public static CraftingManager Instance { get; private set; }

    public RecipeType selectedRecipeType;

    [SerializeField] ItemRecipeSO[] recipes;
    [SerializeField] GameObject recipePrefab;
    [SerializeField] Transform recipeParent;

    private List<ItemTypeAndCount> items = new List<ItemTypeAndCount>();


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnEnable()
    {
        selectedRecipeType = RecipeType.basic;
        UpdateRecipeUI();
    }

    public bool CanCraftRecipe(ItemRecipeSO recipeSO)
    {
        items = InventoryManager.Instance.GetAllItems();

        int foundItems = 0;

        foreach (ItemTypeAndCount neededItemAndCount in recipeSO.input)
        {
            foreach (ItemTypeAndCount foundItemAndCount in items)
            {
                if (foundItemAndCount.item == neededItemAndCount.item && foundItemAndCount.count >= neededItemAndCount.count)
                {
                    foundItems++;
                    break;
                }
            }
        }

        return foundItems == recipeSO.input.Length;
    }

    private void UpdateRecipeUI()
    {
        foreach (Transform child in recipeParent)
        {
            Destroy(child.gameObject);
        }

        for (int i = 0; i < recipes.Length; i++)
        {
            if (recipes[i].recipeType == selectedRecipeType)
            {
                GameObject newRecipe = Instantiate(recipePrefab, recipeParent);
                newRecipe.name = recipes[i].name;
            }
        }

        for (int i = 0; i < recipeParent.childCount; i++)
        {
            ItemRecipe recipeScript = recipeParent.GetChild(i).GetComponent<ItemRecipe>();
            ItemRecipeSO recipeSO = null;

            foreach (ItemRecipeSO r in recipes)
            {
                if (r.recipeName == recipeParent.GetChild(i).name)
                {
                    recipeSO = r;
                    break;
                }
            }

            recipeScript.UpdateRecipeUI(recipeSO);
        }
    }

    public void SelectRecipeType(string type)
    {
        switch (type)
        {
            case "basic":
                selectedRecipeType = RecipeType.basic;
                break;
            case "weapons":
                selectedRecipeType = RecipeType.weapons;
                break;
            case "armour":
                selectedRecipeType = RecipeType.armour;
                break;
        }

        UpdateRecipeUI();
    }
}
