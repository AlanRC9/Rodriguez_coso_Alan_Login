using SQLite4Unity3d;

// TABLA COLLECTIONS \\
[Table("COLLECTIONS")]
public class Collection
{
    [PrimaryKey, AutoIncrement]
    [Column("collection_id")]
    public int collectionId { get; set; }

    [Unique, NotNull]
    [Column("collection_name")]
    public string collectionName { get; set; }
}