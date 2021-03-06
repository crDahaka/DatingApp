namespace DatingApp.API.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Security.Claims;
    using System.Threading.Tasks;
    using AutoMapper;
    using DatingApp.API.Data;
    using DatingApp.API.Dtos;
    using DatingApp.API.Helpers;
    using DatingApp.API.Models;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    [ServiceFilter(typeof(LogUserActivity))]
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IDatingRepository _repo;
        private readonly IMapper _mapper;

        public UsersController(IDatingRepository repo, IMapper mapper)
        {
            _mapper = mapper;
            _repo = repo;
        }

        [HttpGet]
        public async Task<IActionResult> GetUsers([FromQuery]UserParams userParams)
        {
            var currentUserID = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var userFromRepo = await _repo.GetUser(currentUserID);
            userParams.UserId = currentUserID;

            if (string.IsNullOrEmpty(userParams.Gender))
            {
                userParams.Gender = userFromRepo.Gender == "male" ? "female" : "male";
            }

            var users = await _repo.GetUsers(userParams);
            Response.AddPagination(users.CurrentPage, users.PageSize, users.TotalCount, users.TotalPages);

            return Ok(_mapper.Map<IEnumerable<UserListDto>>(users));
        }

        [HttpGet("{id}", Name=nameof(GetUser))]
        public async Task<IActionResult> GetUser(int id)
        {
            return Ok(_mapper.Map<UserDetailsDto>(await _repo.GetUser(id)));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> EditUser(int id, UserEditDto userDto)
        {
            if (id != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value)) return Unauthorized();

            _mapper.Map(userDto, await _repo.GetUser(id));
            if (await _repo.SaveAll()) return NoContent();

            throw new Exception($"Updating user {id} failed on save!");
        }

        [HttpPost("{id}/like/{recipientId}")]
        public async Task<IActionResult> LikeUser(int id, int recipientId)
        {
            if (id != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value)) return Unauthorized();

            var like = await _repo.GetLike(id, recipientId);
            
            if (like != null) return BadRequest("You have already liked this user.");
            if (await _repo.GetUser(recipientId) == null) return NotFound();

            _repo.Add<Like>(new Like{ LikerId = id, LikeeId = recipientId });

            if (await _repo.SaveAll())
            {
                return Ok();
            }

            return BadRequest("Failed to like user.");
        }
    }
}