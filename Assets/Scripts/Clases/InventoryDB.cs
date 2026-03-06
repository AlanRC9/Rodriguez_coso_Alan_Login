using SQLite4Unity3d;

// TABLA INVENTORY \\
[Table("INVENTORY")]
public class InventoryDB
{
    [PrimaryKey, AutoIncrement]
    [Column("inventory_id")]
    public int inventoryId { get; set; }

    [NotNull]
    [Column("slots_number")]
    public int slotsNumber { get; set; }

    [Unique, NotNull]
    [Column("user_id")]
    public int userId { get; set; }
}