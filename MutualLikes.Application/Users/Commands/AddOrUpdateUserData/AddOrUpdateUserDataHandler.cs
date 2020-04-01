using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using MutualLikes.DataAccess;
using MutualLikes.Domain.Entities;

namespace MutualLikes.Application.Users.Commands.AddOrUpdateUserData
{
    public class AddOrUpdateUserDataHandler : IRequestHandler<AddOrUpdateUserDataQuery,int>
    {
        private readonly ApplicationDbContext _db;

        public AddOrUpdateUserDataHandler(ApplicationDbContext db)
        {
            _db = db;
        }
        public async Task<int> Handle(AddOrUpdateUserDataQuery request, CancellationToken cancellationToken)
        {
            var user = await _db.UserDatas.FirstOrDefaultAsync(x => x.UserId == request.UserId);

            if(user is null)
            {
                var newUser = new UserData()
                {
                    UserChecked = true,
                    UserId = request.UserId,
                    DataLastChecked = DateTime.Now.ToString("dd.MM.yyyy hh:mm")
                };

                _db.UserDatas.Add(newUser);

                await _db.SaveChangesAsync(cancellationToken);

                return newUser.Id;
            }

            user.DataLastChecked = DateTime.Now.ToString("dd.MM.yyyy hh:mm");

            await _db.SaveChangesAsync(cancellationToken);

            return user.Id;
        }
    }
}
