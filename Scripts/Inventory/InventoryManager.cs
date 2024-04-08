using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance;
    public List<Item> Items = new List<Item>();
    public Transform ItemContent;
    public GameObject InventoryItem;

    private void Awake()
    {
        Instance = this;
    }

    public void Add(Item item)
    {
        Items.Add(item);
    }

    public void Remove(Item item)
    {
        Items.Remove(item);
    }

    public void ListItems()
    {
        foreach(var item in Items)
        {
            GameObject obj = Instantiate(InventoryItem, ItemContent);
            TextMeshProUGUI mText = obj.GetComponent<RectTransform>().transform.GetChild(0).GetComponent<TextMeshProUGUI>();
            mText.text = item.name;
            Image sprt = obj.GetComponent<Image>();
            sprt.sprite = item.icon;
        }
    }
}
