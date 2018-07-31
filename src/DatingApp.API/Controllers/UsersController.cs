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
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    [ServiceFilter(typeof(LogUserActivity))]
    [Authorize]
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
    }
}