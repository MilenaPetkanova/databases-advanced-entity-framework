namespace PetClinic.DataProcessor
{
    using System;
    using System.Text;
    using System.Collections.Generic;
    using System.Linq;
    using System.Globalization;
    using System.Xml.Serialization;
    using System.IO;

    using Newtonsoft.Json;

    using PetClinic.Data;
    using PetClinic.DataProcessor.Dto.Import;
    using PetClinic.Models;

    public class Deserializer
    {
        public const string ERROR_MESSAGE = "Error: Invalid data.";

        public static string ImportAnimalAids(PetClinicContext context, string jsonString)
        {
            var sb = new StringBuilder();

            var deserializedAnimalAidDtos = JsonConvert.DeserializeObject<AnimalAidDto[]>(jsonString);

            var animalAids = new List<AnimalAid>();

            foreach (var animalAidDto in deserializedAnimalAidDtos)
            {
                var alreadyExists = animalAids.Any(aa => aa.Name == animalAidDto.Name);

                if (!IsValid(animalAidDto) || alreadyExists)
                {
                    sb.AppendLine(ERROR_MESSAGE);
                    continue;
                }

                var animalAid = new AnimalAid
                {
                    Name = animalAidDto.Name,
                    Price = animalAidDto.Price
                };

                animalAids.Add(animalAid);

                sb.AppendLine($"Record {animalAidDto.Name} successfully imported.");
            }

            context.AnimalAids.AddRange(animalAids);
            context.SaveChanges();

            var result = sb.ToString();
            return result;
        }

        public static string ImportAnimals(PetClinicContext context, string jsonString)
        {
            var sb = new StringBuilder();

            var deserializedAnimalDtos = JsonConvert.DeserializeObject<AnimalDto[]>(jsonString);

            var passports = new List<Passport>();
            var animals = new List<Animal>();

            foreach (var animalDto in deserializedAnimalDtos)
            {
                var passportExists = passports.Any(p => p.SerialNumber.Equals(animalDto.Passport.SerialNumber));

                if (!IsValid(animalDto) || (!IsValid(animalDto.Passport)) || passportExists)
                {
                    sb.AppendLine(ERROR_MESSAGE);
                    continue;
                }

                var animal = new Animal
                {
                    Name = animalDto.Name,
                    Type = animalDto.Type,
                    Age = animalDto.Age,
                    PassportSerialNumber = animalDto.Passport.SerialNumber
                };

                var registrationDate = DateTime.ParseExact(animalDto.Passport.RegistrationDate, "dd-MM-yyyy", CultureInfo.InvariantCulture);

                var passport = new Passport
                {
                    SerialNumber = animalDto.Passport.SerialNumber,
                    Animal = animal,
                    OwnerPhoneNumber = animalDto.Passport.OwnerPhoneNumber,
                    OwnerName = animalDto.Passport.OwnerName,
                    RegistrationDate = registrationDate
                };

                animals.Add(animal);
                passports.Add(passport);

                sb.AppendLine($"Record {animal.Name} Passport №: {passport.SerialNumber} successfully imported.");
            }

            context.Animals.AddRange(animals);
            context.SaveChanges();

            context.AddRange(passports);
            context.SaveChanges();

            var result = sb.ToString();
            return result;
        }

        public static string ImportVets(PetClinicContext context, string xmlString)
        {
            var sb = new StringBuilder();

            var serializer = new XmlSerializer(typeof(VetDto[]), new XmlRootAttribute("Vets"));
            var deserializedVetDtos = (VetDto[])serializer.Deserialize(new StringReader(xmlString));

            var vets = new List<Vet>();

            foreach (var vetDto in deserializedVetDtos)
            {
                var vetWithSameProneNum = vets.SingleOrDefault(v => v.PhoneNumber.Equals(vetDto.PhoneNumber));

                if (!IsValid(vetDto) || vetWithSameProneNum != null)
                {
                    sb.AppendLine(ERROR_MESSAGE);
                    continue;
                }
                
                var vet = new Vet
                {
                    Name = vetDto.Name,
                    Profession = vetDto.Profession,
                    Age = vetDto.Age,
                    PhoneNumber = vetDto.PhoneNumber
                };

                vets.Add(vet);

                sb.AppendLine($"Record {vetDto.Name} successfully imported.");
            }

            context.Vets.AddRange(vets);
            context.SaveChanges();

            var result = sb.ToString();
            return result;
        }

        public static string ImportProcedures(PetClinicContext context, string xmlString)
        {
            var sb = new StringBuilder();

            var serializer = new XmlSerializer(typeof(ProcedureDto[]), new XmlRootAttribute("Procedures"));
            var deserializedProductDtos = (ProcedureDto[])serializer.Deserialize(new StringReader(xmlString));

            var procedures = new List<Procedure>();
            var procedureAnimalAids = new List<ProcedureAnimalAid>();
            
            foreach (var procedureDto in deserializedProductDtos)
            {
                var vet = context.Vets.SingleOrDefault(v => v.Name == procedureDto.Vet);
                var animal = context.Animals.SingleOrDefault(a => a.PassportSerialNumber == procedureDto.Animal);
                if (!IsValid(procedureDto) || vet == null || animal == null)
                {
                    sb.AppendLine(ERROR_MESSAGE);
                    continue;
                }

                var dateTime = DateTime.ParseExact(procedureDto.DateTime, "dd-MM-yyyy", CultureInfo.InvariantCulture);
                var procedure = new Procedure
                {
                    Vet = vet,
                    Animal = animal,
                    DateTime = dateTime
                };

                var animalAidsAreValid = true;
                var validProcedureAnimalAids = new List<ProcedureAnimalAid>();

                foreach (var animalAidDto in procedureDto.AnimalAids)
                {
                    var animalAid = context.AnimalAids.SingleOrDefault(aa => aa.Name.Equals(animalAidDto.Name));
                    var alreadyAddes = validProcedureAnimalAids
                        .Any(vpaa => vpaa.AnimalAid.Name.Equals(animalAidDto.Name));

                    if (!IsValid(animalAidDto) || animalAid == null || alreadyAddes)
                    {
                        animalAidsAreValid = false;
                        break;
                    }

                    var procedureAnimalAid = new ProcedureAnimalAid
                    {
                        Procedure = procedure,
                        AnimalAid = animalAid
                    };

                    validProcedureAnimalAids.Add(procedureAnimalAid);
                }

                if (!animalAidsAreValid)
                {
                    sb.AppendLine(ERROR_MESSAGE);
                    continue;
                }

                procedures.Add(procedure);
                procedureAnimalAids.AddRange(validProcedureAnimalAids);

                sb.AppendLine($"Record successfully imported.");
            }

            context.Procedures.AddRange(procedures);
            context.SaveChanges();

            context.ProceduresAnimalAids.AddRange(procedureAnimalAids);
            context.SaveChanges();

            var result = sb.ToString();
            return result;
        }

        private static bool IsValid(object obj)
        {
            var validatonContext = new System.ComponentModel.DataAnnotations.ValidationContext(obj);
            var validationResults = new List<System.ComponentModel.DataAnnotations.ValidationResult>();

            return System.ComponentModel.DataAnnotations.Validator.TryValidateObject(obj, validatonContext, validationResults, true);
        }
    }
}
