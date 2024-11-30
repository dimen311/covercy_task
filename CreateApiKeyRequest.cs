using System;
using System.Collections.Generic;

namespace covercy_task
{
    public class CreateApiKeyRequest
    {
        public string UserId { get; set; }
        public List<string> Permissions { get; set; }
    }
}