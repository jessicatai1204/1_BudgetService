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
        if (start > end)
        {
            return 0;
        }

        var budgets = _repo.GetAll();
        
        if (IsSameMonth(start, end))
        {
            var budget = FindMonth(start, budgets);
            if (budget == null)
            {
                return 0;
            }
            return getPartialAmount(start, end, budget);
        }

        var filterBudgets = FilterMonth(start, end, budgets);
        var result = 0.0;
        foreach (var budget in filterBudgets)
        {
            Console.WriteLine(budget.YearMonth);
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

    private IEnumerable<Budget> FilterMonth(DateTime start, DateTime end, List<Budget> budgets)
    {
        var filterBudgets = budgets.Where(x =>
            TransYearMonthToDatetime(x.YearMonth) >= new DateTime(start.Year, start.Month, 1) &&
            TransYearMonthToDatetime(x.YearMonth) <= new DateTime(end.Year, end.Month, 1));
        return filterBudgets;
    }

    private Budget? FindMonth(DateTime start, List<Budget> budgets)
    {
        var budget = budgets.Find(x => x.YearMonth == DateTimeToString(start));
        return budget;
    }

    private static bool IsSameMonth(DateTime start, DateTime end)
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