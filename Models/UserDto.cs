namespace UserNamespace.Models
{
    /// <summary>
    /// Data Transfer Object for User - excludes password hash
    /// </summary>
    public class UserDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
    }
}
