namespace ProductShop.App.Dtos.Export
{
    using System.Xml.Serialization;

    [XmlType("user")]
    public class UserDTO
    {
        [XmlAttribute("first-name")]
        public string FirstName { get; set; }

        [XmlAttribute("last-name")]
        public string LastName { get; set; }

        [XmlIgnore()]
        public int? Age { get; set; }

        [XmlAttribute("age")]
        public string AgeValue
        {
            get
            {
                if (this.Age.HasValue)
                {
                    return this.Age.Value.ToString();
                }
                else
                {
                    return null;
                }
            }
            set
            {
                if (value != "0")
                {
                    this.Age = int.Parse(value);
                }
                else
                {
                    this.Age = null;
                }
            }
        }

        [XmlElement("sold-products")]
        public SoldProductsCollectionDTO SoldProducts { get; set; }
    }
}
