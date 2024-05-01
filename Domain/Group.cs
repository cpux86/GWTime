namespace Domain;
public class Group
{
    public int Id { get; set; }

    /// <summary>
    /// Название группы
    /// </summary>
    public string Name { get; set; } = string.Empty;
    public List<User>? Users { get; set; }
}
