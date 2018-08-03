namespace ProductShop.App
{
    using AutoMapper;

    using ProductShop.App.Dtos.Import;
    using ProductShop.Models;

    public class ProductShopProfile : Profile
    {
        public ProductShopProfile()
        {
            CreateMap<UserImportDto, User>().ReverseMap();

            CreateMap<ProductImportDto, Product>().ReverseMap();

            CreateMap<CategoryImportDto, Category>().ReverseMap();
        }
    }
}
