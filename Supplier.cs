public class Supplier : IEntity
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    // שדות נוספים: רשימת קבלות, חובות, וכו'
}

public class Survey : IEntity
{
    public int Id { get; set; }
    public string Title { get; set; }
    public int EventId { get; set; }
    // שדות נוספים: שאלות, תאריך סגירה, וכו'
}
