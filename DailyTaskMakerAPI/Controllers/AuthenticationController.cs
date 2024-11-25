using DailyTaskMaker.Infrastructure.Interfaces;
using DailyTaskMaker.Infrastructure.Models;
using DailyTaskMakerAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Web.Resource;

namespace DailyTaskMakerAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    [RequiredScope(RequiredScopesConfigurationKey = "AzureAd:Scopes:api.read")]
    public class AuthenticationController : ControllerBase

    {
        private readonly IAuthenticationRepository _authRepository;
        public AuthenticationController(IAuthenticationRepository authRepository)
        {
            _authRepository = authRepository;
        }
        [HttpPost("SaveUserDetails")]
        public async Task<UserDetailsModel> SaveUserDetails(UserDetailsModel userDetails)
        {
            try
            {
                UserDetailsModel userInformation = await _authRepository.SaveUserDetails(userDetails);
                return userInformation;
            }
            catch(Exception ex)
            {
                throw;
            }
        }
    }
}
