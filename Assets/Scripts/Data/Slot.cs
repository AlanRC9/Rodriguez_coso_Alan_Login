using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class Slot
{
    public int slotId;        
    public int maxCapacity;   
    public int quantity;      
    [SerializeField] public int? objectId;     

    // Devuelve true si el slot no tiene objetos
    public bool IsEmpty()
    {
        return objectId == null || quantity == 0;
    }

    // Devuelve true si el slot está lleno
    public bool IsFull()
    {
        return quantity >= maxCapacity;
    }
}
