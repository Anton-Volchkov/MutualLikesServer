﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.SignalR;
using MutualLikes.Application.Likes.Queries.GetUsersWithMutualLikes;
using MutualLikes.Application.Users.Commands.AddOrUpdateUserData;

namespace Server.Hubs
{
    public class ServerHub : Hub
    {
        private readonly IMediator _mediator;
        public ServerHub(IMediator mediator)
        {
            _mediator = mediator;
        }
        public async Task GetMutualLikes(string userId, string sex)
        {
            var res = await _mediator.Send(new GetUsersWithMutualLikesQuery()
            {
                userId = long.Parse(userId),
                Sex = byte.Parse(sex)
            });

            await _mediator.Send(new AddOrUpdateUserDataQuery()
            {
                UserId = long.Parse(userId)
            });

           await Clients.Caller.SendAsync("SendToCaller", res);
        }
    }
}
