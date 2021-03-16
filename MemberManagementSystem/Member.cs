using System.Collections.Generic;

namespace MemberManagementSystem
{
    class Member
    {
        public string Name { get; set; }
        public string Address { get; set; }
        public List<Account> Accounts { get; set; }
    }
}