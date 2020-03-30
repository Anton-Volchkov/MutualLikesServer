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
    public class GetUsersWithMutualLikesHandler : IRequestHandler<GetUsersWithMutualLikesQuery, GetUsersWithMutualLikesModel>
    {
        private readonly VkFinder _finder;
        private readonly IVkApi _api;
        public GetUsersWithMutualLikesHandler(VkFinder finder, IVkApi api)
        {
            _finder = finder;
            _api = api;
        }
        public async Task<GetUsersWithMutualLikesModel> Handle(GetUsersWithMutualLikesQuery request, CancellationToken cancellationToken)
        {
            long idUser = request.userId;
            byte sex = request.Sex;
            StringBuilder sb = new StringBuilder(2000);

            if (await _finder.CheckAccessToUser(idUser))
            {
                var currentUser = (await _api.Users.GetAsync(new[] { idUser })).FirstOrDefault();

                if(currentUser is null)
                {
                    return new GetUsersWithMutualLikesModel()
                    {
                        Data = "Пользователь не найден!"
                    };
                }

                sb.AppendLine($"\nПользователь: {currentUser.FirstName} {currentUser.LastName}\n");
                sb.AppendLine("\tВзаимные лайки найдены со следующими пользователями:\n");


                var photoIds = await _finder.GetPhotosIds(currentUser.Id);

                if (photoIds.Count == 0)
                {
                    return new GetUsersWithMutualLikesModel()
                        {Data = "У пользователя нет фото."};
                }

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

                        sb.AppendLine($"\t\t{user.FirstName} {user.LastName} (https://vk.com/id{user.Id})\n");
                    }
                }


            }
            else
            {
                return new GetUsersWithMutualLikesModel()
                {
                    Data = "У данного пользователя закрыт профиль."
                };
            }

            return new GetUsersWithMutualLikesModel()
                { Data = sb.ToString() };
        }
    }
}
