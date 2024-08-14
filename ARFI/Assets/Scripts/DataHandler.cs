// DataHandler.cs
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataHandler : MonoBehaviour
{
    private GameObject furniture;
    [SerializeField] private ButtonManager buttonprefab;
    [SerializeField] private GameObject buttonContainer;
    [SerializeField] private List<Item> items;

    private int current_id = 0;
    private static DataHandler instance;
    public static DataHandler Instance
    {
        get
        {
            if(instance==null)
            {
                instance = FindObjectOfType<DataHandler>();
            }
            return instance;
        }
    }

    private void Start()
    {
        LoadItems();
        CreateButtons();
    }

    void LoadItems()
    {
        var items_obj = Resources.LoadAll("Items",typeof(Item));
        foreach(var item in items_obj)
        {
            items.Add(item as Item);
        }
    }

    void CreateButtons()
    {
        foreach (Item i in items)
        {
            // Check if buttonprefab has ButtonManager script
            if (buttonprefab.GetComponent<ButtonManager>() != null)
            {
                ButtonManager b = Instantiate(buttonprefab, buttonContainer.transform);
                b.ItemId = current_id;
                b.ButtonTexture = i.itemImage;
                current_id++;
            }
            else
            {
                Debug.LogError("Button prefab does not have a ButtonManager script attached!");
            }
        }
    }

    public void SetFurniture(int id)
    {
        furniture = items[id].itemPrefab;
    }

    public GameObject GetFurniture()
    {
        if (furniture == null)
        {
            return null; // Or return a placeholder object
        }
        return furniture;
    }
}
