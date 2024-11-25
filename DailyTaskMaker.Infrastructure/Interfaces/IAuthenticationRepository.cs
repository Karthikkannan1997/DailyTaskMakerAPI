using DailyTaskMaker.Infrastructure.Models;
using DailyTaskMakerAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DailyTaskMaker.Infrastructure.Interfaces
{
    public interface IAuthenticationRepository
    {
         Task<UserDetailsModel> SaveUserDetails(UserDetailsModel userDetail);
        Task<List<string>> FetchUserRolesFromDatabase(string userEmail);
    }
}
