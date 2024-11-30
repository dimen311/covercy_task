using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace covercy_task
{
    public class ApiKey
    {
        
        [Key]
        public Guid Id { get; set; }
        public string UserId { get; set; }
        public string Key { get; set; }
        public List<string> Permissions { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? LastUsedAt { get; set; }
        public bool IsRevoked { get; set; }
    }
}
