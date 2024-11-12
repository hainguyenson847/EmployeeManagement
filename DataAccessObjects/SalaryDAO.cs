﻿using BusinessObjects;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace DataAccessObjects
{
    public static class SalaryDAO
    {
        public static List<Salary> GetSalaries()
        {
            using (var context = new FuhrmContext())
            {
                return context.Salaries.Include(s => s.Employee).ToList();
            }
        }

        public static void AddSalary(Salary salary)
        {
            using (var context = new FuhrmContext())
            {
                context.Salaries.Add(salary);
                context.SaveChanges();
            }
        }

        public static List<Salary> SearchSalary(string employeeName)
        {
            using (var context = new FuhrmContext())
            {
                return context.Salaries
                    .Include(s => s.Employee)
                    .Where(s => s.Employee.FullName.Contains(employeeName))
                    .ToList();
            }
        }

        public static void RemoveSalary(Salary salary)
        {
            using (var context = new FuhrmContext())
            {
                context.Salaries.Remove(salary);
                context.SaveChanges();
            }
        }

        public static void UpdateSalary(Salary salary)
        {
            using (var context = new FuhrmContext())
            {
                context.Salaries.Update(salary);
                context.SaveChanges();
            }
        }

        public static Salary GetSalaryByEmployeeId(int employeeId)
        {
            using (var context = new FuhrmContext())
            {
                return context.Salaries
                    .Include(s => s.Employee)
                    .FirstOrDefault(s => s.EmployeeId == employeeId);
            }
        }
    }
}
