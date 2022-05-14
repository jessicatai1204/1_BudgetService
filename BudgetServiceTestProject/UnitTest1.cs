using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace BudgetServiceTestProject;

public class Tests
{
    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public void Test1()
    {
        Assert.Pass();
    }
}

public class BudgetService
{
    public double Query(DateTime start, DateTime end)
    {
        return 0;
    }
}

public interface BudgetRepo
{
    public List<Budget> GetAll();
}

public class Budget
{
    public string YearMonth { get; set; }
    public int  Amount { get; set; }
}