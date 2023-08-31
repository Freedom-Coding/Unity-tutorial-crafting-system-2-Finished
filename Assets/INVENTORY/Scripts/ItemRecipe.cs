using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ItemRecipe : MonoBehaviour
{
    public ItemRecipeSO recipeSO;

    [SerializeField] GameObject itemPrefab;
    [SerializeField] GameObject plusSignPrefab;
    [SerializeField] GameObject equalSignPrefab;

    [SerializeField] Gradient fadeGradientCanCraft;
    [SerializeField] Gradient fadeGradientCantCraft;

    private bool canCraftRecipe;

    public void OnPointerEnter()
    {
        canCraftRecipe = CraftingManager.Instance.CanCraftRecipe(recipeSO);

        StartCoroutine(FadeIn());
    }

    public void OnPointerExit()
    {
        StopAllCoroutines();

        StartCoroutine(FadeOut());
    }

    public void OnPointerClick()
    {
        if (CraftingManager.Instance.CanCraftRecipe(recipeSO))
        {
            InventoryManager.Instance.CraftItems(new List<ItemTypeAndCount>(recipeSO.output), new List<ItemTypeAndCount>(recipeSO.input));

            canCraftRecipe = CraftingManager.Instance.CanCraftRecipe(recipeSO);
            if (!canCraftRecipe)
            {
                StartCoroutine(FadeIn());
            }
        }
    }

    private IEnumerator FadeIn()
    {
        float gradientTime = 0;

        while (gradientTime < 1)
        {
            foreach (Transform child in transform)
            {
                Image[] imageArray = child.GetComponentsInChildren<Image>();

                List<Image> images = new List<Image>(imageArray);

                foreach (Image i in images)
                {
                    if (i.color != Color.black)
                    {
                        if (canCraftRecipe)
                        {
                            i.color = fadeGradientCanCraft.Evaluate(gradientTime);
                        }
                        else
                        {
                            i.color = fadeGradientCantCraft.Evaluate(gradientTime);
                        }
                    }
                }
            }

            gradientTime += Time.deltaTime * 3;

            yield return null;
        }
    }

    private IEnumerator FadeOut()
    {
        float gradientTime = 1;

        while (gradientTime > 0)
        {
            foreach (Transform child in transform)
            {
                Image[] imageArray = child.GetComponentsInChildren<Image>();

                List<Image> images = new List<Image>(imageArray);

                foreach (Image i in images)
                {
                    if (i.color != Color.black)
                    {
                        if (canCraftRecipe)
                        {
                            i.color = fadeGradientCanCraft.Evaluate(gradientTime);
                        }
                        else
                        {
                            i.color = fadeGradientCantCraft.Evaluate(gradientTime);
                        }
                    }
                }
            }

            gradientTime -= Time.deltaTime * 3;

            yield return null;
        }
    }

    public void UpdateRecipeUI(ItemRecipeSO newRecipeSO)
    {
        recipeSO = newRecipeSO;

        foreach(Transform child in transform)
        {
            Destroy(child.gameObject);
        }

        for (int i = 0; i < recipeSO.input.Length; i++)
        {
            GameObject newItem = Instantiate(itemPrefab, transform);
            newItem.transform.GetChild(0).GetComponent<Image>().sprite = recipeSO.input[i].item.icon;
            newItem.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = recipeSO.input[i].count.ToString();

            if (i < recipeSO.input.Length - 1)
            {
                Instantiate(plusSignPrefab, transform);
            }
        }

        Instantiate(equalSignPrefab, transform);

        for (int i = 0; i < recipeSO.output.Length; i++)
        {
            GameObject newItem = Instantiate(itemPrefab, transform);
            newItem.transform.GetChild(0).GetComponent<Image>().sprite = recipeSO.output[i].item.icon;
            newItem.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = recipeSO.output[i].count.ToString();

            if (i < recipeSO.output.Length - 1)
            {
                Instantiate(plusSignPrefab, transform);
            }
        }
    }
}
