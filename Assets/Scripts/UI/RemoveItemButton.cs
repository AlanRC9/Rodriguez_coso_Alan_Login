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
        InventoryUIManager.Instance.RemoveItem(GetComponentInParent<UISlot>().GetID());
    }

}
