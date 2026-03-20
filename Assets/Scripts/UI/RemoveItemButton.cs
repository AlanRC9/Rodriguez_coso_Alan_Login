using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RemoveItemButton : MonoBehaviour
{

    private Button removeItemButton;


    void Start()
    {
        removeItemButton = GetComponent<Button>();
        removeItemButton.onClick.AddListener(OnRemoveItemButtonClicked);
    }

    private void OnRemoveItemButtonClicked()
    {
        if (GetComponentInParent<UISlot>().GetItemID() != -1)
        {
            SQLManager.Instance.AddMoney(ItemsReferences.instance.GetItem(GetComponentInParent<UISlot>().GetItemID()).Price);
            UIMoeyManager.Instance.UpdateMoney();
        }
        InventoryUIManager.Instance.RemoveItem(GetComponentInParent<UISlot>().GetID());

    }

}
