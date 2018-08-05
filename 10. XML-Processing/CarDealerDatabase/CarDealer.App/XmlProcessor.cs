namespace CarDealer.App
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Xml.Serialization;
    using System.Text;
    using System.Xml;
    using AutoMapper;

    using CarDealer.App.DTOs.Import;
    using CarDealer.App.DTOs.Export;
    using CarDealer.Data;
    using CarDealer.Models;
    
    public class XmlProcessor
    {
        private const string PATH_TO_IMPORT = "Xmls/Import/";
        private const string PATH_TO_EXPORT = "Xmls/Export/";

        private CarDealerContext context;

        public XmlProcessor()
        {
            this.context = new CarDealerContext();
        }

        public void ImportData()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<CarDealerProfile>();
            });

            IMapper mapper = config.CreateMapper();

            this.ImportSuppliers(mapper);
            this.ImportParts(mapper);
            this.ImportCars(mapper);
            this.ImportCustomers(mapper);
            this.ImportSales(mapper);
        }

        public void ExportData()
        {
            this.ExportCarsWithDistance();
            this.ExportCarsFromMakeFerrari();
            this.ExportLocalSuppliers();
            this.ExportCarsWithParts();
            this.ExportTotalSalesByCustomer();
            this.SalesWithAppliedDiscount();
        }

        private void ImportSuppliers(IMapper mapper)
        {
            var xmlString = File.ReadAllText(PATH_TO_IMPORT + "suppliers.xml");
            var serializer = new XmlSerializer(typeof(SupplierImportDTO[]), new XmlRootAttribute("suppliers"));
            var deserializedSupplierDTOs = (SupplierImportDTO[])serializer.Deserialize(new StringReader(xmlString));

            var suppliers = new List<Supplier>();

            foreach (var supplierDTO in deserializedSupplierDTOs)
            {
                if (!this.IsValid(supplierDTO))
                {
                    continue;
                }

                var supplier = mapper.Map<Supplier>(supplierDTO);
                suppliers.Add(supplier);
            }

            this.context.Suppliers.AddRange(suppliers);
            this.context.SaveChanges();
        }

        private void ImportParts(IMapper mapper)
        {
            var xmlString = File.ReadAllText(PATH_TO_IMPORT + "parts.xml");
            var serializer = new XmlSerializer(typeof(PartImportDTO[]), new XmlRootAttribute("parts"));
            var deserializedPartDTOs = (PartImportDTO[])serializer.Deserialize(new StringReader(xmlString));

            var parts = new List<Part>();

            foreach (var partDTO in deserializedPartDTOs)
            {
                if (!this.IsValid(partDTO))
                {
                    throw new InvalidOperationException("Invalid deserialized partDTO!");
                }

                var part = mapper.Map<Part>(partDTO);
                parts.Add(part);
            }

            var partsWithSuppliers = this.GenerateRandomSuppliers(parts);

            this.context.Parts.AddRange(partsWithSuppliers);
            this.context.SaveChanges();
        }

        private List<Part> GenerateRandomSuppliers(List<Part> parts)
        {
            var random = new Random();
            var suppliersCount = this.context.Suppliers.Select(s => s.Id).Count();

            foreach (var part in parts)
            {
                part.Supplier_Id = random.Next(1, suppliersCount);
            }

            return parts;
        }

        private void ImportCars(IMapper mapper)
        {
            var xmlString = File.ReadAllText(PATH_TO_IMPORT + "cars.xml");
            var serializer = new XmlSerializer(typeof(CarImportDTO[]), new XmlRootAttribute("cars"));
            var deserializedCarDTOs = (CarImportDTO[])serializer.Deserialize(new StringReader(xmlString));

            var cars = new List<Car>();

            foreach (var carDTO in deserializedCarDTOs)
            {
                if (!this.IsValid(carDTO))
                {
                    throw new InvalidOperationException("Invalid deserialized carDTO.");
                }

                var car = mapper.Map<Car>(carDTO);
                cars.Add(car);
            }

            this.context.Cars.AddRange(cars);
            this.context.SaveChanges();

            var partsCars = this.GenerateCarParts();

            this.context.PartCars.AddRange(partsCars);
            this.context.SaveChanges();
        }

        private List<PartCar> GenerateCarParts()
        {
            var partsCars = new List<PartCar>();

            var random = new Random();
            var allPartsCount = this.context.Parts.Select(p => p.Id).Count();
            var minPartsPerCar = 10;
            var maxPartsPerCar = 20;

            foreach (var car in this.context.Cars)
            {
                var partsPerCarCount = random.Next(minPartsPerCar, maxPartsPerCar);

                var randomPartIdsPerCar = new List<int>();

                for (int i = 0; i < partsPerCarCount; i++)
                {
                    var randomPartId = random.Next(1, allPartsCount);
                    if (randomPartIdsPerCar.Contains(randomPartId))
                    {
                        i--;
                        continue;
                    }
                    randomPartIdsPerCar.Add(randomPartId);

                    var partCar = new PartCar
                    {
                        Part_Id = randomPartId,
                        Car_Id = car.Id
                    };

                    partsCars.Add(partCar);
                }
            }

            return partsCars;
        }

        private void ImportCustomers(IMapper mapper)
        {
            var xmlString = File.ReadAllText(PATH_TO_IMPORT + "customers.xml");
            var serializer = new XmlSerializer(typeof(CustomerImportDTO[]), new XmlRootAttribute("customers"));
            var deserializedCustomerDTOs = (CustomerImportDTO[])serializer.Deserialize(new StringReader(xmlString));

            var customers = new List<Customer>();

            foreach (var customerDTO in deserializedCustomerDTOs)
            {
                if (!this.IsValid(customerDTO))
                {
                    continue;
                }

                var customer = mapper.Map<Customer>(customerDTO);
                customers.Add(customer);
            }

            this.context.Customers.AddRange(customers);
            this.context.SaveChanges();
        }

        private void ImportSales(IMapper mapper)
        {
            var discounts = new decimal[] { 0, 0.05M, 0.1M, 0.15M, 0.2M, 0.3M, 0.4M, 0.5M };
            var carsCount = this.context.Cars.Select(c => c.Id).Count();
            var customersCount = this.context.Customers.Select(c => c.Id).Count();

            var random = new Random();

            var sales = new List<Sale>();

            var salesCount = 6363;
            for (int i = 0; i < salesCount; i++)
            {
                var sale = new Sale
                {
                    Discount = discounts[random.Next(0, discounts.Length - 1)],
                    Car_Id = random.Next(1, carsCount),
                    Customer_Id = random.Next(1, customersCount)
                };

                sales.Add(sale);
            }

            this.context.Sales.AddRange(sales);
            this.context.SaveChanges();
        }

        private bool IsValid(object obj)
        {
            var validatonContext = new System.ComponentModel.DataAnnotations.ValidationContext(obj);
            var validationResults = new List<System.ComponentModel.DataAnnotations.ValidationResult>();

            return System.ComponentModel.DataAnnotations.Validator.TryValidateObject(obj, validatonContext, validationResults, true);
        }

        private void ExportCarsWithDistance()
        {
            var carDTOs = this.context.Cars
                .Where(c => c.TravelledDistance > 2000000)
                .Select(c => new CarElementsDTO
                {
                    Make = c.Make,
                    Model = c.Model,
                    TravelledDistance = c.TravelledDistance
                })
                .OrderBy(c => c.Make)
                .ThenBy(c => c.Model)
                .ToArray();

            var sb = new StringBuilder();
            var xmlNamespaces = new XmlSerializerNamespaces(new[] { XmlQualifiedName.Empty });

            var serializer = new XmlSerializer(typeof(CarElementsDTO[]), new XmlRootAttribute("cars"));
            serializer.Serialize(new StringWriter(sb), carDTOs, xmlNamespaces);

            File.WriteAllText(PATH_TO_EXPORT + "cars.xml", sb.ToString());
        }

        private void ExportCarsFromMakeFerrari()
        {
            var carDTOs = this.context.Cars
                .Where(c => c.Make.Equals("Ferrari"))
                .Select(c => new CarAttributesDTO
                {
                    Id = c.Id,
                    Model = c.Model,
                    TravelledDistance = c.TravelledDistance
                })
                .ToArray()
                .OrderBy(c => c.Model)
                .ThenByDescending(c => c.TravelledDistance)
                .ToArray();

            var sb = new StringBuilder();
            var xmlNamespaces = new XmlSerializerNamespaces(new[] { XmlQualifiedName.Empty });

            var serializer = new XmlSerializer(typeof(CarAttributesDTO[]), new XmlRootAttribute("cars"));
            serializer.Serialize(new StringWriter(sb), carDTOs, xmlNamespaces);

            File.WriteAllText(PATH_TO_EXPORT + "ferrari-cars.xml", sb.ToString());
        }

        private void ExportLocalSuppliers()
        {
            var localSupplierDTOs = this.context.Suppliers
                .Where(s => s.IsImporter.Equals(false))
                .Select(s => new LocalSupplierDTO
                {
                    Id = s.Id,
                    Name = s.Name,
                    PartsCount = s.Parts.Count
                })
                .ToArray();

            var sb = new StringBuilder();
            var xmlNamespaces = new XmlSerializerNamespaces(new[] { XmlQualifiedName.Empty });

            var serializer = new XmlSerializer(typeof(LocalSupplierDTO[]), new XmlRootAttribute("suppliers"));
            serializer.Serialize(new StringWriter(sb), localSupplierDTOs, xmlNamespaces);

            File.WriteAllText(PATH_TO_EXPORT + "local-suppliers.xml", sb.ToString());
        }

        private void ExportCarsWithParts()
        {
            var carPartsDTOs = this.context.Cars
                .Select(c => new CarPartsDTO
                {
                    Make = c.Make,
                    Model = c.Model,
                    TravelledDistance = c.TravelledDistance,
                    CarParts = c.PartCars.Select(pc => new PartAttributesDTO
                    {
                        Name = pc.Part.Name,
                        Price = pc.Part.Price
                    })
                    .ToArray()
                })
                .ToArray();

            var sb = new StringBuilder();
            var xmlNamespaces = new XmlSerializerNamespaces(new[] { XmlQualifiedName.Empty });

            var serializer = new XmlSerializer(typeof(CarPartsDTO[]), new XmlRootAttribute("cars"));
            serializer.Serialize(new StringWriter(sb), carPartsDTOs, xmlNamespaces);

            File.WriteAllText(PATH_TO_EXPORT + "cars-and-parts.xml", sb.ToString());
        }

        private void ExportTotalSalesByCustomer()
        {
            var customerTotalSalesDTOs = this.context.Customers
                .Where(c => c.Sales.Count > 0)
                .Select(c => new CustomerTotalSalesDTO
                {
                    Name = c.Name,
                    BoughtCars = c.Sales.Count,
                    SpentMoney = c.Sales
                        .Select(s => s.Car.PartCars.Sum(cp => cp.Part.Price) * (1 + s.Discount))
                        .DefaultIfEmpty(0)
                        .Sum()
                })
                .OrderByDescending(c => c.SpentMoney)
                .ThenByDescending(c => c.BoughtCars)
                .ToArray();

            var sb = new StringBuilder();
            var xmlNamespaces = new XmlSerializerNamespaces(new[] { XmlQualifiedName.Empty });

            var serializer = new XmlSerializer(typeof(CustomerTotalSalesDTO[]), new XmlRootAttribute("customers"));
            serializer.Serialize(new StringWriter(sb), customerTotalSalesDTOs, xmlNamespaces);

            File.WriteAllText(PATH_TO_EXPORT + "customers-total-sales.xml", sb.ToString());
        }

        private void SalesWithAppliedDiscount()
        {
            var saleDiscountsDTOs = this.context.Sales
                .Select(s => new SaleDiscountsDTO
                {
                    Car = new CarFromSaleDTO
                    {
                        Make = s.Car.Make,
                        Model = s.Car.Model,
                        TravelledDistance = s.Car.TravelledDistance
                    },
                    CustomerName = s.Customer.Name,
                    Discount = s.Discount,
                    Price = s.Car.PartCars
                        .Select(pc => pc.Part.Price * (1 + s.Discount))
                        .DefaultIfEmpty(0)
                        .Sum(),
                    PriceWithDiscount = s.Car.PartCars
                        .Select(pc => pc.Part.Price)
                        .DefaultIfEmpty(0)
                        .Sum(),
                })
                .ToArray();

            var sb = new StringBuilder();
            var xmlNamespaces = new XmlSerializerNamespaces(new[] { XmlQualifiedName.Empty });

            var serializer = new XmlSerializer(typeof(SaleDiscountsDTO[]), new XmlRootAttribute("sales"));
            serializer.Serialize(new StringWriter(sb), saleDiscountsDTOs, xmlNamespaces);

            File.WriteAllText(PATH_TO_EXPORT + "sales-discounts.xml", sb.ToString());
        }
    }
}