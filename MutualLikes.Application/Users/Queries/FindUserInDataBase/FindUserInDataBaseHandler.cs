using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using MutualLikes.DataAccess;

namespace MutualLikes.Application.Users.Queries.FindUserInDataBase
{
    public class FindUserInDataBaseHandler : IRequestHandler<FindUserInDataBaseQuery, bool>
    {
        private readonly ApplicationDbContext _db;

        public FindUserInDataBaseHandler(ApplicationDbContext db)
        {
            _db = db;
        }
        public async Task<bool> Handle(FindUserInDataBaseQuery request, CancellationToken cancellationToken)
        {
            var user = await _db.UserDatas.FirstOrDefaultAsync(x => x.UserId == request.UserId);

            if(user is null)
            {
                return false;
            }

            return user.UserChecked;
        }
    }
}
