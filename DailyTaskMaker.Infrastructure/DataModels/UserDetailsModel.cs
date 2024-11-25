using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace DailyTaskMakerAPI.Models
{
    public class UserDetailsModel
    {
        
        public  string ? FirstName { get; set; }
        
        public string ? SurName { get; set; }

        
        public  string? EmailId { get; set; }

        
        public string ? MobileNumber { get; set; }

        
        public  string ?  DisplayName { get; set; }

        public List<string>? UserRoleName { get; set; }

        
    }
}
