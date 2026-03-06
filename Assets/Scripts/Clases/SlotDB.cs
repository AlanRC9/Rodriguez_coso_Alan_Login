using SQLite4Unity3d;

// TABLA SLOT \\
[Table("SLOT")]
public class SlotDB
{
    [PrimaryKey, AutoIncrement]
    [Column("slot_id")]
    public int slotId { get; set; }

    [NotNull]
    [Column("max_capacity")]
    public int maxCapacity { get; set; }

    [NotNull]
    [Column("quantity")]
    public int quantity { get; set; }

    [NotNull]
    [Column("inventory_id")]
    public int inventoryId { get; set; }

    [Column("object_id")]
    public int? objectId { get; set; }
}