namespace BakeryNamespace.Models
{
    public class LogMessage
    {
        public DateTime StartTime { get; set; }
        public string Controller { get; set; } = string.Empty;
        public string Action { get; set; } = string.Empty;
        public string User { get; set; } = string.Empty;
        public long DurationMs { get; set; }
    }
}