using System.Linq;
using System.Collections.Generic;
using PaginationExam.Models;
using System;

namespace PaginationExam.Dao
{
    public class EmployeeDao
    {
        private List<Employee> _dummyEmployeeData = new List<Employee>();

        public EmployeeDao()
        {
            for (int i = 0; i < 100; i++)
            {
                this._dummyEmployeeData.Add(
                    new Employee()
                    {
                        ID = i,
                        FirstName = "しゃいん",
                        LastName = i.ToString() + "号"
                    });
            }
        }

        public (int totalItemCount, int lastPage, List<Employee>) GetEmployees(int page, int countPerPage)
        {
            int totalItemCount = 0;
            int lastPage = 0;
            List<Employee> employees = null;

            employees = this._dummyEmployeeData.Skip(countPerPage * (page - 1)).Take(countPerPage).ToList();
            totalItemCount = this._dummyEmployeeData.Count();

            lastPage = (int)Math.Floor((decimal)totalItemCount / countPerPage);
            if (totalItemCount % countPerPage > 0)
                lastPage++;

            return (totalItemCount, lastPage, employees);
        }
    }
}
