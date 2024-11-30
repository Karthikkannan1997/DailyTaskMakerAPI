using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DailyTaskMaker.Infrastructure.DataModels
{
    public class UserManagementListModel
    {


        public int UserId { get; set; }
        public string? FirstName { get; set; }

        public string? SurName { get; set; }


        public string? EmailId { get; set; }


        public long? MobileNumber { get; set; }

        public bool? IsActive { get; set; }
        public required List<UserRoleList> UserRoleList { get; set; }

    }

    public class UserRoleList
    {
        public int UserRoleId { get; set; }

        public required string RoleName { get; set; }
    }

    public class UserDetailModel
    {
        public string? EmailId { get; set;}
        public long  MobileNumber { get; set; }

        public string? RoleIds { get; set; }

        public int UserId { get; set; }

        public bool? IsActive { get; set; }
    }
}


