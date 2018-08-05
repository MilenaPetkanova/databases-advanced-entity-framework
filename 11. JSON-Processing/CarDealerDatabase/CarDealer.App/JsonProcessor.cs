namespace CarDealer.App
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    using Newtonsoft.Json;

    using CarDealer.App.DTOs;
    using CarDealer.Data;
    using CarDealer.Models;
    
    public class JsonProcessor
    {
        private const string PATH_TO_IMPORT = "Jsons/Import/";
        private const string PATH_TO_EXPORT = "Jsons/Export/";

        private CarDealerContext context;

        public JsonProcessor()
        {
            this.context = new CarDealerContext();
        }

        public void ImportData()
        {
            this.ImportSuppliers();
            this.ImportParts();
            this.ImportCars();
            this.GenerateAndImportCarParts();
            this.ImportCustomers();
            this.GenerateAndImportSales();
        }

        public void ExportData()
        {
            this.ExportOrderedCustomers();
            this.ExportCarsFromMakeToyota();
            this.ExportLocalSuppliers();
            this.ExportCarsWithTheirParts();
            this.ExportTotalSalesByCustomer();
            this.SalesWithAppliedDiscount();
        }

        private void ImportSuppliers()
        {
            var jsonString = File.ReadAllText(PATH_TO_IMPORT + "suppliers.json");

            var deserializedSuppliers = JsonConvert.DeserializeObject<Supplier[]>(jsonString);

            this.context.Suppliers.AddRange(deserializedSuppliers);
            this.context.SaveChanges();
        }

        private void ImportParts()
        {
            var jsonString = File.ReadAllText(PATH_TO_IMPORT + "parts.json");

            var deserializedParts = JsonConvert.DeserializeObject<Part[]>(jsonString);

            var random = new Random();
            var suppliersCount = this.context.Suppliers.Count();

            foreach (var part in deserializedParts)
            {
                part.Supplier_Id = random.Next(1, suppliersCount);
            }

            this.context.Parts.AddRange(deserializedParts);
            this.context.SaveChanges();
        }

        private void ImportCars()
        {
            var jsonString = File.ReadAllText(PATH_TO_IMPORT + "cars.json");

            var deserializedCars = JsonConvert.DeserializeObject<Car[]>(jsonString);

            this.context.Cars.AddRange(deserializedCars);
            this.context.SaveChanges();
        }

        private void GenerateAndImportCarParts()
        {
            var partsCars = new List<PartCar>();

            var random = new Random();
            var allPartsCount = this.context.Parts.Count();
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

            this.context.PartCars.AddRange(partsCars);
            this.context.SaveChanges();
        }

        private void ImportCustomers()
        {
            var jsonString = File.ReadAllText(PATH_TO_IMPORT + "customers.json");

            var deserializedCustomers = JsonConvert.DeserializeObject<Customer[]>(jsonString);

            this.context.Customers.AddRange(deserializedCustomers);
            this.context.SaveChanges();
        }

        private void GenerateAndImportSales()
        {
            var discounts = new decimal[] { 0, 0.05M, 0.1M, 0.15M, 0.2M, 0.3M, 0.4M, 0.5M };
            var carsCount = this.context.Cars.Count();
            var customersCount = this.context.Customers.Count();

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

        private void ExportOrderedCustomers()
        {
            var customerDTOs = this.context.Customers
                .OrderBy(c => c.BirthDate)
                .ThenBy(c => c.IsYoungDriver)
                .Select(c => new CustomerDTO
                {
                    Id = c.Id,
                    Name = c.Name,
                    BirthDate = c.BirthDate,
                    IsYoungDriver = c.IsYoungDriver,
                    SalesCount = c.Sales.Count
                })
                .ToArray();

            var jsonSerializerSettings = this.GetDefaultNullValueHandling();

            var jsonString = JsonConvert.SerializeObject(customerDTOs, Formatting.Indented, jsonSerializerSettings);

            File.WriteAllText(PATH_TO_EXPORT + "ordered-customers.json", jsonString);
        }

        private void ExportCarsFromMakeToyota()
        {
            var carDTOs = this.context.Cars
                .Where(c => c.Make.Equals("Toyota"))
                .Select(c => new CarDTO
                {
                    Id = c.Id,
                    Make = c.Make,
                    Model = c.Model,
                    TravelledDistance = c.TravelledDistance
                })
                .ToArray()
                .OrderBy(c => c.Model)
                .ThenByDescending(c => c.TravelledDistance)
                .ToArray();

            var jsonSerializerSettings = this.GetDefaultNullValueHandling();

            var jsonString = JsonConvert.SerializeObject(carDTOs, Formatting.Indented, jsonSerializerSettings);

            File.WriteAllText(PATH_TO_EXPORT + "toyota-cars.json", jsonString);
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

            var jsonSerializerSettings = this.GetDefaultNullValueHandling();

            var jsonString = JsonConvert.SerializeObject(localSupplierDTOs, Formatting.Indented, jsonSerializerSettings);

            File.WriteAllText(PATH_TO_EXPORT + "local-suppliers.json", jsonString);
        }

        private void ExportCarsWithTheirParts()
        {
            var carPartsDTOs = this.context.Cars
                .Select(c => new
                {
                    car = new CarPartsDTO
                    {
                        Make = c.Make,
                        Model = c.Model,
                        TravelledDistance = c.TravelledDistance,
                        CarParts = c.PartCars.Select(pc => new PartDTO
                        {
                            Name = pc.Part.Name,
                            Price = pc.Part.Price
                        })
                        .ToArray()
                    }
                })
                .ToArray();

            var jsonSerializerSettings = this.GetDefaultNullValueHandling();

            var jsonString = JsonConvert.SerializeObject(carPartsDTOs, Formatting.Indented, jsonSerializerSettings);

            File.WriteAllText(PATH_TO_EXPORT + "cars-and-parts.json", jsonString);
        }

        private void ExportTotalSalesByCustomer()
        {
            var customerTotalSalesDTOs = this.context.Customers
                .Where(c => c.Sales.Count > 0)
                .Select(c => new CustomerTotalSalesDTO
                {
                    Name = c.Name,
                    BoughtCars = c.Sales.Count,
                    SpentMoney = Math.Round(
                        c.Sales
                        .Select(s => s.Car.PartCars.Sum(cp => cp.Part.Price) * (1 + s.Discount))
                        .DefaultIfEmpty(0)
                        .Sum(), 2)
                })
                .OrderByDescending(c => c.SpentMoney)
                .ThenByDescending(c => c.BoughtCars)
                .ToArray();

            var jsonSerializerSettings = this.GetDefaultNullValueHandling();

            var jsonString = JsonConvert.SerializeObject(customerTotalSalesDTOs, Formatting.Indented, jsonSerializerSettings);

            File.WriteAllText(PATH_TO_EXPORT + "customers-total-sales.json", jsonString);
        }

        private void SalesWithAppliedDiscount()
        {
            var saleDiscountsDTOs = this.context.Sales
                .Select(s => new SaleDiscountsDTO
                {
                    Car = new CarDTO
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

            var jsonSerializerSettings = this.GetDefaultNullValueHandling();

            var jsonString = JsonConvert.SerializeObject(saleDiscountsDTOs, Formatting.Indented, jsonSerializerSettings);

            File.WriteAllText(PATH_TO_EXPORT + "sales-discounts.json", jsonString);
        }

        private JsonSerializerSettings GetDefaultNullValueHandling()
        {
            var settings = new JsonSerializerSettings
            {
                DefaultValueHandling = DefaultValueHandling.Ignore
            };

            return settings;
        }
    }
}