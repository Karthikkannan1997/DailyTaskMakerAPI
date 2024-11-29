using DailyTaskMaker.Infrastructure.DataModels;
using DailyTaskMakerAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DailyTaskMaker.Infrastructure.Interfaces
{
    public interface IUserManagementRepository
    {
         Task<List<UserManagementListModel>> GetUserList(int skip,int take);

        Task<dynamic> GetRoles();

        Task<bool> SaveUserData(UserDetailModel userData);
    }
}
