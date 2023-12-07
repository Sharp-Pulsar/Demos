namespace Shared.Models
{
    public sealed record Post 
    { 
        public string Connect_Id { get; set; } 
        public string Username { get; set; } 
        public string Message { get; set; } 
        public string Timestamp { get; set; }  
    };
}
