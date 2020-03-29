using System;
using System.Collections.Generic;
using System.Text;
using MediatR;

namespace MutualLikes.Application.Likes.Queries.GetUsersWithMutualLikes
{
    public class GetUsersWithMutualLikesQuery : IRequest<GetUsersWithMutualLikesModel>
    {
        public long userId { get; set; }
        public byte Sex { get; set; }
    }
}
