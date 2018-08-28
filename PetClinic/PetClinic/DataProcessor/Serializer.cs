namespace PetClinic.DataProcessor
{
    using System;

    using PetClinic.Data;
    using Newtonsoft.Json;
    using System.Linq;
    using PetClinic.DataProcessor.Dto.Export;
    using System.Globalization;
    using System.Xml.Serialization;
    using System.Text;
    using System.IO;
    using System.Xml;

    public class Serializer
    {
        public static string ExportAnimalsByOwnerPhoneNumber(PetClinicContext context, string phoneNumber)
        {
            var animalDtos = context.Animals
                .Where(a => a.Passport.OwnerPhoneNumber == phoneNumber)
                .Select(a => new AnimalDto
                {
                    OwnerName = a.Passport.OwnerName,
                    AnimalName = a.Name,
                    Age = a.Age,
                    SerialNumber = a.PassportSerialNumber,
                    RegisteredOn = a.Passport.RegistrationDate.ToString("dd-MM-yyyy", CultureInfo.InvariantCulture)
                })
                .OrderBy(a => a.Age)
                .ThenBy(a => a.SerialNumber)
                .ToArray();

            var jsonSerializerSettings = GetDefaultNullValueHandling();

            var json = JsonConvert.SerializeObject(animalDtos, Newtonsoft.Json.Formatting.Indented, jsonSerializerSettings);

            return json;
        }

        public static string ExportAllProcedures(PetClinicContext context)
        {
            var procedureDtos = context.Procedures
                .Select(p => new ProcedureDto
                {
                    Passport = p.Animal.PassportSerialNumber,
                    OwnerNumber = p.Animal.Passport.OwnerPhoneNumber,
                    DateTime = p.DateTime.ToString("dd-MM-yyyy", CultureInfo.InvariantCulture),
                    AnimalAids = p.ProcedureAnimalAids.Select(paa => new AnimalAidDto
                    {
                        Name = paa.AnimalAid.Name,
                        Price = paa.AnimalAid.Price
                    })
                    .ToArray(),
                    TotalPrice = p.ProcedureAnimalAids.Sum(paa => paa.AnimalAid.Price)
                })
                .OrderBy(p => p.DateTime)
                .ThenBy(p => p.Passport)
                .ToArray();

            var sb = new StringBuilder();
            var xmlNamespaces = new XmlSerializerNamespaces(new[] { XmlQualifiedName.Empty });

            var serializer = new XmlSerializer(typeof(ProcedureDto[]), new XmlRootAttribute("Procedures"));
            serializer.Serialize(new StringWriter(sb), procedureDtos, xmlNamespaces);

            var result = sb.ToString();
            return result;
        }

        private static JsonSerializerSettings GetDefaultNullValueHandling()
        {
            var settings = new JsonSerializerSettings
            {
                DefaultValueHandling = DefaultValueHandling.Ignore
            };

            return settings;
        }
    }
}
