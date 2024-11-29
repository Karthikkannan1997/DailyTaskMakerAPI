using DailyTaskMaker.Infrastructure.DataModels;
using DailyTaskMaker.Infrastructure.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Graph;
using Microsoft.Identity.Web.Resource;

namespace DailyTaskMakerAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Policy = "AdminPolicy")]
    [RequiredScope(RequiredScopesConfigurationKey = "AzureAd:Scopes:api.read")]
    public class UserManagementController : ControllerBase

        
    {
        private readonly IUserManagementRepository _userRepo;
        public UserManagementController(IUserManagementRepository userRepo)
        {
            _userRepo = userRepo;
        }
        [HttpGet("GetUserList")]   
        public async Task<List<UserManagementListModel>> GetUserList(int skip,int take)
        {
            try
            {
                return await _userRepo.GetUserList(skip,take);

            }
            catch(Exception ex)
            {
                throw;
            }
        }

        [HttpGet("GetRoles")]
        public async Task<dynamic> GetRoles()
        {
            try
            {
                return await _userRepo.GetRoles();

            }
            catch (Exception ex)
            {
                throw;
            }
        }


    }
}
