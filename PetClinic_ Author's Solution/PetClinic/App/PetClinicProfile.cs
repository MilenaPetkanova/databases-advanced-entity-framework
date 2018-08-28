namespace PetClinic.App
{
    using System;
    using System.Globalization;

    using AutoMapper;

    using PetClinic.Models;
    using PetClinic.DataProcessor.Dto.Import;

    public class PetClinicProfile : Profile
    {
        public PetClinicProfile()
        {
            CreateMap<AnimalDto, Animal>()
                .ForMember(a =>
                    a.PassportSerialNumber, id =>
                        id.MapFrom(dto => dto.Passport.SerialNumber));
            CreateMap<PassportDto, Passport>()
                .ForMember(p => 
                    p.RegistrationDate, rd => 
                        rd.MapFrom(dto => 
                            DateTime.ParseExact(dto.RegistrationDate, "dd-MM-yyyy", CultureInfo.InvariantCulture)));
        }
    }
}
