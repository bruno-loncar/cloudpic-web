using CloudPic.BAL.Interfaces;
using CloudPic.DAL.Interfaces;
using CloudPic.Models.DTO;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CloudPic.DAL.Concretes
{
    public class UserService : IUserService
    {
        private readonly ILogger<UserService> _logger;
        private readonly IConfiguration _configuration;
        private readonly IUserRepo _userRepo;
        private readonly IPhotoRepo _photoRepo;

        public UserService(ILogger<UserService> logger,
            IConfiguration configuration,
            IUserRepo userRepo,
            IPhotoRepo photoRepo)
        {
            this._logger = logger;
            this._configuration = configuration;
            this._userRepo = userRepo;
            this._photoRepo = photoRepo;
        }

        public async Task<int> DeleteUser(int id)
        {
            var photos = await _photoRepo.GetPhotosForUserAsync(id);

            foreach (var photo in photos)
            {
                await _photoRepo.DeleteHashtagsForPhotoAsync(photo.Id);
                await _photoRepo.DeletePhotoAsync(photo.Id);
            }

            await _userRepo.DeleteUserPlansForUser(id);

            return await _userRepo.DeleteUserAsync(id);
        }
    }
}
