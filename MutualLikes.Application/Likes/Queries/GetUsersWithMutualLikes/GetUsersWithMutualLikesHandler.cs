using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using MutualLikes.Application.VK;
using VkNet.Abstractions;
using VkNet.Enums;
using VkNet.Enums.Filters;

namespace MutualLikes.Application.Likes.Queries.GetUsersWithMutualLikes
{
    public class GetUsersWithMutualLikesHandler : IRequestHandler<GetUsersWithMutualLikesQuery, List<GetUsersWithMutualLikesModel>>
    {
        private readonly VkFinder _finder;
        private readonly IVkApi _api;
        public GetUsersWithMutualLikesHandler(VkFinder finder, IVkApi api)
        {
            _finder = finder;
            _api = api;
        }
        public async Task<List<GetUsersWithMutualLikesModel>> Handle(GetUsersWithMutualLikesQuery request, CancellationToken cancellationToken)
        {
            long idUser = request.userId;
            byte sex = request.Sex;
          
            var users = new List<GetUsersWithMutualLikesModel>();

            if (await _finder.CheckAccessToUser(idUser))
            {
                var currentUser = (await _api.Users.GetAsync(new[] { idUser })).FirstOrDefault();

                if(currentUser is null)
                {
                    users.Add(new GetUsersWithMutualLikesModel()
                    {
                        UserName = $"Неизвестный пользователь",
                        UserId = idUser,
                        AdditionalData = $"Пользователь с ID {idUser} не найден."
                    });

                    return users;
                }



                var photoIds = await _finder.GetPhotosIds(currentUser.Id);

                if (photoIds.Count == 0)
                {
                    users.Add(new GetUsersWithMutualLikesModel()
                    {
                        UserName = $"{currentUser.FirstName} {currentUser.LastName}",
                        UserId = currentUser.Id,
                        AdditionalData = "У пользователя нет фото."
                    });

                    return users;
                }

                //Если вссе проверки проши, то наш нулевой пользователь в массиве, это тот пользователь для которого делается проверка, у него не должно быть описания
                users.Add(new GetUsersWithMutualLikesModel()
                {
                    UserName = $"{currentUser.FirstName} {currentUser.LastName}",
                    UserId = currentUser.Id
                });

                var usersWhoLiked = await _finder.GetAllLikesByPhotoIds(currentUser.Id, photoIds);

                foreach (var ownerId in usersWhoLiked)
                {
                    var user = (await _api.Users.GetAsync(new[] { ownerId }, ProfileFields.Sex)).First();

                    if (sex != 2)
                    {
                        if (user.Sex == Sex.Female && sex != 0)
                        {
                            continue;
                        }
                        else if (user.Sex == Sex.Male && sex != 1)
                        {
                            continue;
                        }

                    }

                    var idPhotos = await _finder.GetPhotosIds(ownerId);

                    if (idPhotos.Count == 0)
                    {
                        continue;
                    }

                    var hasLike = (await _finder.CheckMutualLikes(ownerId, idPhotos, currentUser.Id));

                    if (hasLike)
                    {
                        users.Add(new GetUsersWithMutualLikesModel()
                        {
                            UserName = $"{user.FirstName} {user.LastName}",
                            UserId = user.Id
                        });
                   
                    }
                }


            }
            else
            {
                var currentUser = (await _api.Users.GetAsync(new[] { idUser })).FirstOrDefault();

                users.Add(new GetUsersWithMutualLikesModel()
                {
                    UserName = $"{currentUser.FirstName} {currentUser.LastName}",
                    UserId = currentUser.Id,
                    AdditionalData = "У данного пользователя закрыт профиль."
                });
            }

            return users;
        }
    }
}
