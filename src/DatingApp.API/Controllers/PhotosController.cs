namespace DatingApp.API.Controllers
{
    using System.Linq;
    using System.Security.Claims;
    using System.Threading.Tasks;
    using AutoMapper;
    using CloudinaryDotNet;
    using CloudinaryDotNet.Actions;
    using DatingApp.API.Data;
    using DatingApp.API.Dtos;
    using DatingApp.API.Helpers;
    using DatingApp.API.Models;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Options;

    [Route("api/users/{userId}/photos")]
    [ApiController]
    public class PhotosController : ControllerBase
    {
        private readonly IDatingRepository _repo;
        private readonly IMapper _mapper;
        private readonly IOptions<CloudinarySettings> _cloudinaryConfig;
        private Cloudinary _cloudinary;

        public PhotosController(IDatingRepository repo, IMapper mapper, IOptions<CloudinarySettings> cloudinaryConfig)
        {
            _cloudinaryConfig = cloudinaryConfig;
            _mapper = mapper;
            _repo = repo;

            Account acc = new Account(_cloudinaryConfig.Value.CloudName, cloudinaryConfig.Value.ApiKey, _cloudinaryConfig.Value.ApiSecret);
            _cloudinary = new Cloudinary(acc);
        }

        [HttpGet("{id}", Name = "GetPhoto")]
        public async Task<IActionResult> GetPhoto(int id)
        {
            return Ok(_mapper.Map<PhotoReturnDto>(await _repo.GetPhoto(id)));
        }

        [HttpPost]
        public async Task<IActionResult> AddPhotoForUser(int userId, [FromForm]PhotoCreationDto photoDto)
        {
            if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
            {
                return Unauthorized();
            }

            var userExisting = await _repo.GetUser(userId);

            var file = photoDto.File;

            var uploadResult = new ImageUploadResult();

            if (file.Length > 0)
            {
                using (var stream = file.OpenReadStream())
                {
                    var uploadParams = new ImageUploadParams()
                    {
                        File = new FileDescription(file.Name, stream),
                        Transformation = new Transformation().Width(500).Height(500).Crop("fill").Gravity("face")
                    };
                    uploadResult = _cloudinary.Upload(uploadParams);
                }
            }
            photoDto.Url = uploadResult.Uri.ToString();
            photoDto.PublicId = uploadResult.PublicId;

            var photo = _mapper.Map<Photo>(photoDto);

            if (!userExisting.Photos.Any(u => u.IsMain))
            {
                photo.IsMain = true;
            }

            userExisting.Photos.Add(photo);
            
            if (await _repo.SaveAll())
            {
                return CreatedAtRoute(nameof(GetPhoto), new { id = photo.Id }, _mapper.Map<PhotoReturnDto>(photo));
            }
            return BadRequest("Could not add the photo");
        }

        [HttpPost("{id}/setMain")]
        public async Task<IActionResult> SetMainPhoto(int userId, int id)
        {
            if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value)) return Unauthorized();
            
            var userExisting = await _repo.GetUser(userId);
            if (!userExisting.Photos.Any(p => p.Id == id)) return Unauthorized();

            var photoExisting = await _repo.GetPhoto(id);
            if (photoExisting.IsMain) return BadRequest("This is already the main photo.");

            var currentMainPhoto = await _repo.GetMainPhoto(userId);
            currentMainPhoto.IsMain = false;

            photoExisting.IsMain = true;

            if (await _repo.SaveAll()) return NoContent();

            return BadRequest("Could not set photo to main.");
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePhoto(int userId, int id)
        {
            if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value)) return Unauthorized();
            
            var userExisting = await _repo.GetUser(userId);
            if (!userExisting.Photos.Any(p => p.Id == id)) return Unauthorized();

            var photoExisting = await _repo.GetPhoto(id);
            if (photoExisting.IsMain) return BadRequest("You cannot delete your main photo.");

            if (photoExisting.PublicId != null) 
            {
                var deletionParams = new DeletionParams(photoExisting.PublicId);
                var result = _cloudinary.Destroy(deletionParams);

                if (result.Result == "ok") {
                    _repo.Delete(photoExisting);
                } 
            }

            if (photoExisting.PublicId == null)
            {
                _repo.Delete(photoExisting);
            }

            if (await _repo.SaveAll()) return Ok();

            return BadRequest("Failed to delete the photo");
        }
    }
}