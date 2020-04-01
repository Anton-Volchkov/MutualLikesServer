using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using MutualLikes.DataAccess;
using MutualLikes.Domain.Entities;

namespace MutualLikes.Application.Users.Queries.FindUserInDataBase
{
    public class FindUserInDataBaseHandler : IRequestHandler<FindUserInDataBaseQuery, UserData>
    {
        private readonly ApplicationDbContext _db;

        public FindUserInDataBaseHandler(ApplicationDbContext db)
        {
            _db = db;
        }
        public async Task<UserData> Handle(FindUserInDataBaseQuery request, CancellationToken cancellationToken)
        {
            var user = await _db.UserDatas.FirstOrDefaultAsync(x => x.UserId == request.UserId);

            if(user is null)
            {
                return new UserData()
                {
                    UserChecked = false
                };
            }

            return user;
        }
    }
}
