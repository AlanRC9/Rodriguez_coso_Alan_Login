using Mono.Data.Sqlite;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;

public class SQLManager : MonoBehaviour
{
    public static SQLManager Instance { get; private set; }

    private void Awake()
    {
        // Si ya existe una instancia y no somos nosotros, nos destruimos
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        // Asignamos la instancia
        Instance = this;

        // Opcional: persiste entre escenas
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        CreateDatabase();
    }


    private void CreateDatabase()
    {
        try
        {
            // Creación / Apertura de la conexión con la base de datos
            string dbUri = "URI=file:" + Application.dataPath + "/MyDatabase.sqlite";
            IDbConnection dbConnection = new SqliteConnection(dbUri);
            dbConnection.Open();

            // Creación de la tabla Users si no existe
            IDbCommand dbCommandCreateTable = dbConnection.CreateCommand();
            dbCommandCreateTable.CommandText = @"CREATE TABLE IF NOT EXISTS Users (
                UserID INTEGER PRIMARY KEY AUTOINCREMENT,
                Username TEXT UNIQUE NOT NULL,
                Password TEXT NOT NULL
            );";
            dbCommandCreateTable.ExecuteNonQuery();

            // Cerrar conexión
            dbConnection.Close();

            Debug.Log("Base de datos y tabla Users creadas correctamente.");
        }
        catch (System.Exception ex)
        {
            Debug.LogError("Error creando la base de datos: " + ex.Message);
        }
    }


    public bool Login(string username, string password)
    {
        try
        {
            // Creación de la conexión a la base de datos
            string dbUri = "URI=file:" + Application.dataPath + "/MyDatabase.sqlite";
            IDbConnection dbConnection = new SqliteConnection(dbUri);
            dbConnection.Open();

            // Comando de login
            IDbCommand dbCommandLogin = dbConnection.CreateCommand();
            dbCommandLogin.CommandText =
                "SELECT COUNT(*) FROM Users WHERE Username = @username AND Password = @password";


            dbCommandLogin.Parameters.Add(new SqliteParameter("@username", username));
            dbCommandLogin.Parameters.Add(new SqliteParameter("@password", password));

            // Ejecutar la query
            int result = System.Convert.ToInt32(dbCommandLogin.ExecuteScalar());

            // Cerrar conexión
            dbConnection.Close();

            // Si existe exactamente 1 usuario el login correcto
            return result == 1;
        }
        catch (System.Exception ex)
        {
            Debug.LogError("Error en login: " + ex.Message);
            return false;
        }
    }


    public bool Register(string username, string password)
    {
        // Comprobación de largo
        if (password.Length < 8)
        {
            Debug.Log("La contraseña debe tener al menos 8 caracteres.");
            return false;
        }

        try
        {
            // Creación / Apertura de la conexión con la base de datos
            string dbUri = "URI=file:" + Application.dataPath + "/MyDatabase.sqlite";
            IDbConnection dbConnection = new SqliteConnection(dbUri);
            dbConnection.Open();

            // Comando de inserción
            IDbCommand dbCommandRegister = dbConnection.CreateCommand();
            dbCommandRegister.CommandText =
                "INSERT INTO Users (Username, Password) VALUES (@username, @password)";

            // Parámetros (evita SQL injection)
            dbCommandRegister.Parameters.Add(new SqliteParameter("@username", username));
            dbCommandRegister.Parameters.Add(new SqliteParameter("@password", password));

            // Ejecutar la query
            dbCommandRegister.ExecuteNonQuery();

            // Cerrar conexión
            dbConnection.Close();

            Debug.Log("Usuario registrado correctamente.");
            return true;
        }
        catch (System.Exception ex)
        {
            Debug.LogError("Error en el registro (usuario duplicado?): " + ex.Message);
            return false;
        }
    }

}
