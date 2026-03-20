using SQLite4Unity3d;

// TABLA USERS \\
[Table("USERS")]
public class User
{
    [PrimaryKey, AutoIncrement]
    [Column("user_id")]
    public int userId { get; set; }

    [Unique, NotNull]
    [Column("user_name")]
    public string userName { get; set; }

    [NotNull]
    [Column("user_password")]
    public string userPassword { get; set; }

    [Column("money")]
    public int money { get; set; }
}