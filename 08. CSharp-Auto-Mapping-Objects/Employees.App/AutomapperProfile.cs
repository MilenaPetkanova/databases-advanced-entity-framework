namespace Employees.App
{
    using AutoMapper;
    using Employees.DtoModels;
    using Employees.Models;

    public class AutomapperProfile : Profile
    {
        public AutomapperProfile()
        {
            CreateMap<Employee, EmployeeDto>().ReverseMap();
            CreateMap<Employee, EmployeePersonalInfoDto>().ReverseMap();
            CreateMap<Employee, ManagerDto>().ReverseMap();
            CreateMap<Employee, EmployeeWithManagerDto>().ReverseMap();
        }
    }
}
