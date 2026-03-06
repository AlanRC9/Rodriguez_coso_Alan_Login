using SQLite4Unity3d;

// TABLA OBJECT \\
[Table("OBJECT")]
public class ObjectDB
{
    [PrimaryKey, AutoIncrement]
    [Column("object_id")]
    public int objectId { get; set; }

    [NotNull]
    [Column("object_collection_id")]
    public int objectCollectionId { get; set; }
}