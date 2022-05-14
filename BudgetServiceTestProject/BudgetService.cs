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
            return getPartialAmount(start, end, budget);
        }

        var filterBudgets = FilterMonth(start, end, budgets);
        var result = 0.0;
        foreach (var budget in filterBudgets)
        {
            var budgetDate = getBudgetFirstDay(budget);
            if (budgetDate.Year == start.Year && budgetDate.Month == start.Month)
            {
                result += getPartialAmount(start, new DateTime(start.Year, start.Month, DateTime.DaysInMonth(start.Year, start.Month)), budget);
            }
            else if (budgetDate.Year == end.Year && budgetDate.Month == end.Month)
            {
                result += getPartialAmount(new DateTime(end.Year, end.Month, 1), end, budget);
            }
            else
            {
                result += budget.Amount;
            }
        }

        return result;
    }

    private static bool IsInvalidDateRange(DateTime start, DateTime end)
    {
        return start > end;
    }

    private IEnumerable<Budget> FilterMonth(DateTime start, DateTime end, List<Budget> budgets)
    {
        return budgets.Where(x =>
            TransYearMonthToDatetime(x.YearMonth) >= GetFirstDayOfMonth(start) &&
            TransYearMonthToDatetime(x.YearMonth) <= GetFirstDayOfMonth(end));
    }

    private static DateTime GetFirstDayOfMonth(DateTime start)
    {
        return new DateTime(start.Year, start.Month, 1);
    }

    private Budget? FindOneMonth(DateTime start, List<Budget> budgets)
    {
        return budgets.Find(x => x.YearMonth == DateTimeToString(start));
    }

    private static bool IsSameYearMonth(DateTime start, DateTime end)
    {
        return start.Year == end.Year && start.Month == end.Month;
    }

    private static double getPartialAmount(DateTime start, DateTime end, Budget budget)
    {
        var days = (end - start).TotalDays + 1;
        var startMonth = getBudgetFirstDay(budget);
        return budget.Amount / DateTime.DaysInMonth(startMonth.Year, startMonth.Month) * days;
    }

    private static DateTime getBudgetFirstDay(Budget budget)
    {
        return DateTime.ParseExact(budget.YearMonth+"01","yyyyMMdd",CultureInfo.CurrentCulture);
    }

    private string DateTimeToString(DateTime dateTime)
    {
        return dateTime.ToString("yyyyMM");
    }

    private DateTime TransYearMonthToDatetime(string yearMonth)
    {
        return DateTime.ParseExact(yearMonth+"01","yyyyMMdd",CultureInfo.CurrentCulture);
    }
}