using DailyTaskMaker.Infrastructure.CommonUtilityClasses;
using DailyTaskMaker.Infrastructure.Interfaces;
using DailyTaskMaker.Infrastructure.Models;
using DailyTaskMakerAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace DailyTaskMaker.Infrastructure.Repository
{
    public class AuthenticationRepository : IAuthenticationRepository
    {
        private readonly DailyTaskMakerDbContext _dbContext;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public AuthenticationRepository(DailyTaskMakerDbContext dbContext, IHttpContextAccessor httpContextAccessor)
        {
            _dbContext = dbContext;
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task<UserDetailsModel> SaveUserDetails(UserDetailsModel userDetail)
        {
            try
            {
                string email = UserUtility.GetUserEmail(_httpContextAccessor) ?? "";
                var responseData = new UserDetailsModel();
                //if (_httpContextAccessor.HttpContext.User.Identity.IsAuthenticated)
                //{
                if (!string.IsNullOrEmpty(email) && email.Equals(userDetail.EmailId, StringComparison.CurrentCultureIgnoreCase))
                {
                    UserDetail? fetchedRecord = await _dbContext.UserDetails.Where(x => x.EmailId == userDetail.EmailId).FirstOrDefaultAsync();
                    if (fetchedRecord == null)
                    {
                        var UserDetailData = new UserDetail
                        {

                            EmailId = userDetail.EmailId,
                            FirstName = userDetail.FirstName,
                            SurName = userDetail.SurName,
                            DisplayName = userDetail.DisplayName,
                            MobileNumber = userDetail.MobileNumber,
                            IsActive = true
                        };


                        await _dbContext.AddAsync(UserDetailData);
                        await _dbContext.SaveChangesAsync();

                        int userId = UserDetailData.UserId;
                        var userRoleDetails = await _dbContext.Set<UserRole>().FirstOrDefaultAsync(x => x.RoleName.ToLower() == "free user");
                        int userRoleId = userRoleDetails?.UserRoleId ?? 2;
                        var roleDetailsData = new UserRoleMapping
                        {
                            UserId = userId,
                            UserRoleId = userRoleId,
                            CreatedBy = UserDetailData.EmailId
                        };

                        await _dbContext.AddAsync(roleDetailsData);
                        await _dbContext.SaveChangesAsync();

                        responseData.EmailId = userDetail.EmailId;
                        responseData.FirstName = userDetail.FirstName;
                        responseData.SurName = userDetail.SurName;
                        responseData.DisplayName = userDetail.DisplayName;
                        responseData.MobileNumber = userDetail.MobileNumber;
                        responseData.UserRoleName = new List<string> { "Free User" };

                    }
                    else
                    {

                        responseData.EmailId = fetchedRecord.EmailId;
                        responseData.FirstName = fetchedRecord.FirstName;
                        responseData.SurName = fetchedRecord.SurName;
                        responseData.DisplayName = fetchedRecord.DisplayName;
                        responseData.MobileNumber = fetchedRecord.MobileNumber;
                        responseData.UserRoleName = await FetchUserRolesFromDatabase(fetchedRecord.EmailId);




                    }
                    //    var claims = responseData.UserRoleName.Select(role => new Claim(ClaimTypes.Role, role));
                    //    var appIdentity = new ClaimsIdentity(claims);
                    //    _httpContextAccessor.HttpContext.User.AddIdentity(appIdentity);
                    //}
                    
                }
                return responseData;
            }
            catch (Exception ex)
            {
                throw;
            }


        }

       public async Task<List<string>> FetchUserRolesFromDatabase(string userEmail)
        {
            try
            {
                List<string> dataTobeReturned = new List<string>();
                UserDetail? fetchedRecord = await _dbContext.UserDetails.Where(x => x.EmailId == userEmail).FirstOrDefaultAsync();
                if (fetchedRecord != null)
                {
                    List<UserRoleMapping> roleMapping = await _dbContext.UserRoleMappings.Where(x => x.UserId == fetchedRecord.UserId).ToListAsync();
                    foreach (var item in roleMapping)
                    {
                        var roleDetails = await _dbContext.UserRoles.FirstOrDefaultAsync(x => x.UserRoleId == item.UserRoleId);
                        if (roleDetails != null)
                        {
                            dataTobeReturned.Add(roleDetails.RoleName);
                        }
                        else
                        {
                            return dataTobeReturned;
                        }
                    }
                }

                else
                {
                    return dataTobeReturned;
                }

                return dataTobeReturned;

            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
