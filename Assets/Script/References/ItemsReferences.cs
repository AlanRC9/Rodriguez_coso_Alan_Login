using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ItemsReferences : MonoBehaviour
{

    public static ItemsReferences instance;

    [SerializeField] List<ItemSO> items = new List<ItemSO>();

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;

    }

    public ItemSO GetItem(int itemID)
    {
        return items[itemID-1];
    }
}
