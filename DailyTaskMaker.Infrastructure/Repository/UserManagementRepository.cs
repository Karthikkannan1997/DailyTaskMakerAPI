using DailyTaskMaker.Infrastructure.DataModels;
using DailyTaskMaker.Infrastructure.Interfaces;
using DailyTaskMaker.Infrastructure.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DailyTaskMaker.Infrastructure.Repository
{
    public class UserManagementRepository : IUserManagementRepository
    {
        private readonly DailyTaskMakerDbContext _dbContext;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserManagementRepository(DailyTaskMakerDbContext dbContext, IHttpContextAccessor httpContextAccessor) 
        {
            _dbContext = dbContext;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<List<UserManagementListModel>> GetUserList(int skip,int take)
        {
            try
            {
                   var data =   await   _dbContext.Set<UserDetail>().Skip(skip).Take(take).Select(
                    x => new UserManagementListModel ()
                    {
                        UserId = x.UserId,
                        FirstName = x.FirstName,
                        SurName = x.SurName,
                        EmailId = x.EmailId,
                        MobileNumber = x.MobileNumber,
                        UserRoleList = x.UserRoleMappings.Where(y => y.UserId == x.UserId).
                        Select( z=>new UserRoleList() {

                            UserRoleId=z.UserRoleId, 
                            RoleName=   _dbContext.Set<UserRole>().Single(a => a.UserRoleId == z.UserRoleId).RoleName

                        }
                        ).ToList()

                    }
                    ).ToListAsync();

                return data;
               
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
