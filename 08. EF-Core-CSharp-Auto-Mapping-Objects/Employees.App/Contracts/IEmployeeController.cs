namespace Employees.App.Contracts
{
    using System;
    using Employees.DtoModels;
    using System.Collections.Generic;

    public interface IEmployeeController
    {
        void AddEmployee(EmployeeDto employeeDto);
        void SetBirthday(int employeeId, DateTime date);
        void SetAddress(int employeeId, string address);
        EmployeeDto GetEmployeeInfo(int employeeId);
        EmployeePersonalInfoDto GetEmployeePersonalInfo(int employeeId);
        void SetManager(int employeeId, int managerId);
        ManagerDto GetManagerInfo(int employeeId);
        IList<EmployeeWithManagerDto> ListEmployeesOlderThan(int age);
    }
}
