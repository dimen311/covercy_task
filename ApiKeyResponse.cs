using System;
using System.Collections.Generic;

namespace covercy_task
{
    public class ApiKeyResponse
    {        
        public Guid Id { get; set; }
        public string Key { get; set; }
        public List<string> Permissions { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? LastUsedAt { get; set; }
        public bool IsRevoked { get; set; }
    }
}