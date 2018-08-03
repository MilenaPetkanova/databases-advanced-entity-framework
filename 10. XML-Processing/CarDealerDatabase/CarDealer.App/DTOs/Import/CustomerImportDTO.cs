namespace CarDealer.App.DTOs.Import
{
    using System;
    using System.Xml.Serialization;

    [XmlType("customer")]
    public class CustomerImportDTO
    {
        [XmlAttribute("name")]
        public string Name { get; set; }

        [XmlElement("birth-date")]
        public DateTime BirthDate { get; set; }

        [XmlElement("is-young-driver")]
        public bool IsYoungDriver { get; set; }
    }
}
