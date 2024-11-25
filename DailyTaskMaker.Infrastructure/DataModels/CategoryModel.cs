using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DailyTaskMaker.Infrastructure.DataModels
{
    public class CategoryModel
    {
        public int CategoryId { get; set; }
        public required string  CategoryName { get; set; }
    }
}
