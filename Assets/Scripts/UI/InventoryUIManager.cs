using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class InventoryUIManager : MonoBehaviour
{
    public static InventoryUIManager Instance;

    [SerializeField] private List<GameObject> UIInventory = new List<GameObject>();


    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

    }

    private void Start()
    {

        foreach (Transform child in transform)
        {
            child.gameObject.GetComponent<UISlot>().SetID(UIInventory.Count);
            UIInventory.Add(child.gameObject);
            
        }

        foreach (var item in UIInventory)
        {
            Debug.Log("Child GameObject: " + item.name);
        }

        FillSlots();

    }

    public void FillSlots()
    {
        List<Slot> inventory = Inventory.Instance.GetInventory();

        for (int i = 0; i < UIInventory.Count; i++)
        {
            int? objectID = inventory[i].objectId;
            if (objectID != null)
            {
                ItemSO item = ItemsReferences.instance.GetItem(objectID.Value);

                UIInventory[i].GetComponent<UISlot>().SetItem(item.Icon, item.ItemName, inventory[i].quantity.ToString(), CollectionReferences.instance.GetCollection(item.CollectionId));
            }
            else UIInventory[i].GetComponent<UISlot>().SetItem(null, "", "", "");

        }
    }

    public void RemoveItem(int slotID)
    {
        Inventory.Instance.RemoveItem(slotID);
        FillSlots();
    }
}
