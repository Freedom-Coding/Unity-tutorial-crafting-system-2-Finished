using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class InventoryManager : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public static InventoryManager Instance { get; private set; }

    public List<ItemTypeAndCount> items = new List<ItemTypeAndCount>();

    [HideInInspector] public bool isStorageOpened;

    [SerializeField] GameObject[] hotbarSlots = new GameObject[4];
    [SerializeField] GameObject[] slots = new GameObject[20];
    [SerializeField] GameObject inventoryParent;
    [SerializeField] GameObject storageParent;
    [SerializeField] Transform handParent;
    [SerializeField] GameObject craftingParent;
    [SerializeField] GameObject itemPrefab;
    [SerializeField] Camera cam;

    GameObject draggedObject;
    GameObject lastItemSlot;

    Storage lastStorage;

    bool isInventoryOpened;
    bool isCraftingOpened;

    int selectedHotbarSlot = 0;

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
    void Start()
    {
        HotbarItemChanged();
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        CheckForHotbarInput();

        storageParent.SetActive(isStorageOpened);
        inventoryParent.SetActive(isInventoryOpened);
        craftingParent.SetActive(isCraftingOpened);

        //Move item
        if (draggedObject != null)
        {
            draggedObject.transform.position = Input.mousePosition;
        }

        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (isInventoryOpened)
            {
                Cursor.lockState = CursorLockMode.Locked;
                isInventoryOpened = false;
                isStorageOpened = false;
                isCraftingOpened = false;

                if (lastStorage != null)
                {
                    CloseStorage(lastStorage);
                }
            }
            else
            {
                Cursor.lockState = CursorLockMode.None;
                isInventoryOpened = true;
            }
        }
    }

    public List<ItemTypeAndCount> GetAllItems()
    {
        items.Clear();

        foreach(GameObject slot in slots)
        {
            InventorySlot slotScript = slot.GetComponent<InventorySlot>();

            if (slotScript.heldItem != null)
            {
                InventoryItem itemScript = slotScript.heldItem.GetComponent<InventoryItem>();

                int i = 0;
                bool wasItemAdded = false;

                foreach (ItemTypeAndCount itemAndCount in items)
                {
                    if (itemAndCount.item == itemScript.itemScriptableObject)
                    {
                        items[i].count += itemScript.stackCurrent;
                        wasItemAdded = true;
                    }
                    i++;
                }

                if (!wasItemAdded)
                {
                    items.Add(new ItemTypeAndCount(itemScript.itemScriptableObject, itemScript.stackCurrent));
                }
            }
        }

        foreach (GameObject slot in hotbarSlots)
        {
            InventorySlot slotScript = slot.GetComponent<InventorySlot>();

            if (slotScript.heldItem != null)
            {
                InventoryItem itemScript = slotScript.heldItem.GetComponent<InventoryItem>();

                int i = 0;
                bool wasItemAdded = false;

                foreach (ItemTypeAndCount itemAndCount in items)
                {
                    if (itemAndCount.item == itemScript.itemScriptableObject)
                    {
                        items[i].count += itemScript.stackCurrent;
                        wasItemAdded = true;
                    }
                    i++;
                }

                if (!wasItemAdded)
                {
                    items.Add(new ItemTypeAndCount(itemScript.itemScriptableObject, itemScript.stackCurrent));
                }
            }
        }

        return items;
    }

    private void CheckForHotbarInput()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            selectedHotbarSlot = 0;
            HotbarItemChanged();
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            selectedHotbarSlot = 1;
            HotbarItemChanged();
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            selectedHotbarSlot = 2;
            HotbarItemChanged();
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            selectedHotbarSlot = 3;
            HotbarItemChanged();
        }
    }

    private void HotbarItemChanged()
    {
        for (int i = 0; i < handParent.childCount; i++)
        {
            handParent.GetChild(i).gameObject.SetActive(false);
        }

        foreach(GameObject slot in hotbarSlots)
        {
            Vector3 scale;

            if (slot == hotbarSlots[selectedHotbarSlot])
            {
                scale = new Vector3(1.1f, 1.1f, 1.1f);

                if (slot.GetComponent<InventorySlot>().heldItem != null)
                {
                    for (int i = 0; i < handParent.childCount; i++)
                    {
                        if (handParent.GetChild(i).GetComponent<ItemHand>().itemScriptableObject 
                            == hotbarSlots[selectedHotbarSlot].GetComponent<InventorySlot>().heldItem.GetComponent<InventoryItem>().itemScriptableObject)
                        {
                            handParent.GetChild(i).gameObject.SetActive(true);
                        }
                    }
                }
            }
            else
            {
                scale = new Vector3(0.9f, 0.9f, 0.9f);
            }

            slot.transform.localScale = scale;
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            GameObject clickedObject = eventData.pointerCurrentRaycast.gameObject;
            InventorySlot slot = clickedObject.GetComponent<InventorySlot>();

            //There is item in the slot - pick it up
            if (slot != null && slot.heldItem != null)
            {
                draggedObject = slot.heldItem;
                slot.heldItem = null;
                lastItemSlot = clickedObject;
            }
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (draggedObject != null && eventData.pointerCurrentRaycast.gameObject != null && eventData.button == PointerEventData.InputButton.Left)
        {
            GameObject clickedObject = eventData.pointerCurrentRaycast.gameObject;
            InventorySlot slot = clickedObject.GetComponent<InventorySlot>();

            //There isnt item in the slot - place item
            if (slot != null && slot.heldItem == null)
            {
                slot.SetHeldItem(draggedObject);
                draggedObject.transform.SetParent(slot.transform.parent.parent.GetChild(2));
            }
            //There is item in the slot - switch items
            else if (slot != null && slot.heldItem != null && slot.heldItem.GetComponent<InventoryItem>().stackCurrent == slot.heldItem.GetComponent<InventoryItem>().stackMax
                || slot != null && slot.heldItem != null && slot.heldItem.GetComponent<InventoryItem>().itemScriptableObject != draggedObject.GetComponent<InventoryItem>().itemScriptableObject)
            {
                lastItemSlot.GetComponent<InventorySlot>().SetHeldItem(slot.heldItem);
                slot.heldItem.transform.SetParent(slot.transform.parent.parent.GetChild(2));

                slot.SetHeldItem(draggedObject);
                draggedObject.transform.SetParent(slot.transform.parent.parent.GetChild(2));
            }

            //Fill stack
            else if (slot != null && slot.heldItem != null && slot.heldItem.GetComponent<InventoryItem>().stackCurrent < slot.heldItem.GetComponent<InventoryItem>().stackMax
                && slot.heldItem.GetComponent<InventoryItem>().itemScriptableObject == draggedObject.GetComponent<InventoryItem>().itemScriptableObject)
            {
                InventoryItem slotHeldItem = slot.heldItem.GetComponent<InventoryItem>();
                InventoryItem draggedItem = draggedObject.GetComponent<InventoryItem>();

                int itemsToFillStack = slotHeldItem.stackMax - slotHeldItem.stackCurrent;

                if (itemsToFillStack >= draggedItem.stackCurrent)
                {
                    slotHeldItem.stackCurrent += draggedItem.stackCurrent;
                    Destroy(draggedObject);
                }
                else
                {
                    slotHeldItem.stackCurrent += itemsToFillStack;
                    draggedItem.stackCurrent -= itemsToFillStack;
                    lastItemSlot.GetComponent<InventorySlot>().SetHeldItem(draggedObject);
                }
            }
            //Return item to last slot
            else if (clickedObject.name != "DropItem")
            {
                lastItemSlot.GetComponent<InventorySlot>().SetHeldItem(draggedObject);
                draggedObject.transform.SetParent(slot.transform.parent.parent.GetChild(2));
            }
            //Drop item
            else
            {
                Ray ray = cam.ScreenPointToRay(Input.mousePosition);
                Vector3 position = ray.GetPoint(3);

                GameObject newItem = Instantiate(draggedObject.GetComponent<InventoryItem>().itemScriptableObject.prefab, position, new Quaternion());
                newItem.GetComponent<itemPickable>().itemScriptableObject = draggedObject.GetComponent<InventoryItem>().itemScriptableObject;

                lastItemSlot.GetComponent<InventorySlot>().heldItem = null;
                Destroy(draggedObject);
            }

            HotbarItemChanged();
            draggedObject = null;
        }
    }

    public void ItemPicked(GameObject pickedItem)
    {
        GameObject emptySlot = null;

        for (int i = 0; i < slots.Length; i++)
        {
            InventorySlot slot = slots[i].GetComponent<InventorySlot>();

            if (slot.heldItem == null)
            {
                emptySlot = slots[i];
                break;
            }
        }

        if (emptySlot != null)
        {
            GameObject newItem = Instantiate(itemPrefab);
            newItem.GetComponent<InventoryItem>().itemScriptableObject = pickedItem.GetComponent<itemPickable>().itemScriptableObject;
            newItem.transform.SetParent(emptySlot.transform.parent.parent.GetChild(2));
            newItem.GetComponent<InventoryItem>().stackCurrent = 1;

            emptySlot.GetComponent<InventorySlot>().SetHeldItem(newItem);
            newItem.transform.localScale = new Vector3(1, 1, 1);

            Destroy(pickedItem);
        }
    }

    public void CraftItems(List<ItemTypeAndCount> itemsToCraft, List<ItemTypeAndCount> itemsToDestroy)
    {
        foreach (ItemTypeAndCount itemToCraft in itemsToCraft)
        {
            GameObject emptySlot = null;

            for (int i = 0; i < slots.Length; i++)
            {
                InventorySlot slot = slots[i].GetComponent<InventorySlot>();

                if (slot.heldItem == null)
                {
                    emptySlot = slots[i];
                    break;
                }
            }

            if (emptySlot != null)
            {
                GameObject newItem = Instantiate(itemPrefab);
                newItem.GetComponent<InventoryItem>().itemScriptableObject = itemToCraft.item;
                newItem.transform.SetParent(emptySlot.transform.parent.parent.GetChild(2));
                newItem.GetComponent<InventoryItem>().stackCurrent = itemToCraft.count;

                emptySlot.GetComponent<InventorySlot>().SetHeldItem(newItem);
                newItem.transform.localScale = new Vector3(1, 1, 1);
            }
        }

        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i].GetComponent<InventorySlot>().heldItem != null)
            {
                InventorySlot slotScript = slots[i].GetComponent<InventorySlot>();
                InventoryItem itemScript = slotScript.heldItem.GetComponent<InventoryItem>();

                foreach (ItemTypeAndCount itemToDestroy in itemsToDestroy)
                {
                    if (itemToDestroy.item == itemScript.itemScriptableObject)
                    {
                        if (itemToDestroy.count >= itemScript.stackCurrent)
                        {
                            slotScript.heldItem = null;
                            Destroy(itemScript.gameObject);
                        }
                        else if (itemToDestroy.count < itemScript.stackCurrent)
                        {
                            itemScript.stackCurrent -= itemToDestroy.count;
                        }
                    }
                }
            }
        }
    }

    public void OpenStorage(Storage storage)
    {
        lastStorage = storage;

        Cursor.lockState = CursorLockMode.None;
        isStorageOpened = true;

        //Set all slots to inactive
        for (int i = 0; i < storageParent.transform.GetChild(1).childCount; i++)
        {
            storageParent.transform.GetChild(1).GetChild(i).gameObject.SetActive(false);
        }

        //Set some of the slots to active
        for (int i = 0; i < storage.size; i++)
        {
            storageParent.transform.GetChild(1).GetChild(i).gameObject.SetActive(true);
        }
        //Set background size and position
        float sizeY = (float)Mathf.CeilToInt(storage.size / 4f) / 4;
        storageParent.transform.GetChild(0).localScale = new Vector2(1, sizeY);

        float posY = (1 - sizeY) * 230;
        storageParent.transform.GetChild(0).localPosition = new Vector2(-615, 130 + posY);

        //Destroy all items
        for (int i = 0; i < storageParent.transform.GetChild(2).childCount; i++)
        {
            Destroy(storageParent.transform.GetChild(2).GetChild(i).gameObject);
        }

        int index = 0;
        foreach (StorageItem storageItem in storage.items)
        {
            if (storageItem.itemScriptableObject != null)
            {
                GameObject newItem = Instantiate(itemPrefab);
                InventoryItem item = newItem.GetComponent<InventoryItem>();
                item.itemScriptableObject = storageItem.itemScriptableObject;
                item.stackCurrent = storageItem.currentStack;

                Transform slot = storageParent.transform.GetChild(1).GetChild(index);
                newItem.transform.SetParent(slot.parent.parent.GetChild(2));
                slot.GetComponent<InventorySlot>().SetHeldItem(newItem);
                newItem.transform.localScale = new Vector3(1, 1, 1);
            }
            index++;
        }
    }

    public void CloseStorage(Storage storage)
    {
        lastStorage = null;
        Cursor.lockState = CursorLockMode.Locked;
        isStorageOpened = false;

        Transform slotsParent = storageParent.transform.GetChild(1);

        storage.items.Clear();

        for (int i = 0; i < slotsParent.childCount; i++)
        {
            Transform slot = slotsParent.GetChild(i);

            if (slot.gameObject.activeInHierarchy && slot.GetComponent<InventorySlot>().heldItem != null)
            {
                InventoryItem inventoryItem = slot.GetComponent<InventorySlot>().heldItem.GetComponent<InventoryItem>();

                storage.items.Add(new StorageItem(inventoryItem.stackCurrent, inventoryItem.itemScriptableObject));
            }
            else
            {
                storage.items.Add(new StorageItem(0, null));
            }
        }
    }

    public void OpenAndCloseCrafting()
    {
        if (isCraftingOpened)
        {
            isCraftingOpened = false;
        }
        else
        {
            isCraftingOpened = true;
        }
    }
}
