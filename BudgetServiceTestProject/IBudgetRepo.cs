using System.Collections.Generic;

namespace BudgetServiceTestProject;

public interface IBudgetRepo
{
    public List<Budget> GetAll();
}