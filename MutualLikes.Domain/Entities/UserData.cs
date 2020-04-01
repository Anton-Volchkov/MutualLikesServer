using System;
using System.Collections.Generic;
using System.Text;

namespace MutualLikes.Domain.Entities
{
    public class UserData
    {
        public int Id { get; set; }
        public long UserId { get; set; }
        public string FullUserName { get; set; }
        public bool UserChecked { get; set; }
        public string DataLastChecked { get; set; }
    }
}
