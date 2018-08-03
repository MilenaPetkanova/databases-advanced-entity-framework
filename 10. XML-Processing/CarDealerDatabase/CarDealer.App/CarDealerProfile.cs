namespace CarDealer.App
{
    using AutoMapper;

    using CarDealer.Models;
    using CarDealer.App.DTOs.Import;

    public class CarDealerProfile : Profile
    {
        public CarDealerProfile()
        {
            CreateMap<CarImportDTO, Car>();

            CreateMap<CustomerImportDTO, Customer>();

            CreateMap<PartImportDTO, Part>();

            CreateMap<SupplierImportDTO, Supplier>();
        }
    }
}
