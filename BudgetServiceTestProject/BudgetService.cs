using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace BudgetServiceTestProject;

public class BudgetService
{
    private readonly IBudgetRepo _repo;
    public BudgetService(IBudgetRepo repo)
    {
        _repo = repo;
    }

    public double Query(DateTime start, DateTime end)
    {
        const int defaultAmount = 0;
        if (IsInvalidDateRange(start, end))
        {
            return defaultAmount;
        }

        var budgets = _repo.GetAll();
        
        if (IsSameYearMonth(start, end))
        {
            var budget = FindOneMonthBudge(start, budgets);
            if (budget == null)
            {
                return defaultAmount;
            }
            return GetPartialAmountOfMonth(start, end, budget);
        }

        var filterBudgets = FindMultiMonthBudges(start, end, budgets);
        var result = 0.0;
        foreach (var budget in filterBudgets)
        {
            var budgetDate = ParseYearMonthToDatetimeAtFirstDay(budget.YearMonth);
            if (IsSameYearMonth(budgetDate,start))
            {
                result += GetPartialAmountOfMonth(start, EndDayOfYearMonth(start), budget);
            }
            else if (IsSameYearMonth(budgetDate,end))
            {
                result += GetPartialAmountOfMonth(FirstDayOfYearMonth(end), end, budget);
            }
            else
            {
                result += budget.Amount;
            }
        }

        return result;
    }

    private static DateTime EndDayOfYearMonth(DateTime start)
    {
        return new DateTime(start.Year, start.Month, DateTime.DaysInMonth(start.Year, start.Month));
    }

    private static DateTime FirstDayOfYearMonth(DateTime end)
    {
        return new DateTime(end.Year, end.Month, 1);
    }

    private static bool IsInvalidDateRange(DateTime start, DateTime end)
    {
        return start > end;
    }

    private static IEnumerable<Budget> FindMultiMonthBudges(DateTime start, DateTime end, List<Budget> budgets)
    {
        return budgets.Where(x =>
            ParseYearMonthToDatetimeAtFirstDay(x.YearMonth) >= GetFirstDayOfMonth(start) &&
            ParseYearMonthToDatetimeAtFirstDay(x.YearMonth) <= GetFirstDayOfMonth(end));
    }

    private static DateTime GetFirstDayOfMonth(DateTime start)
    {
        return new DateTime(start.Year, start.Month, 1);
    }

    private static Budget? FindOneMonthBudge(DateTime start, List<Budget> budgets)
    {
        return budgets.Find(x => x.YearMonth == ParseDateTimeToYearMonth(start));
    }

    private static bool IsSameYearMonth(DateTime start, DateTime end)
    {
        return start.Year == end.Year && start.Month == end.Month;
    }

    private static double GetPartialAmountOfMonth(DateTime start, DateTime end, Budget budget)
    {
        var days = (end - start).TotalDays + 1;
        var startMonth = ParseYearMonthToDatetimeAtFirstDay(budget.YearMonth);
        return budget.Amount / DateTime.DaysInMonth(startMonth.Year, startMonth.Month) * days;
    }
    

    private static string ParseDateTimeToYearMonth(DateTime dateTime)
    {
        return dateTime.ToString("yyyyMM");
    }

    private static DateTime ParseYearMonthToDatetimeAtFirstDay(string yearMonth)
    {
        return DateTime.ParseExact(yearMonth+"01","yyyyMMdd",CultureInfo.CurrentCulture);
    }
}