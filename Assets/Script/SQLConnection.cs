using Mono.Data.Sqlite;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;

public class SQLConnection : MonoBehaviour
{
    private int hitCount;

    // Llegir tots els valors de la taula

    // referencia
    private void Start()
    {
        ReadAllValuesFromDatabase();
    }

    private void ReadAllValuesFromDatabase()
    {
        try
        {
            // Creació/Obertura de la connexió amb la base de dades sobre un fitxer a Assets/MyDatabase.sqlite
            string dbUri = "URI=file:" + Application.dataPath + "/MyDatabase.sqlite"; // 4

            // Creació de la connexió
            IDbConnection dbConnection = new SqliteConnection(dbUri); // 5

            // Obertura de la connexió
            dbConnection.Open(); // 6

            // Creació de la tabla si no existeix
            IDbCommand dbCommandCreateTable = dbConnection.CreateCommand(); // 6
            dbCommandCreateTable.CommandText =
                "CREATE TABLE IF NOT EXISTS HitCountTableSimple (id INTEGER PRIMARY KEY, hits INTEGER )"; // 7
            dbCommandCreateTable.ExecuteNonQuery(); // Execució de una query SQL

            // Creació del comandament de lectura
            IDbCommand dbCommandReadValues = dbConnection.CreateCommand(); // 15
            dbCommandReadValues.CommandText = "SELECT * FROM HitCountTableSimple"; // 16

            // Executem el query y rebem a un IDataReader el resultat
            IDataReader dataReader = dbCommandReadValues.ExecuteReader();

            // Mentre hi hagi linies per llegir
            while (dataReader.Read())
            {
                // Llegim l'entrada y la mostrem per consola
                Debug.Log(dataReader.GetInt32(1)); // Llegim especificament la segona columna com a Integer32
            }

            dbCommandReadValues.Dispose();

            // Recordeu tancar la connexió sempre
            dbConnection.Close();

            Debug.Log("Conexion exitosa.");

        }
        catch (System.Exception ex)
        {
            Debug.Log("Error de la conexion: " + ex.Message);
            throw;
        }

    }

    private IDbConnection CreateAndOpenDatabase() // 3
    {
        
        // Open a connection to the database.
        string dbUri = "URI=file:" + Application.dataPath + "/MyDatabase.sqlite"; // 4
        IDbConnection dbConnection = new SqliteConnection(dbUri); // 5
        dbConnection.Open(); // 6

        // Create a table for the hit count in the database if it does not exist yet.
        IDbCommand dbCommandCreateTable = dbConnection.CreateCommand(); // 6
        dbCommandCreateTable.CommandText =
            "CREATE TABLE IF NOT EXISTS HitCountTableSimple (id INTEGER PRIMARY KEY, hits INTEGER )"; // 7
        dbCommandCreateTable.ExecuteReader(); // 8

        return dbConnection;
        
    }

    // Mensaje de Unity | referencias
    private void OnMouseDown()
    {
        hitCount++;

        // Insert hits into the table.
        IDbConnection dbConnection = CreateAndOpenDatabase(); // 2
        IDbCommand dbCommandInsertValue = dbConnection.CreateCommand(); // 9
        dbCommandInsertValue.CommandText =
            "INSERT OR REPLACE INTO HitCountTableSimple (id, hits) VALUES (0, " + hitCount + ")"; // 10
        dbCommandInsertValue.ExecuteNonQuery(); // 11


        // Remember to always close the connection at the end.
        dbConnection.Close(); // 12

        Debug.Log("OnMouse funciona.");
    }




}
