using UnityEngine;

[CreateAssetMenu(fileName = "NewItem", menuName = "Inventory/Item")]
public class ItemSO : ScriptableObject
{
    [Header("Database ID")]
    [SerializeField] private int id; // Debe coincidir con object_id en SQLite

    [Header("Basic data")]
    [SerializeField] private string itemName;
    [SerializeField] private Sprite icon;
    [SerializeField] private int price;


    [Header("Collection ID")]
    [SerializeField] private int collectionId;



    public int Id => id;
    public string ItemName => itemName;
    public Sprite Icon => icon;
    public int CollectionId => collectionId;
    public int Price => price;
}