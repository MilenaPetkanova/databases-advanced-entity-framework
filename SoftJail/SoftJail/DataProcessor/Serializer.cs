namespace SoftJail.DataProcessor
{

    using Data;
    using Newtonsoft.Json;
    using SoftJail.DataProcessor.ExportDto;
    using System;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Xml;
    using System.Xml.Serialization;

    public class Serializer
    {
        public static string ExportPrisonersByCells(SoftJailDbContext context, int[] ids)
        {
            var sb = new StringBuilder();

            var prisonerDtos = context.Prisoners
                .Where(p => ids.Contains(p.Id) && p.CellId != 0)
                .Select(p => new PrisonerDto
                {
                    Id = p.Id,
                    FullName = p.FullName,
                    CellNumber = p.Cell.CellNumber,
                    Officers = p.PrisonerOfficers.Select(po => new OfficerDto
                    {
                        FullName = po.Officer.FullName,
                        Department = po.Officer.Department.Name
                    })
                    .OrderBy(o => o.FullName)
                    .ToArray(),
                    TotalOfficerSalary = Math.Round(p.PrisonerOfficers.Sum(po => po.Officer.Salary), 2)
                })
                .OrderBy(p => p.FullName)
                .ThenBy(p => p.Id)
                .ToArray();

            var jsonSerializerSettings = GetDefaultNullValueHandling();

            var json = JsonConvert.SerializeObject(prisonerDtos, Newtonsoft.Json.Formatting.Indented, jsonSerializerSettings);

            return json;
        }

        public static string ExportPrisonersInbox(SoftJailDbContext context, string prisonersNames)
        {
            var prisonerDtos = context.Prisoners
                .Where(p => prisonersNames.Contains(p.FullName))
                .Select(p => new PrisonerInboxDto
                {
                    Id = p.Id,
                    Name = p.FullName,
                    IncarcerationDate = p.IncarcerationDate.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
                    EncryptedMessages = p.Mails.Select(m => new MessageDto
                    {
                        Description = Reverse(m.Description)
                    })
                    .ToArray()
                })
                .OrderBy(p => p.Name)
                .ThenBy(p => p.Id)
                .ToArray();

            var sb = new StringBuilder();
            var xmlNamespaces = new XmlSerializerNamespaces(new[] { XmlQualifiedName.Empty });

            var serializer = new XmlSerializer(typeof(PrisonerInboxDto[]), new XmlRootAttribute("Prisoners"));
            serializer.Serialize(new StringWriter(sb), prisonerDtos, xmlNamespaces);

            var result = sb.ToString();
            return result;
        }


        public static string Reverse(string s)
        {
            char[] charArray = s.ToCharArray();
            Array.Reverse(charArray);
            return new string(charArray);
        }

        public static JsonSerializerSettings GetDefaultNullValueHandling()
        {
            var settings = new JsonSerializerSettings
            {
                DefaultValueHandling = DefaultValueHandling.Ignore
            };

            return settings;
        }
    }
}