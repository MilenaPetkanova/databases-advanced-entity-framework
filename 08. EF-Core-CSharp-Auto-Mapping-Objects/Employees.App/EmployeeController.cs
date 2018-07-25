namespace Employees.App
{
    using System;
    using Employees.Data;
    using Employees.DtoModels;
    using AutoMapper;
    using Employees.Models;
    using Employees.App.Contracts;
    using System.Collections.Generic;
    using System.Linq;
    using AutoMapper.QueryableExtensions;
    using Microsoft.EntityFrameworkCore;

    public class EmployeeController : IEmployeeController
    {
        private readonly EmployeeContext context;
        private readonly IMapper mapper;

        public EmployeeController(EmployeeContext context, IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }

        public void AddEmployee(EmployeeDto employeeDto)
        {
            var employee = this.mapper.Map<Employee>(employeeDto);

            this.context.Employees.Add(employee);

            this.context.SaveChanges();
        }

        public EmployeeDto GetEmployeeInfo(int employeeId)
        {
            var employee = this.context.Employees.Find(employeeId);
            ValidateEmployee(employee);

            var employeeDto = this.mapper.Map<EmployeeDto>(employee);
            
            return employeeDto;
        }

        public EmployeePersonalInfoDto GetEmployeePersonalInfo(int employeeId)
        {
            var employee = this.context.Employees.Find(employeeId);
            ValidateEmployee(employee);

            var employeePersonalInfoDto = this.mapper.Map<EmployeePersonalInfoDto>(employee);
            
            return employeePersonalInfoDto;
        }

        public void SetAddress(int employeeId, string address)
        {
            var employee = this.context.Employees.Find(employeeId);
            ValidateEmployee(employee);
            
            employee.Address = address;

            this.context.SaveChanges();
        }

        public void SetBirthday(int employeeId, DateTime date)
        {
            var employee = this.context.Employees.Find(employeeId);
            ValidateEmployee(employee);

            employee.Birthday = date;

            this.context.SaveChanges();
        }

        public void SetManager(int employeeId, int managerId)
        {
            var employee = this.context.Employees.Find(employeeId);
            ValidateEmployee(employee);

            var manager = this.context.Employees.Find(managerId);
            ValidateEmployee(manager);

            employee.Manager = manager;

            this.context.SaveChanges();
        }

        public ManagerDto GetManagerInfo(int employeeId)
        {
            var manager = this.context.Employees.Find(employeeId);
            ValidateEmployee(manager);

            var managerDto = this.mapper.Map<ManagerDto>(manager);

            return managerDto;
        }

        private static void ValidateEmployee(Employee employee)
        {
            if (employee == null)
            {
                throw new ArgumentException("Invalid id!");
            }
        }

        public IList<EmployeeWithManagerDto> ListEmployeesOlderThan(int age)
        {
            var employeesOlderThan = this.context.Employees
                .Include(e => e.Manager)
                .Where(e => e.Birthday != null && this.CalculateCurrectAge(e) > age)
                .Select(e => this.mapper.Map<EmployeeWithManagerDto>(e))
                .OrderByDescending(e => e.Salary)
                //.ProjectTo<EmployeeWithManagerDto>()
                .ToList();

            return employeesOlderThan;
        }

        private int CalculateCurrectAge(Employee employee)
        {
            var dateNow = DateTime.Now;
            var birthDate = employee.Birthday.Value;

            if (birthDate == null)
            {
                return 0;
            }

            int age = dateNow.Year - birthDate.Year;

            if (dateNow.Month < birthDate.Month || (dateNow.Month == birthDate.Month && birthDate.Day < birthDate.Day))
            {
                age--;
            }

            return age;
        }
    }
}
