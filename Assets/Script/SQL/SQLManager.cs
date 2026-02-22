using Mono.Data.Sqlite;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;

public class SQLManager : MonoBehaviour
{
    public static SQLManager Instance { get; private set; }

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
            string dbUri = "URI=file:" + Application.persistentDataPath + "/MyDatabase.sqlite";
            IDbConnection dbConnection = new SqliteConnection(dbUri);
            dbConnection.Open();

            // Activar claves foráneas en SQLite
            IDbCommand pragmaCommand = dbConnection.CreateCommand();
            pragmaCommand.CommandText = "PRAGMA foreign_keys = ON;";
            pragmaCommand.ExecuteNonQuery();

            // Creación de todas las tablas
            IDbCommand dbCommandCreateTable = dbConnection.CreateCommand();
            dbCommandCreateTable.CommandText = @"
            CREATE TABLE IF NOT EXISTS USERS (
                user_id INTEGER PRIMARY KEY AUTOINCREMENT,
                user_name TEXT UNIQUE NOT NULL,
                user_password TEXT NOT NULL
            );

            CREATE TABLE IF NOT EXISTS COLLECTIONS (
                collection_id INTEGER PRIMARY KEY AUTOINCREMENT,
                collection_name TEXT UNIQUE NOT NULL
            );

            CREATE TABLE IF NOT EXISTS INVENTORY (
                inventory_id INTEGER PRIMARY KEY AUTOINCREMENT,
                slots_number INTEGER NOT NULL,
                user_id INTEGER NOT NULL UNIQUE,
                FOREIGN KEY (user_id) REFERENCES USERS(user_id)
                    ON DELETE CASCADE
            );

            CREATE TABLE IF NOT EXISTS OBJECT (
                object_id INTEGER PRIMARY KEY AUTOINCREMENT,
                object_collection_id INTEGER NOT NULL,
                FOREIGN KEY (object_collection_id) REFERENCES COLLECTIONS(collection_id)
                    ON DELETE CASCADE
            );

            CREATE TABLE IF NOT EXISTS SLOT (
                slot_id INTEGER PRIMARY KEY AUTOINCREMENT,
                max_capacity INTEGER NOT NULL,
                quantity INTEGER NOT NULL,
                inventory_id INTEGER NOT NULL,
                object_id INTEGER,
                FOREIGN KEY (inventory_id) REFERENCES INVENTORY(inventory_id)
                    ON DELETE CASCADE,
                FOREIGN KEY (object_id) REFERENCES OBJECT(object_id)
                    ON DELETE SET NULL
            );
        ";
            dbCommandCreateTable.ExecuteNonQuery();

            // Insertar colecciones
            InsertCollections(dbConnection);

            // Insertar objetos (8 objetos, 4 colecciones con 2 objetos cada una)
            InsertObjects(dbConnection);

            // Cerrar conexión
            dbConnection.Close();

            Debug.Log("Base de datos y tablas creadas correctamente.");
        }
        catch (System.Exception ex)
        {
            Debug.LogError("Error creando la base de datos: " + ex.Message);
        }
    }

    private void InsertCollections(IDbConnection dbConnection)
    {
        IDbCommand dbCommandInsertCollection = dbConnection.CreateCommand();

        // Insertar las colecciones con los nuevos nombres
        dbCommandInsertCollection.CommandText = @"
        INSERT OR IGNORE INTO COLLECTIONS (collection_name) VALUES
        ('Green collection'),
        ('Blue collection'),
        ('Purple collection'),
        ('Gold collection');
    ";
        dbCommandInsertCollection.ExecuteNonQuery();
    }

    private void InsertObjects(IDbConnection dbConnection)
    {
        // Insertar los 8 objetos con sus correspondientes colecciones (colección_id)
        IDbCommand dbCommandInsertObject = dbConnection.CreateCommand();

        dbCommandInsertObject.CommandText = @"
        INSERT OR IGNORE INTO OBJECT (object_collection_id) VALUES
        (1),
        (1),
        (2),
        (2),
        (3),
        (3),
        (4),
        (4);
        ";

        dbCommandInsertObject.ExecuteNonQuery();
    }


    public bool Login(string username, string password)
    {
        try
        {
            string dbUri = "URI=file:" + Application.persistentDataPath + "/MyDatabase.sqlite";
            IDbConnection dbConnection = new SqliteConnection(dbUri);
            dbConnection.Open();

            IDbCommand dbCommandLogin = dbConnection.CreateCommand();
            dbCommandLogin.CommandText =
                "SELECT user_id FROM USERS WHERE user_name = @username AND user_password = @password";

            dbCommandLogin.Parameters.Add(new SqliteParameter("@username", username));
            dbCommandLogin.Parameters.Add(new SqliteParameter("@password", password));

            object result = dbCommandLogin.ExecuteScalar();

            if (result != null)
            {
                int userId = System.Convert.ToInt32(result);

                dbConnection.Close();

                Inventory.Instance.InitializeInventory(userId);

                return true;
            }

            dbConnection.Close();
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
            string dbUri = "URI=file:" + Application.persistentDataPath + "/MyDatabase.sqlite";
            IDbConnection dbConnection = new SqliteConnection(dbUri);
            dbConnection.Open();

            // Activar claves foraneas
            IDbCommand pragmaCommand = dbConnection.CreateCommand();
            pragmaCommand.CommandText = "PRAGMA foreign_keys = ON;";
            pragmaCommand.ExecuteNonQuery();


            IDbTransaction transaction = dbConnection.BeginTransaction();

            //Crear usuario
            IDbCommand dbCommandRegister = dbConnection.CreateCommand();
            dbCommandRegister.Transaction = transaction;
            dbCommandRegister.CommandText =
                "INSERT INTO Users (user_name, user_password) VALUES (@username, @password);";
            dbCommandRegister.Parameters.Add(new SqliteParameter("@username", username));
            dbCommandRegister.Parameters.Add(new SqliteParameter("@password", password));
            dbCommandRegister.ExecuteNonQuery();

            //Obtener user_id recien creado
            IDbCommand getUserIdCommand = dbConnection.CreateCommand();
            getUserIdCommand.Transaction = transaction;
            getUserIdCommand.CommandText = "SELECT last_insert_rowid();";
            int userId = System.Convert.ToInt32(getUserIdCommand.ExecuteScalar());

            //Crear inventario con 20 slots
            IDbCommand createInventoryCommand = dbConnection.CreateCommand();
            createInventoryCommand.Transaction = transaction;
            createInventoryCommand.CommandText =
                "INSERT INTO INVENTORY (slots_number, user_id) VALUES (12, @user_id);";
            createInventoryCommand.Parameters.Add(new SqliteParameter("@user_id", userId));
            createInventoryCommand.ExecuteNonQuery();

            //Obtener inventory_id
            IDbCommand getInventoryIdCommand = dbConnection.CreateCommand();
            getInventoryIdCommand.Transaction = transaction;
            getInventoryIdCommand.CommandText = "SELECT last_insert_rowid();";
            int inventoryId = System.Convert.ToInt32(getInventoryIdCommand.ExecuteScalar());

            //Crear 20 slots vacios
            for (int i = 0; i < 12; i++)
            {
                IDbCommand createSlotCommand = dbConnection.CreateCommand();
                createSlotCommand.Transaction = transaction;
                createSlotCommand.CommandText =
                    @"INSERT INTO SLOT (max_capacity, quantity, inventory_id, object_id)
                  VALUES (5, 0, @inventory_id, NULL);";
                createSlotCommand.Parameters.Add(new SqliteParameter("@inventory_id", inventoryId));
                createSlotCommand.ExecuteNonQuery();
            }

            //Confirmar transaccion
            transaction.Commit();

            dbConnection.Close();

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
            string dbUri = "URI=file:" + Application.persistentDataPath + "/MyDatabase.sqlite";
            IDbConnection dbConnection = new SqliteConnection(dbUri);
            dbConnection.Open();

            IDbCommand dbCommand = dbConnection.CreateCommand();
            dbCommand.CommandText = @"
                SELECT s.slot_id, s.max_capacity, s.quantity, s.object_id
                FROM SLOT s
                JOIN INVENTORY i ON s.inventory_id = i.inventory_id
                WHERE i.user_id = @userId
                ORDER BY s.slot_id ASC
            ";

            dbCommand.Parameters.Add(new SqliteParameter("@userId", userId));

            IDataReader reader = dbCommand.ExecuteReader();

            while (reader.Read())
            {
                Slot slot = new Slot();
                slot.slotId = reader.GetInt32(0);
                slot.maxCapacity = reader.GetInt32(1);
                slot.quantity = reader.GetInt32(2);
                slot.objectId = reader.IsDBNull(3) ? (int?)null : reader.GetInt32(3);

                loadedSlots.Add(slot);
            }

            reader.Close();
            dbConnection.Close();
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
            string dbUri = "URI=file:" + Application.persistentDataPath + "/MyDatabase.sqlite";
            IDbConnection dbConnection = new SqliteConnection(dbUri);
            dbConnection.Open();

            // Iniciamos una transacción para asegurarnos de que todo se guarda correctamente
            IDbTransaction transaction = dbConnection.BeginTransaction();

            // Actualizamos los slots en la base de datos
            foreach (Slot slot in slots)
            {
                IDbCommand dbCommand = dbConnection.CreateCommand();
                dbCommand.Transaction = transaction;
                dbCommand.CommandText = @"
                    UPDATE SLOT 
                    SET quantity = @quantity, object_id = @objectId 
                    WHERE slot_id = @slotId AND inventory_id IN 
                    (SELECT inventory_id FROM INVENTORY WHERE user_id = @userId)";

                // Asignar parámetros
                dbCommand.Parameters.Add(new SqliteParameter("@quantity", slot.quantity));
                dbCommand.Parameters.Add(new SqliteParameter("@objectId", slot.objectId.HasValue ? (object)slot.objectId.Value : DBNull.Value)); // Si el objeto es null, se usa DBNull
                dbCommand.Parameters.Add(new SqliteParameter("@slotId", slot.slotId));
                dbCommand.Parameters.Add(new SqliteParameter("@userId", userId));

                dbCommand.ExecuteNonQuery();
            }

            // Confirmar la transacción
            transaction.Commit();

            dbConnection.Close();

            Debug.Log("Inventario guardado correctamente.");
        }
        catch (System.Exception ex)
        {
            Debug.LogError("Error guardando el inventario: " + ex.Message);
        }
    }

    public void DeleteDatabaseFile()
    {
        string path = Application.persistentDataPath + "/MyDatabase.sqlite";

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
