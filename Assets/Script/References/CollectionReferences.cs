using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectionReferences : MonoBehaviour
{
    public static CollectionReferences instance;
    public enum Collections
    {
        GreenCollection,   // Green Collection
        BlueCollection,    // Blue Collection
        PurpleCollection,  // Purple Collection
        GoldCollection     // Gold Collection
    }

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;

    }

    public string GetCollection(int collectionID)
    {
        Collections collection = (Collections)collectionID-1;

        return collection.ToString();
    }

}
