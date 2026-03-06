using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemPurchaseButton : MonoBehaviour
{

    [SerializeField] private ItemSO itemToPurchase;

    private Button purchaseButton;


    void Start()
    {
        purchaseButton = GetComponent<Button>();
        purchaseButton.onClick.AddListener(OnPurchaseButtonClicked);
    }

    private void OnPurchaseButtonClicked()
    {
        Purchase();
    }

    private void Purchase()
    {
        bool itemAdded = Inventory.Instance.AddItem(itemToPurchase);
        InventoryUIManager.Instance.FillSlots();

        if (itemAdded)
        {
            Debug.Log("Item added to inventory");
            //espacio para agregar funcionalidades si me da tiempo
        }
        else
        {
            Debug.Log("Failed to add item to inventory");
        }
    }
}
