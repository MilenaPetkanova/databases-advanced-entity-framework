namespace SoftJail.DataProcessor
{

    using Data;
    using Newtonsoft.Json;
    using SoftJail.Data.Models;
    using SoftJail.Data.Models.Enums;
    using SoftJail.DataProcessor.ImportDto;
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Xml.Serialization;

    public class Deserializer
    {
        public const string INVALID_DATA = "Invalid Data";

        public static string ImportDepartmentsCells(SoftJailDbContext context, string jsonString)
        {
            var sb = new StringBuilder();

            var deserializedDepartmentDtos = JsonConvert.DeserializeObject<DepartmentDto[]>(jsonString);

            var departments = new List<Department>();
            var cellsCount = 0;

            foreach (var departmentDto in deserializedDepartmentDtos)
            {
                if (!IsValid(departmentDto) || departmentDto.Cells.Any(c => !IsValid(c)))
                {
                    sb.AppendLine(INVALID_DATA);
                    continue;
                }

                var department = new Department
                {
                    Name = departmentDto.Name
                };

                foreach (var cellDto in departmentDto.Cells)
                {
                    var cell = new Cell
                    {
                        CellNumber = cellDto.CellNumber,
                        HasWindow = cellDto.HasWindow,
                        Department = department
                    };

                    department.Cells.Add(cell);
                    cellsCount++;
                }

                departments.Add(department);

                sb.AppendLine($"Imported {department.Name} with {cellsCount} cells");
            }

            context.Departments.AddRange(departments);
            context.SaveChanges();

            var result = sb.ToString();
            return result;
        }

        public static string ImportPrisonersMails(SoftJailDbContext context, string jsonString)
        {
            var sb = new StringBuilder();

            var deserializedPrisonerDtos = JsonConvert.DeserializeObject<PrisonerDto[]>(jsonString);

            var prisoners = new List<Prisoner>();

            foreach (var prisonerDto in deserializedPrisonerDtos)
            {
                if (!IsValid(prisonerDto) || prisonerDto.Mails.Any(m => !IsValid(m)))
                {
                    sb.AppendLine(INVALID_DATA);
                    continue;
                }

                DateTime.TryParseExact(prisonerDto.ReleaseDate, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime releaseDate);
                var incarcerationDate = DateTime.ParseExact(prisonerDto.IncarcerationDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);

                var prisoner = new Prisoner
                {
                    FullName = prisonerDto.FullName,
                    Nickname = prisonerDto.Nickname,
                    Age = prisonerDto.Age,
                    IncarcerationDate = incarcerationDate,
                    ReleaseDate = releaseDate,
                    Bail = prisonerDto.Bail ?? 0,
                    CellId = prisonerDto.CellId,
                };

                foreach (var mailDto in prisonerDto.Mails)
                {
                    var mail = new Mail
                    {
                        Description = mailDto.Description,
                        Sender = mailDto.Sender,
                        Address = mailDto.Address
                    };

                    prisoner.Mails.Add(mail);
                }

                prisoners.Add(prisoner);

                sb.AppendLine($"Imported {prisoner.FullName} {prisoner.Age} years old");
            }

            context.AddRange(prisoners);
            context.SaveChanges();

            var result = sb.ToString();
            return result;
        }

        public static string ImportOfficersPrisoners(SoftJailDbContext context, string xmlString)
        {
            var sb = new StringBuilder();

            var serializer = new XmlSerializer(typeof(OfficerDto[]), new XmlRootAttribute("Officers"));
            var deserializedOfficerDtos = (OfficerDto[])serializer.Deserialize(new StringReader(xmlString));

            var officers = new List<Officer>();

            foreach (var officedDto in deserializedOfficerDtos)
            {
                if (!IsValid(officedDto))
                {
                    sb.AppendLine(INVALID_DATA);
                    continue;
                }

                Position positionEnum;
                Weapon weaponEnum;

                if (!Enum.TryParse(officedDto.Position, out positionEnum) || !Enum.TryParse(officedDto.Weapon, out weaponEnum))
                {
                    sb.AppendLine(INVALID_DATA);
                    continue;
                }

                var officer = new Officer
                {
                    FullName = officedDto.FullName,
                    Salary = officedDto.Salary,
                    Position = positionEnum,
                    Weapon = weaponEnum,
                    DepartmentId = officedDto.DepartmentId
                };

                foreach (var prisonerDto in officedDto.Prisoners)
                {
                    var officerPrisoner = new OfficerPrisoner
                    {
                        Officer = officer,
                        PrisonerId = prisonerDto.PrisonerId
                    };

                    officer.OfficerPrisoners.Add(officerPrisoner);
                }

                officers.Add(officer);

                sb.AppendLine($"Imported {officer.FullName} ({officer.OfficerPrisoners.Count} prisoners)");
            }

            context.Officers.AddRange(officers);
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