namespace PetClinic.DataProcessor.Dto.Import
{
    public class AnimalDto
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public int Age { get; set; }
        public PassportDto Passport { get; set; }
    }
}
