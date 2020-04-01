using System;
using System.Collections.Generic;
using System.Text;
using MediatR;

namespace MutualLikes.Application.Users.Commands.AddOrUpdateUserData
{
    public class AddOrUpdateUserDataQuery : IRequest<int>
    {
        public long UserId { get; set; }
    }
}
