using System;
using System.Collections.Generic;
using System.Text;
using MediatR;
using MutualLikes.Domain.Entities;

namespace MutualLikes.Application.Users.Queries.FindUserInDataBase
{
    public class FindUserInDataBaseQuery : IRequest<UserData>
    {
        public long UserId { get; set; }
    }
}
