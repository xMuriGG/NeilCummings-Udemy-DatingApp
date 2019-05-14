using AutoMapper;
using DatingApp.API.Data;
using DatingApp.API.Dtos;
using DatingApp.API.Helpers;
using DatingApp.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace DatingApp.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    [ServiceFilter(typeof(LogUserActivity))]
    public class UserController : ControllerBase
    {
        private readonly IDatingRepository _repo;
        private readonly IMapper _mapper;
        public UserController(IDatingRepository repo, IMapper mapper)
        {
            this._mapper = mapper;
            this._repo = repo;
        }

        [HttpGet]
        public async Task<IActionResult> GetUsers([FromQuery]UserParams userParams)
        {
            var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            userParams.UserId = currentUserId;
            //var userFromRepo = await _repo.GetUserAsync(currentUserId);
            //userParams.UserId = userFromRepo.Id;
            //if (string.IsNullOrEmpty(userParams.Gender))
            //{
            //    userParams.Gender = userFromRepo.Gender == "male" ? "female" : "male";
            //}

            var pagedList = await _repo.GetUsersAsync(userParams);
            var userToReturn = _mapper.Map<IEnumerable<UserForListDto>>(pagedList);
            Response.AddPagination(pagedList.CurrentPage, pagedList.PageSize, pagedList.TotalCount, pagedList.TotalPages);

            return Ok(userToReturn);
        }
        [HttpGet("{id}", Name = "GetUser")]
        public async Task<IActionResult> GetUser(int id)
        {
            var user = await _repo.GetUserAsync(id);
            var userToReturn = _mapper.Map<UserForDetailsDto>(user);
            return Ok(userToReturn);
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id, UserForUpdateDto userForUpdateDto)
        {
            if (id != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
            {
                return Unauthorized();
            }

            var user = await _repo.GetUserAsync(id);

            _mapper.Map(userForUpdateDto, user);

            try
            {
                await _repo.SaveAll();
                return NoContent();
            }
            catch (System.Exception)
            {
                throw new System.Exception($"Updating user {id} failed on save");
            }
        }
        [HttpPost("{id}/like/{recipientId}")]
        public async Task<IActionResult> LikeUser(int id, int recipientId)
        {
            if (id != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
            {
                return Unauthorized();
            }
            var like = await _repo.GetLikeAsync(id, recipientId);
            if (like != null)
            {
                return BadRequest("You already like this user.");
            }
            if (! await _repo.UserExistsAsync(recipientId))
            {
                return NotFound($"User with id:{recipientId} doesn't exist.");
            }

            like = new Like()
            {
                LikerId = id,
                LikeeId = recipientId,
                DateLiked = DateTime.Now
            };
            _repo.Add<Like>(like);

            if (await _repo.SaveAll())
            {
                return Ok(like);
            }

            return BadRequest("Failed to like user");
        }
    }
}