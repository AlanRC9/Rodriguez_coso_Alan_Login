using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UISlot : MonoBehaviour
{
    [SerializeField] private Image itemImage;
    [SerializeField] private TextMeshProUGUI itemName;
    [SerializeField] private TextMeshProUGUI itemQuantity;
    [SerializeField] private TextMeshProUGUI itemCollection;
    [SerializeField] private int itemID;
    [SerializeField] private int slotID;

    private void Start()
    {
        itemImage = GetComponentInChildren<Image>();
    }
    
    public void SetItem(Sprite ItemSprite, string itemName, string itemQuantity, string itemCollection, int? itemID)
    {
        itemImage.sprite = ItemSprite;
        this.itemName.text = itemName;
        this.itemQuantity.text = itemQuantity;
        this.itemCollection.text = itemCollection;

        if (itemID.HasValue) this.itemID = itemID.Value;
        else this.itemID = -1;
    }


    public void SetID(int id)
    {
        slotID = id;
    }

    public int GetID() => slotID;

    public int GetItemID() => itemID;

}
