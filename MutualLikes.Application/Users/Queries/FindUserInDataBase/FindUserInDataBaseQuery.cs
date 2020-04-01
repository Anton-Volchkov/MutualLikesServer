using System;
using System.Collections.Generic;
using System.Text;
using MediatR;

namespace MutualLikes.Application.Users.Queries.FindUserInDataBase
{
    public class FindUserInDataBaseQuery : IRequest<bool>
    {
        public long UserId { get; set; }
    }
}
