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

namespace MutualLikes.Application.Users.Queries.FindUserInDataBase
{
    public class FindUserInDataBaseHandler : IRequestHandler<FindUserInDataBaseQuery, UserData>
    {
        private readonly ApplicationDbContext _db;
        private readonly IVkApi _api;

        public FindUserInDataBaseHandler(ApplicationDbContext db, IVkApi api)
        {
            _db = db;
            _api = api;
        }
        public async Task<UserData> Handle(FindUserInDataBaseQuery request, CancellationToken cancellationToken)
        {
            var currentUser = (await _api.Users.GetAsync(new[] { request.UserId })).FirstOrDefault();

            if(currentUser is null)
            {
                return new UserData()
                {
                    UserChecked = false,
                    FullUserName = $"Неизвестный пользователь"
                };
            }

            var user = await _db.UserDatas.FirstOrDefaultAsync(x => x.UserId == request.UserId);

            if(user is null)
            {
                return new UserData()
                {
                    UserChecked = false,
                    FullUserName = $"{currentUser.FirstName} {currentUser.LastName}"
                };
            }

            user.FullUserName = $"{currentUser.FirstName} {currentUser.LastName}";
            await _db.SaveChangesAsync(cancellationToken);

            return user;
        }
    }
}
