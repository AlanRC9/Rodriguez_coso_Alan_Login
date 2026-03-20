using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemPurchaseButton : MonoBehaviour
{

    [SerializeField] private ItemSO itemToPurchase;
    private TextMeshProUGUI priceText;

    private Button purchaseButton;

    private void Awake()
    {
        priceText = GetComponentInChildren<TextMeshProUGUI>();
    }

    void Start()
    {
        purchaseButton = GetComponent<Button>();
        purchaseButton.onClick.AddListener(OnPurchaseButtonClicked);

        priceText.text = itemToPurchase.Price.ToString();
    }

    private void OnPurchaseButtonClicked()
    {
        Purchase();
    }

    private void Purchase()
    {
        if (SQLManager.Instance.SpendMoney(itemToPurchase.Price))
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
            UIMoeyManager.Instance.UpdateMoney();
    }
}
