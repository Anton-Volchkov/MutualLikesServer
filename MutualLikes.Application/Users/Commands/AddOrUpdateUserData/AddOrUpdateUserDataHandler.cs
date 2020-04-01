using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using MutualLikes.DataAccess;
using MutualLikes.Domain.Entities;
using VkNet.Abstractions;

namespace MutualLikes.Application.Users.Commands.AddOrUpdateUserData
{
    public class AddOrUpdateUserDataHandler : IRequestHandler<AddOrUpdateUserDataQuery,int>
    {
        private readonly ApplicationDbContext _db;
        private readonly IVkApi _api;

        public AddOrUpdateUserDataHandler(ApplicationDbContext db, IVkApi api)
        {
            _db = db;
            _api = api;
        }
        public async Task<int> Handle(AddOrUpdateUserDataQuery request, CancellationToken cancellationToken)
        {
            var currentUser = (await _api.Users.GetAsync(new[] { request.UserId })).FirstOrDefault();

            if(currentUser is null)
            {
                return 0;
            }

            var user = await _db.UserDatas.FirstOrDefaultAsync(x => x.UserId == request.UserId);

            if(user is null)
            {
                var newUser = new UserData()
                {
                    UserChecked = true,
                    UserId = request.UserId,
                    FullUserName = $"{currentUser.FirstName} {currentUser.LastName}" ,
                    DataLastChecked = DateTime.Now.ToString("dd.MM.yyyy HH:mm")
                };

                _db.UserDatas.Add(newUser);

                await _db.SaveChangesAsync(cancellationToken);

                return newUser.Id;
            }

            user.DataLastChecked = DateTime.Now.ToString("dd.MM.yyyy HH:mm");
            user.FullUserName = $"{currentUser.FirstName} {currentUser.LastName}";

            await _db.SaveChangesAsync(cancellationToken);

            return user.Id;
        }
    }
}
