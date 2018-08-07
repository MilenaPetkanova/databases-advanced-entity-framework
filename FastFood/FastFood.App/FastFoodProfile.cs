namespace FastFood.App
{
    using AutoMapper;

    using FastFood.DataProcessor.Dto.Import;
    using FastFood.Models;

    public class FastFoodProfile : Profile
	{
		// Configure your AutoMapper here if you wish to use it. If not, DO NOT DELETE THIS CLASS
		public FastFoodProfile()
		{
            CreateMap<EmployeeDTO, Employee>().ReverseMap();

            CreateMap<ItemDTO, Item>();

            CreateMap<OrderDTO, Order>();

            
        }
	}
}
