using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace MemberManagementSystem
{
    class Account
    {
        [JsonIgnore]
        public string AccountId { get; set; }
        public string Name { get; set; }
        public int Balance { get; set; }
        public string Status { get; set; }

        [JsonIgnore]
        public string MemberName { get; set; }
    }
}
