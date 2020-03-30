using System;
using System.Collections.Generic;
using System.Text;

namespace MutualLikes.Application.Likes.Queries.GetUsersWithMutualLikes
{
   
    public class GetUsersWithMutualLikesModel
    {
        public string UserName { get; set; }
        public long UserId { get; set; }
        public string AdditionalData { get; set; }
    }
}
