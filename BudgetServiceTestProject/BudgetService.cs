using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace BudgetServiceTestProject;

public class BudgetService
{
    private IBudgetRepo _repo;
    public BudgetService(IBudgetRepo repo)
    {
        _repo = repo;
    }

    public double Query(DateTime start, DateTime end)
    {
        var defaultAmount = 0;
        if (IsInvalidDateRange(start, end))
        {
            return defaultAmount;
        }

        var budgets = _repo.GetAll();
        
        if (IsSameYearMonth(start, end))
        {
            var budget = FindOneMonth(start, budgets);
            if (budget == null)
            {
                return defaultAmount;
            }
            return getPartialAmountOfMonth(start, end, budget);
        }

        var filterBudgets = FindMultiMonth(start, end, budgets);
        var result = 0.0;
        foreach (var budget in filterBudgets)
        {
            var budgetDate = ParseToDatetimeAtFirstDayOfMonth(budget.YearMonth);
            if (IsSameYearMonth(budgetDate,start))
            {
                result += getPartialAmountOfMonth(start, new DateTime(start.Year, start.Month, DateTime.DaysInMonth(start.Year, start.Month)), budget);
            }
            else if (budgetDate.Year == end.Year && budgetDate.Month == end.Month)
            {
                result += getPartialAmountOfMonth(new DateTime(end.Year, end.Month, 1), end, budget);
            }
            else
            {
                result += budget.Amount;
            }
        }

        return result;
    }

    private bool IsInvalidDateRange(DateTime start, DateTime end)
    {
        return start > end;
    }

    private IEnumerable<Budget> FindMultiMonth(DateTime start, DateTime end, List<Budget> budgets)
    {
        return budgets.Where(x =>
            ParseToDatetimeAtFirstDayOfMonth(x.YearMonth) >= GetFirstDayOfMonth(start) &&
            ParseToDatetimeAtFirstDayOfMonth(x.YearMonth) <= GetFirstDayOfMonth(end));
    }

    private DateTime GetFirstDayOfMonth(DateTime start)
    {
        return new DateTime(start.Year, start.Month, 1);
    }

    private Budget? FindOneMonth(DateTime start, List<Budget> budgets)
    {
        return budgets.Find(x => x.YearMonth == DateTimeToString(start));
    }

    private  bool IsSameYearMonth(DateTime start, DateTime end)
    {
        return start.Year == end.Year && start.Month == end.Month;
    }

    private  double getPartialAmountOfMonth(DateTime start, DateTime end, Budget budget)
    {
        var days = (end - start).TotalDays + 1;
        var startMonth = ParseToDatetimeAtFirstDayOfMonth(budget.YearMonth);
        return budget.Amount / DateTime.DaysInMonth(startMonth.Year, startMonth.Month) * days;
    }
    

    private string DateTimeToString(DateTime dateTime)
    {
        return dateTime.ToString("yyyyMM");
    }

    private DateTime ParseToDatetimeAtFirstDayOfMonth(string yearMonth)
    {
        return DateTime.ParseExact(yearMonth+"01","yyyyMMdd",CultureInfo.CurrentCulture);
    }
}