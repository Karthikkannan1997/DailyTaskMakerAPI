using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace DailyTaskMaker.Infrastructure.DataModels
{
    public class ExpenseTrackerModel
    {

        public DateTime ExpenseDate { get; set; }
        public double Price { get; set; }
        public int CategoryId {get;set;}
        public required string  Description { get; set; }

        public int ? UserExpenseTrackerId { get; set; }
    }

    public class ExpenseTrackerFilterModel
    {
        public DateTime ? DateFrom { get; set; }
        public DateTime ? DateTo { get; set; }
        public required string Duration { get; set; }

        public required bool IsCategoryWise { get; set; }
    }

    public class ExpenseTrackerResponse : ExpenseTrackerModel
    {
        public required string CategoryName { get; set; }
       
        public int TotalRows { get; set; }

        public double TotalExpenses { get; set; }
    }

    public class GroupedExpenseDataResponse {
        public required string CategoryName { get; set; }

        public int CategoryId { get; set; }

        public int TotalRows { get; set; }

        public double TotalExpenses { get; set; }

        public List<ExpenseTrackerResponse> ExpenseData { get;set; }

    }


    


}
