using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public static Inventory Instance;

    private int currentUserId;           
    [SerializeField] private List<Slot> slots = new List<Slot>();  


    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void InitializeInventory(int userId)
    {
        currentUserId = userId;
        LoadInventoryFromDatabase();
    }

    private void LoadInventoryFromDatabase()
    {
        slots = SQLManager.Instance.LoadInventory(currentUserId);
        Debug.Log("Inventario cargado. Slots: " + slots.Count);
    }
    public bool AddItem(ItemSO item)
    {
        //Busca y guarda el item en un slot que ya tenga ese objeto y con espacio
        foreach (Slot slot in slots)
        {
            if (slot.objectId == item.Id && !slot.IsFull())
            {
                slot.quantity++;
                SaveInventory();
                return true;
            }
        }

        //Busca un slot vacío
        foreach (Slot slot in slots)
        {
            if (slot.IsEmpty())
            {
                slot.objectId = item.Id;
                slot.quantity = 1;
                SaveInventory();
                return true;
            }
        }

        Debug.Log("No se pudo añadir el objeto al inventario. El inventario está lleno.");

        return false;
    }

    public void RemoveItem(int id)
    {
        if(slots[id].quantity > 0) slots[id].quantity--;
        
        if (slots[id].quantity == 0)
        {
            slots[id].objectId = null;
        }
        SaveInventory();
    }



    public void SaveInventory()
    {
        SQLManager.Instance.SaveInventory(slots, currentUserId);
    }

    public List<Slot> GetInventory() => slots;
}