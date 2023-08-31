using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] Camera cam;
    [SerializeField] InventoryManager inventoryManager;

    void Start()
    {
        
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.E))
        {
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            RaycastHit hitInfo;

            if (Physics.Raycast(ray, out hitInfo, 3))
            {
                itemPickable item = hitInfo.collider.gameObject.GetComponent<itemPickable>();
                Storage storage = hitInfo.collider.gameObject.GetComponent<Storage>();

                if (item != null)
                {
                    inventoryManager.ItemPicked(hitInfo.collider.gameObject);
                }

                else if (storage != null)
                {
                    if (Input.GetKeyDown(KeyCode.E) && inventoryManager.isStorageOpened)
                    {
                        inventoryManager.CloseStorage(storage);
                    }
                    else if (Input.GetKeyDown(KeyCode.E) && !inventoryManager.isStorageOpened)
                    {
                        inventoryManager.OpenStorage(storage);
                    }
                }
            }
        }
    }
}
