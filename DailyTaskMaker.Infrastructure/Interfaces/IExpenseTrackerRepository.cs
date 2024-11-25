using DailyTaskMaker.Infrastructure.DataModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DailyTaskMaker.Infrastructure.Interfaces
{
    public interface IExpenseTrackerRepository
    {
        Task<List<CategoryModel>> GetCategory();
        Task<bool> SaveExpenses(List<ExpenseTrackerModel> expenseTrackerData);
        Task<List<ExpenseTrackerResponse>> GetExpenses( string Duration, bool IsCategoryWise, int Skip, int Take, DateOnly? DateFrom, DateOnly? DateTo);
        Task<dynamic> GetExpensesByCategory(string Duration, bool IsCategoryWise, int Skip, int Take, DateOnly? DateFrom = null, DateOnly? DateTo = null);
        Task<bool> DeleteExpenseByUserExpenseTrackerId(int UserExpenseTrackerId);

    }
}
