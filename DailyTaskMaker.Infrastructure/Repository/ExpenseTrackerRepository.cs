using Azure;
using DailyTaskMaker.Infrastructure.CommonUtilityClasses;
using DailyTaskMaker.Infrastructure.DataModels;
using DailyTaskMaker.Infrastructure.Interfaces;
using DailyTaskMaker.Infrastructure.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace DailyTaskMaker.Infrastructure.Repository
{
    public class ExpenseTrackerRepository : IExpenseTrackerRepository
    {

        private readonly DailyTaskMakerDbContext _dbContext;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public ExpenseTrackerRepository(DailyTaskMakerDbContext dbContext, IHttpContextAccessor httpContextAccessor)
        {
            _dbContext = dbContext;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<List<CategoryModel>> GetCategory()
        {
            try
            {
                return await _dbContext.Set<ExpenseTrackerCategory>().Where(x => x.IsActive == true).Select(
                 categoryData => new CategoryModel()
                 {
                     CategoryId = categoryData.CategoryId,
                     CategoryName = categoryData.CategoryName
                 }
                 ).ToListAsync();
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<bool> SaveExpenses(List<ExpenseTrackerModel> expenseTrackerData)
        {
            try
            {

                string convertToXml = UserUtility.ConvertToXml(expenseTrackerData);
                var userEmail = UserUtility.GetUserEmail(_httpContextAccessor);
                var resultParameter = new SqlParameter
                {
                    ParameterName = "@Result",
                    SqlDbType = System.Data.SqlDbType.Bit,
                    Direction = System.Data.ParameterDirection.Output
                };

                var parameters = new[]
            {
                    new SqlParameter("@ExpensesXmlData", convertToXml),
                    new SqlParameter("@UserEmail", userEmail),
                    new SqlParameter("@isEdited",  (expenseTrackerData.Count == 1 && expenseTrackerData[0]?.UserExpenseTrackerId != null && expenseTrackerData[0]?.UserExpenseTrackerId != 0) ? true : false),
                    resultParameter
                };

                await _dbContext.Database.ExecuteSqlRawAsync("EXEC Sp_SaveExpenseData @ExpensesXmlData,@UserEmail,@isEdited,@Result OUTPUT", parameters);

                return (bool)resultParameter.Value;

            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<List<ExpenseTrackerResponse>> GetExpenses(string Duration, bool IsCategoryWise, int Skip, int Take, DateOnly? DateFrom = null, DateOnly? DateTo = null)
        {
            try
            {
                return await GetExpensesData(Duration, Skip, Take, IsCategoryWise, DateFrom, DateTo);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<dynamic> GetExpensesByCategory(string Duration, bool IsCategoryWise, int Skip, int Take, DateOnly? DateFrom = null, DateOnly? DateTo = null)
        {
            try
            {
                List<ExpenseTrackerResponse> expensesData = await GetExpensesData(Duration, Skip, Take, IsCategoryWise, DateFrom, DateTo);


                var GroupedExpenseData = expensesData.GroupBy(expense => new { expense.CategoryName, expense.CategoryId, expense.TotalRows, expense.TotalExpenses })
                    .Select(data =>
                  new GroupedExpenseDataResponse
                  {
                      CategoryId = data.Key.CategoryId,
                      CategoryName = data.Key.CategoryName,
                      TotalRows = data.Key.TotalRows,
                      TotalExpenses = data.Key.TotalExpenses,
                      ExpenseData = data.ToList()
                  });

                return GroupedExpenseData;
            

            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<bool> DeleteExpenseByUserExpenseTrackerId(int UserExpenseTrackerId)
        {
            try
            {
                var expenseData = new UserExpenseTracker { UserExpenseTrackerId = UserExpenseTrackerId };
                _dbContext.Entry(expenseData).State = EntityState.Deleted;
                await _dbContext.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
           
        }

        private async Task<List<ExpenseTrackerResponse>> GetExpensesData( string Duration, int Skip, int Take, bool IsCategoryWise, DateOnly? DateFrom = null, DateOnly? DateTo = null)
        {
            try
            {
              
                DateOnly TodayDate =  DateOnly.FromDateTime(DateTime.Now);
                DateOnly firstDayOfCurrentMonth = new DateOnly(TodayDate.Year, TodayDate.Month, 1);
                DateOnly lastDayOfCurrentMonth = firstDayOfCurrentMonth.AddMonths(1).AddDays(-1);
                DateOnly firstDayOfLastMonth = firstDayOfCurrentMonth.AddMonths(-1);
                DateOnly LastDayOfLastMonth = firstDayOfCurrentMonth.AddDays(-1);

                switch (Duration.ToLower())
                {
                    case "last week":
                       
                        DateFrom = TodayDate.AddDays(-7);
                        DateTo = TodayDate;
                        break;

                    case "current month":
                       
                        DateFrom = firstDayOfCurrentMonth;
                        DateTo = lastDayOfCurrentMonth;
                        break;
                    case "last month":

                        DateFrom = firstDayOfLastMonth;
                        DateTo = LastDayOfLastMonth;
                        break;
                    case "last 6 months":
                        DateFrom = firstDayOfCurrentMonth.AddMonths(-6);
                        DateTo = firstDayOfCurrentMonth;
                        break;
                    case "last year":
                        // Get dates for last year
                        DateFrom = new DateOnly(TodayDate.Year - 1, 1, 1);
                        DateTo = new DateOnly(TodayDate.Year -1 ,12,31);
                        break;
                    case "current year":
                        // Get dates for current year
                        DateFrom = new DateOnly(TodayDate.Year, 1, 1);
                        DateTo = new DateOnly(TodayDate.Year, 12, 31);
                        break;
                     


                }
                var userEmail =  UserUtility.GetUserEmail(_httpContextAccessor);

                var parameters = new[]
            {
                 new SqlParameter("@dateFrom", DateFrom!=null ? DateFrom:DBNull.Value ),
                 new SqlParameter("@dateTo", DateTo !=null ? DateTo :  DBNull.Value),
                 new SqlParameter("@duration", Duration),
                 new SqlParameter("@UserEmail", userEmail),
                 new SqlParameter("@Skip",Skip),
                 new SqlParameter("@Take",Take),
                 new SqlParameter("@IsCategoryWise",IsCategoryWise)


            };
                var query = "EXEC sp_GetExpensesData @dateFrom, @dateTo, @duration, @UserEmail, @Skip,@Take,@IsCategoryWise";

             return await _dbContext.Database.SqlQueryRaw<ExpenseTrackerResponse>(query, parameters).ToListAsync();
           

            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
