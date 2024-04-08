using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenInventory : MonoBehaviour
{
    public GameObject inventory;
    public GameObject inventoryManager;
    private InventoryManager im;
    void Start()
    {
        im = inventoryManager.GetComponent<InventoryManager>();
    }


    void Update()
    {
        if (Input.GetKeyUp("i"))
        {
            if (inventory.activeInHierarchy == false)
            {
                im.ListItems();
                inventory.SetActive(true);
            }
            else
            {
                while (GameObject.FindWithTag("Item") != null)
                    DestroyImmediate(GameObject.FindWithTag("Item"));
                inventory.SetActive(false);
            }
            
        }
    }
}
