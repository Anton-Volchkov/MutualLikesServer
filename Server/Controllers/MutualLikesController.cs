using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MutualLikes.Application.Likes.Queries.GetUsersWithMutualLikes;

namespace MutualLikes.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class MutualLikesController : ControllerBase
    {

        private readonly ILogger<MutualLikesController> _logger;
        private readonly IMediator _mediator;

        public MutualLikesController(ILogger<MutualLikesController> logger, IMediator mediator)
        {
            _logger = logger;
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<GetUsersWithMutualLikesModel> Get(long userId, byte sex)
        {
            return await _mediator.Send(new GetUsersWithMutualLikesQuery() { userId = userId, Sex = sex });
        }
    }
}