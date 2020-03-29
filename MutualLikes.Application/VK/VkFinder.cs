using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VkNet;
using VkNet.Abstractions;
using VkNet.Enums.Filters;
using VkNet.Enums.SafetyEnums;
using VkNet.Model.RequestParams;

namespace MutualLikes.Application.VK
{
    public class VkFinder
    {
        private readonly IVkApi api;

        public VkFinder(IVkApi api)
        {
            this.api = api;
        }

        public async Task<bool> CheckAccessToUser(long userId)
        {
            var user = (await api.Users.GetAsync(new[] { userId }, ProfileFields.All)).First();

            if (user.IsDeactivated || !user.CanAccessClosed.Value)
            {
                return false;
            }

            return true;
        }

        public async Task<bool> CheckMutualLikes(long ownerId, IEnumerable<long> photoIds, long currentUser)
        {
            if (!await CheckAccessToUser(ownerId))
            {
                return false;
            }

            foreach (var photoId in photoIds)
            {
                bool _isCopied;

                if (api.Likes.IsLiked(itemId: photoId, ownerId: ownerId, userId: currentUser, type: LikeObjectType.Photo,
                                      copied: out _isCopied))
                {
                    return true;
                }
            }

            return false;
        }

        public async Task<List<long>> GetAllLikesByPhotoIds(long userId, IEnumerable<long> PhotoIds)
        {
            if (!await CheckAccessToUser(userId))
            {
                return new List<long>();
            }

            var userIds = new List<long>();
            foreach (var photoId in PhotoIds)
            {
                var IdUserlikesOnPhoto = await GetLikesOnPhoto(photoId, userId);
                foreach (var id in IdUserlikesOnPhoto)
                    if (!userIds.Contains(id))
                    {
                        userIds.Add(id);
                    }
            }

            return userIds;
        }


        public async Task<List<long>> GetPhotosIds(long userId)
        {
            if (!await CheckAccessToUser(userId))
            {
                return new List<long>();
            }

            var photoIds = new List<long>();

            var photos = api.Photo.Get(new PhotoGetParams
            {
                OwnerId = userId,
                AlbumId = PhotoAlbumType.Profile,
            });

            foreach (var photo in photos) photoIds.Add(photo.Id.Value);

            photos = api.Photo.Get(new PhotoGetParams
            {
                OwnerId = userId,
                AlbumId = PhotoAlbumType.Wall,
            });

            foreach (var photo in photos)
                if (!photoIds.Contains(photo.Id.Value))
                {
                    photoIds.Add(photo.Id.Value);
                }

            return photoIds;
        }

        public async Task<List<long>> GetLikesOnPhoto(long photoId, long userId)
        {
            if (!await CheckAccessToUser(userId))
            {
                return new List<long>();
            }

            return api.Likes.GetList(new LikesGetListParams
            {
                OwnerId = userId,
                Type = LikeObjectType.Photo,
                ItemId = photoId,
                Filter = LikesFilter.Likes,
                Count = 500,
                SkipOwn = true
            }).ToList();
        }
    }
}
