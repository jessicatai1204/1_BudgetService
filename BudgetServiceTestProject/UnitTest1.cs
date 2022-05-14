using System;
using System.Collections.Generic;
using Moq;
using NUnit.Framework;
using NUnit.Framework.Constraints;

namespace BudgetServiceTestProject;

public class Tests
{
    private Mock<IBudgetRepo> _mockRepo;
    private BudgetService _service;
    public Tests()
    {
        _mockRepo = new Mock<IBudgetRepo>();
    }

    [SetUp]
    public void SetUp()
    {
        _service = new BudgetService(_mockRepo.Object);
    }

    [Test]
    public void Test_Invalid_Date_Range()
    {
        var result = _service.Query(new DateTime(2022, 5, 1), new DateTime(2022, 4, 1));
        ShouldBe(0, result);
    }
    
    [Test]
    public void Test_No_Budget()
    {
        _mockRepo.Setup(x => x.GetAll()).Returns(new List<Budget>
        {
            new()
            {
                YearMonth = "202206", 
                Amount = 1000
            }
        });

        var result = _service.Query(new DateTime(2022, 9, 1), new DateTime(2022, 9, 30));
        ShouldBe(0, result);
    }
    
    [Test]
    public void Test_Has_Budget()
    {
        _mockRepo.Setup(x => x.GetAll()).Returns(new List<Budget>
        {
            new()
            {
                YearMonth = "202206", 
                Amount = 300
            }
        });

        var result = _service.Query(new DateTime(2022, 6, 1), new DateTime(2022, 6, 30));
        ShouldBe(300, result);
    }
    
    [Test]
    public void Test_Has_Budget_Partial()
    {
        _mockRepo.Setup(x => x.GetAll()).Returns(new List<Budget>
        {
            new()
            {
                YearMonth = "202205", 
                Amount = 1000
            },
            new()
            {
                YearMonth = "202206", 
                Amount = 300
            },
            new()
            {
                YearMonth = "202207",
                Amount = 620
            },
            new()
            {
                YearMonth = "202208", 
                Amount = 3100
            },
            new()
            {
                YearMonth = "202209", 
                Amount = 3000
            },
        });

        var result = _service.Query(new DateTime(2022, 6, 1), new DateTime(2022, 8, 20));
        ShouldBe(2920, result);
    }
    
    [Test]
    public void Test_Has_Budget_Partial_CrossYear()
    {
        _mockRepo.Setup(x => x.GetAll()).Returns(new List<Budget>
        {
            new()
            {
                YearMonth = "202211", 
                Amount = 1000
            },
            new()
            {
                YearMonth = "202212", 
                Amount = 310
            },
            new()
            {
                YearMonth = "202301",
                Amount = 3100
            },
            new()
            {
                YearMonth = "202302", 
                Amount = 1000000
            },
        });

        var result = _service.Query(new DateTime(2022, 12, 10), new DateTime(2023, 1, 3));
        ShouldBe(520, result);
    }
    
    private void ShouldBe(double expected, double actual)
    {
        Assert.AreEqual(expected, actual);
    }
}