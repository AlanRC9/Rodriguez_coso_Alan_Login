using Mono.Data.Sqlite;
using SQLite4Unity3d;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.IO;
using System.Linq;
using UnityEngine;

public class SQLManager : MonoBehaviour
{
    public static SQLManager Instance { get; private set; }

    private SQLiteConnection db;

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

    private void Start()
    {
        // DESCOMENTAR CUANDO SE QUIERA BORRAR LA BASE DE DATOS PARA LIMPIARLA TOTALMENTE
        //DeleteDatabaseFile();
        CreateDatabase();
    }

    private void CreateDatabase()
    {
        try
        {
            // Creación / Apertura de la conexión con la base de datos
            string dbPath = Path.Combine(Application.persistentDataPath, "MyDatabase.sqlite");
            db = new SQLiteConnection(dbPath);

            // Creación de todas las tablas (ORM crea las tablas a partir de las clases)
            db.CreateTable<User>();
            db.CreateTable<Collection>();
            db.CreateTable<InventoryDB>();
            db.CreateTable<ObjectDB>();
            db.CreateTable<SlotDB>();

            // Insertar colecciones
            InsertCollections();

            // Insertar objetos (8 objetos, 4 colecciones con 2 objetos cada una)
            InsertObjects();

            Debug.Log("Base de datos y tablas creadas correctamente.");
        }
        catch (System.Exception ex)
        {
            Debug.LogError("Error creando la base de datos: " + ex.Message);
        }
    }

    private void InsertCollections()
    {
        // Insertar las colecciones con los nuevos nombres
        List<Collection> collections = new List<Collection>()
        {
            new Collection{collectionName="Green collection"},
            new Collection{collectionName="Blue collection"},
            new Collection{collectionName="Purple collection"},
            new Collection{collectionName="Gold collection"}
        };

        foreach (var c in collections)
        {
            if (db.Table<Collection>().FirstOrDefault(x => x.collectionName == c.collectionName) == null)
            {
                db.Insert(c);
            }
        }
    }

    private void InsertObjects()
    {
        // Insertar los 8 objetos con sus correspondientes colecciones (colección_id)
        for (int i = 1; i <= 4; i++)
        {
            if (db.Table<ObjectDB>().Count(x => x.objectCollectionId == i) < 2)
            {
                db.Insert(new ObjectDB { objectCollectionId = i });
                db.Insert(new ObjectDB { objectCollectionId = i });
            }
        }
    }

    public bool Login(string username, string password)
    {
        try
        {
            // Buscar usuario con ORM en lugar de SQL
            User user = db.Table<User>()
                .FirstOrDefault(u => u.userName == username && u.userPassword == password);

            if (user != null)
            {
                int userId = user.userId;

                Inventory.Instance.InitializeInventory(userId);

                return true;
            }

            return false;
        }
        catch (System.Exception ex)
        {
            Debug.LogError("Error en login: " + ex.Message);
            return false;
        }
    }

    public bool SignUp(string username, string password)
    {
        if (password.Length < 8)
        {
            Debug.Log("La contraseña debe tener al menos 8 caracteres.");
            return false;
        }

        try
        {
            // Transacción para asegurar que todo se crea correctamente
            db.RunInTransaction(() =>
            {
                //Crear usuario
                User newUser = new User
                {
                    userName = username,
                    userPassword = password
                };

                db.Insert(newUser);

                //Obtener user_id recien creado
                int userId = newUser.userId;

                //Crear inventario con 12 slots
                InventoryDB inventory = new InventoryDB
                {
                    slotsNumber = 12,
                    userId = userId
                };

                db.Insert(inventory);

                //Obtener inventory_id
                int inventoryId = inventory.inventoryId;

                //Crear 12 slots vacios
                for (int i = 0; i < 12; i++)
                {
                    SlotDB slot = new SlotDB
                    {
                        maxCapacity = 5,
                        quantity = 0,
                        inventoryId = inventoryId,
                        objectId = null
                    };

                    db.Insert(slot);
                }
            });

            Debug.Log("Usuario registrado con inventario vacío de 12 slots.");
            return true;
        }
        catch (System.Exception ex)
        {
            Debug.LogError("Error en el registro: " + ex.Message);
            return false;
        }
    }

    public List<Slot> LoadInventory(int userId)
    {
        List<Slot> loadedSlots = new List<Slot>();

        try
        {
            // Buscar inventario del usuario
            InventoryDB inventory = db.Table<InventoryDB>()
                .FirstOrDefault(i => i.userId == userId);

            if (inventory == null)
                return loadedSlots;

            // Obtener slots del inventario
            List<SlotDB> slots = db.Table<SlotDB>()
                .Where(s => s.inventoryId == inventory.inventoryId)
                .OrderBy(s => s.slotId)
                .ToList();

            foreach (SlotDB s in slots)
            {
                Slot slot = new Slot();
                slot.slotId = s.slotId;
                slot.maxCapacity = s.maxCapacity;
                slot.quantity = s.quantity;
                slot.objectId = s.objectId;

                loadedSlots.Add(slot);
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogError("Error cargando inventario: " + ex.Message);
        }

        return loadedSlots;
    }

    public void SaveInventory(List<Slot> slots, int userId)
    {
        try
        {
            // Buscar inventario del usuario
            InventoryDB inventory = db.Table<InventoryDB>()
                .FirstOrDefault(i => i.userId == userId);

            if (inventory == null)
                return;

            // Iniciamos una transacción para asegurarnos de que todo se guarda correctamente
            db.RunInTransaction(() =>
            {
                // Actualizamos los slots en la base de datos
                foreach (Slot slot in slots)
                {
                    SlotDB dbSlot = db.Find<SlotDB>(slot.slotId);

                    if (dbSlot != null && dbSlot.inventoryId == inventory.inventoryId)
                    {
                        dbSlot.quantity = slot.quantity;
                        dbSlot.objectId = slot.objectId;

                        db.Update(dbSlot);
                    }
                }
            });

            Debug.Log("Inventario guardado correctamente.");
        }
        catch (System.Exception ex)
        {
            Debug.LogError("Error guardando el inventario: " + ex.Message);
        }
    }
    public List<User> GetUsersFromDB()
    {
        List<User> users = new List<User>();

        users = db.Table<User>().ToList();

        return users;
    }

    // Elimina a un usuario de la base de datos
    public void DeleteUser(int userId)
    {
        
        User user = db.Find<User>(userId);

        if (user != null) db.Delete(user);
        else Debug.Log("No se encontró el usuario con id: " + userId);
    }

    public void DeleteDatabaseFile()
    {
        string path = Path.Combine(Application.persistentDataPath, "MyDatabase.sqlite");

        if (System.IO.File.Exists(path))
        {
            System.IO.File.Delete(path);
            Debug.Log("Base de datos eliminada");
        }
        else
        {
            Debug.Log("La base de datos no existe");
        }
    }
}
