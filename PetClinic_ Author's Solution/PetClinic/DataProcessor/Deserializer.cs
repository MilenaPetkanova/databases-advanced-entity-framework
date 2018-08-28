namespace PetClinic.DataProcessor
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.Text;
    using System.Xml.Linq;

    using Newtonsoft.Json;
    using AutoMapper;

    using PetClinic.Data;
    using PetClinic.Models;
    using Dto.Import;
    using System.Globalization;

    public class Deserializer
    {
        private const string FailureMessage = "Error: Invalid data.";
        private const string SuccessMessage = "Record {0} successfully imported.";

        public static string ImportAnimalAids(PetClinicContext context, string jsonString)
        {
            AnimalAid[] animalAids = JsonConvert.DeserializeObject<AnimalAid[]>(jsonString);

            var validEntries = new List<AnimalAid>();

            var sb = new StringBuilder();

            foreach (var aa in animalAids)
            {
                bool isValid = IsValid(aa);

				bool alreadyExists = validEntries.Any(a => a.Name == aa.Name);
				
                if (!isValid || alreadyExists)
                {
                    sb.AppendLine(FailureMessage);
                    continue;
                }
               
                validEntries.Add(aa);
                sb.AppendLine(String.Format(SuccessMessage, aa.Name));
            }

            context.AnimalAids.AddRange(validEntries);
            context.SaveChanges();

            string result = sb.ToString().TrimEnd();

            return result;
        }

        public static string ImportAnimals(PetClinicContext context, string jsonString)
        {
            AnimalDto[] animals = JsonConvert.DeserializeObject<AnimalDto[]>(jsonString);

            var sb = new StringBuilder();
            var validEntries = new List<Animal>();

            foreach (var dto in animals)
            {
                Animal animal = Mapper.Map<Animal>(dto);

                bool animalIsValid = IsValid(animal);
                bool passportIsValid = IsValid(animal.Passport);

                bool alreadyExists = validEntries.Any(a => a.Passport.SerialNumber == animal.Passport.SerialNumber);

                if (!animalIsValid || !passportIsValid || alreadyExists)
                {
                    sb.AppendLine(FailureMessage);
                    continue;
                }

                validEntries.Add(animal);
                sb.AppendLine(String.Format(SuccessMessage, $"{animal.Name} Passport №: {animal.Passport.SerialNumber}"));
            }

            context.Animals.AddRange(validEntries);
            context.SaveChanges();

            string result = sb.ToString().TrimEnd();
            return result;
        }

        public static string ImportVets(PetClinicContext context, string xmlString)
        {
            var xDoc = XDocument.Parse(xmlString);
            var elements = xDoc.Root.Elements();

            var sb = new StringBuilder();
            var validEntries = new List<Vet>();

            foreach (var el in elements)
            {
                string name = el.Element("Name")?.Value;
                string profession = el.Element("Profession")?.Value;
                string ageString = el.Element("Age")?.Value;
                string phoneNumber = el.Element("PhoneNumber")?.Value;

                //int age = ageString != null ? int.Parse(ageString) : 0;

                int age = 0;

                if (ageString != null)
                {
                    age = int.Parse(ageString);
                }

                Vet vet = new Vet()
                {
                    Name = name,
                    Profession = profession,
                    Age = age,
                    PhoneNumber = phoneNumber,
                };

                bool isValid = IsValid(vet);
                bool phoneNumberExists = validEntries.Any(v => v.PhoneNumber == vet.PhoneNumber);

                if (!isValid || phoneNumberExists)
                {
                    sb.AppendLine(FailureMessage);
                    continue;
                }

                validEntries.Add(vet);
                sb.AppendLine(String.Format(SuccessMessage, vet.Name));
            }

            context.Vets.AddRange(validEntries);
            context.SaveChanges();

            string result = sb.ToString().TrimEnd();
            return result;
        }

        public static string ImportProcedures(PetClinicContext context, string xmlString)
        {
            var xDoc = XDocument.Parse(xmlString);
            var elements = xDoc.Root.Elements();

            var sb = new StringBuilder();
            var validEntries = new List<Procedure>();

            foreach (var el in elements)
            {
                string vetName = el.Element("Vet")?.Value;
                string passportId = el.Element("Animal")?.Value;
                string dateTimeString = el.Element("DateTime")?.Value;

                int? vetId = context.Vets.SingleOrDefault(v => v.Name == vetName)?.Id;
                bool passportExists = context.Passports.Any(p => p.SerialNumber == passportId);

                bool dateIsValid = DateTime
                    .TryParseExact(dateTimeString, "dd-MM-yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime dateTime);

                var animalAidElements = el.Element("AnimalAids")?.Elements();

                if (vetId == null || !passportExists || animalAidElements == null || !dateIsValid)
                {
                    sb.AppendLine(FailureMessage);
                    continue;
                }

                var animalAidIds = new List<int>();
                bool allAidsExist = true;

                foreach (var aid in animalAidElements)
                {
                    string aidName = aid.Element("Name")?.Value;

                    int? aidId = context.AnimalAids.SingleOrDefault(a => a.Name == aidName)?.Id;

                    if (aidId == null || animalAidIds.Any(id => id == aidId))
                    {
                        allAidsExist = false;
                        break;
                    }

                    animalAidIds.Add(aidId.Value);
                }

                if (!allAidsExist)
                {
                    sb.AppendLine(FailureMessage);
                    continue;
                }

                var procedure = new Procedure()
                {
                    VetId = vetId.Value,
                    AnimalId = context.Animals.Single(a => a.PassportSerialNumber == passportId).Id,
                    DateTime = dateTime,
                };

                foreach (var id in animalAidIds)
                {
                    var mapping = new ProcedureAnimalAid()
                    {
                        Procedure = procedure,
                        AnimalAidId = id
                    };

                    procedure.ProcedureAnimalAids.Add(mapping);
                }

                bool isValid = IsValid(procedure);

                if (!isValid)
                {
                    sb.AppendLine(FailureMessage);
                    continue;
                }

                validEntries.Add(procedure);
                sb.AppendLine("Record successfully imported.");
            }

            context.Procedures.AddRange(validEntries);
            context.SaveChanges();

            string result = sb.ToString().TrimEnd();
            return result;
        }

        private static bool IsValid(object obj)
        {
            var validationContext = new System.ComponentModel.DataAnnotations.ValidationContext(obj);
            var validationResults = new List<ValidationResult>();

            var isValid = Validator.TryValidateObject(obj, validationContext, validationResults, true);

            return isValid;
        }
    }
}
