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
    [Route("api/users/{userId}/[controller]")]
    [ApiController]
    public class MessagesController : ControllerBase
    {
        private readonly IDatingRepository _repo;
        private readonly IMapper _mapper;

        public MessagesController(IDatingRepository repo, IMapper mapper)
        {
            _mapper = mapper;
            _repo = repo;
        }

        [HttpGet("{id}", Name = nameof(GetMessage))]
        public async Task<IActionResult> GetMessage(int userId, int id)
        {
            if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value)) return Unauthorized();

            var messageFromRepo = await _repo.GetMessage(id);

            if (messageFromRepo == null) return NotFound();

            return Ok(messageFromRepo);
        }

        [HttpGet]
        public async Task<IActionResult> GetMessagesForUser(int userId, [FromQuery]MessageParams messageParams)
        {
            if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value)) return Unauthorized();

            messageParams.UserId = userId;

            var messagesFromRepo = await _repo.GetMessagesForUser(messageParams);
            
            var messages = _mapper.Map<IEnumerable<MessageReturnDto>>(messagesFromRepo);

            Response.AddPagination(messagesFromRepo.CurrentPage, messagesFromRepo.PageSize, messagesFromRepo.TotalCount, messagesFromRepo.TotalPages);

            return Ok(messages);
        }

        [HttpGet("thread/{recipientId}")]
        public async Task<IActionResult> GetMessageThread(int userId, int recipientId)
        {
            if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value)) return Unauthorized();
            
            var messagesFromRepo = await _repo.GetMessageThread(userId, recipientId);

            return Ok(_mapper.Map<IEnumerable<MessageReturnDto>>(messagesFromRepo));
        }

        [HttpPost]
        public async Task<IActionResult> CreateMessage(int userId, [FromBody]MessageCreateDto messageDto)
        {
            var sender = await _repo.GetUser(userId);
            if (sender.Id != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value)) return Unauthorized();

            messageDto.SenderId = userId;

            var recipient = await _repo.GetUser(messageDto.RecipientId);
            if (recipient == null) return BadRequest("Could not find user.");

            var message = _mapper.Map<Message>(messageDto);
            _repo.Add(message);

            if (await _repo.SaveAll())
            {
                var messageToReturn = _mapper.Map<MessageReturnDto>(message);
                return CreatedAtRoute(nameof(GetMessage), new { id = message.Id}, messageToReturn);
            }

            throw new Exception("Creating the message faied on save.");
        }

        [HttpPost("{id}")]
        public async Task<IActionResult> DeleteMessage(int id, int userId)
        {
            if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value)) return Unauthorized();

            var messageFromRepo = await _repo.GetMessage(id);

            if (messageFromRepo.SenderId == userId)
            {
                messageFromRepo.SenderDeleted = true;
            }
            else if (messageFromRepo.RecipientId == userId)
            {
                messageFromRepo.RecipientDeleted = true;
            }
            else if (messageFromRepo.SenderDeleted && messageFromRepo.RecipientDeleted)
            {
                _repo.Delete(messageFromRepo);
            }

            if (await _repo.SaveAll())
            {
                return NoContent();
            }

            throw new Exception("Failed to delete the message.");
        }

        [HttpPost("{id}/read")]
        public async Task<IActionResult> MarkMessageAsRead(int userId, int id)
        {
            if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value)) return Unauthorized();

            var message = await _repo.GetMessage(id);

            if (message.RecipientId != userId) return Unauthorized();

            message.IsRead = true;
            message.DateRead = DateTime.Now;

            await _repo.SaveAll();
            return NoContent();   
        }
    }
}