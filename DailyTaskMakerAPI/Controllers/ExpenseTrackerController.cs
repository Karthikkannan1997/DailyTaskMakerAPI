using DailyTaskMaker.Infrastructure.DataModels;
using DailyTaskMaker.Infrastructure.Interfaces;
using DailyTaskMakerAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Graph;
using Microsoft.Identity.Web.Resource;
using System.Collections.Generic;

namespace DailyTaskMakerAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    [RequiredScope(RequiredScopesConfigurationKey = "AzureAd:Scopes:api.read")]
    public class ExpenseTrackerController : ControllerBase
    {
        private readonly IExpenseTrackerRepository _expenseTrackerRepo;
        public ExpenseTrackerController(IExpenseTrackerRepository expenseTrackerRepo)
        {
            _expenseTrackerRepo = expenseTrackerRepo;
        }
        [HttpGet("GetCategories")]
        public async Task<List<CategoryModel>> GetCategories()
        {
            try
            {
                List<CategoryModel> CategoryDetails = await _expenseTrackerRepo.GetCategory();
                return CategoryDetails;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        [HttpPost("SaveExpenses")]
        public async Task<bool> SaveExpenses(List<ExpenseTrackerModel> expenseTrackerData)
        {
            try
            {
                bool  isSaved = await _expenseTrackerRepo.SaveExpenses(expenseTrackerData);
                return isSaved;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        [HttpGet("GetExpenses")]
        public async Task<List<ExpenseTrackerResponse>> GetExpenses(string Duration,bool IsCategoryWise, int Skip, int Take, DateOnly? DateFrom = null, DateOnly? DateTo = null)
        {
            try
            {
                
                List <ExpenseTrackerResponse> expenseTrackerData = await _expenseTrackerRepo.GetExpenses(  Duration,  IsCategoryWise, Skip,Take,DateFrom, DateTo);
                //if (expenseTrackerData != null)
                //{
                //    Response.Headers.Append("X-Total-Count", expenseTrackerData.Count > 0 ? expenseTrackerData[0].TotalRows.ToString() : "0");
                //}
                //Response.Headers.Append("Access-Control-Expose-Headers", "X-Total-Count");
                return expenseTrackerData;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        [HttpGet("GetExpensesByCategory")]
        public async Task<dynamic> GetExpensesByCategory(string Duration, bool IsCategoryWise, int Skip, int Take, DateOnly? DateFrom = null, DateOnly? DateTo = null)
        {
            try
            {
                var expenseTrackerData = await _expenseTrackerRepo.GetExpensesByCategory(Duration, IsCategoryWise, Skip, Take, DateFrom, DateTo);
                //if (expenseTrackerData != null)
                //{
                //    Response.Headers.Add("X-Total-Count", expenseTrackerData.Count > 0 ? expenseTrackerData[0].TotalRows.ToString() : "0");
                //}
                //Response.Headers.Append("Access-Control-Expose-Headers", "X-Total-Count");
                return expenseTrackerData;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        [HttpDelete("DeleteExpenseByUserExpenseTrackerId")]
        public async Task<bool> DeleteExpenseByUserExpenseTrackerId(int UserExpenseTrackerId)
        {
            try
            {
                var isDeleted = await _expenseTrackerRepo.DeleteExpenseByUserExpenseTrackerId(UserExpenseTrackerId);
                
                return isDeleted;
            }
            catch (Exception ex)
            {
                throw;
            }
        }


    }
}
