using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using SoftJail;
using SoftJail.Data;
using SoftJail.Data.Models;
using SoftJail.DataProcessor;

[TestFixture]
public class Export_000_001
{
    private IServiceProvider serviceProvider;

    private const string jsonOutputData = "[{\"Id\":2,\"Name\":\"Diana Express\",\"CellNumber\":666,\"Officers\":[{\"OfficerName\":\"Chiba KenChi Mariko\",\"Department\":\"DogStyle\"},{\"OfficerName\":\"Papia Fernandez\",\"Department\":\"IndexOutOfBound\"}],\"TotalOfficerSalary\":0.0},{\"Id\":4,\"Name\":\"Jenny Rhys\",\"CellNumber\":666,\"Officers\":[{\"OfficerName\":\"Paddy Weiner\",\"Department\":\"Jail\"}],\"TotalOfficerSalary\":0.0},{\"Id\":1,\"Name\":\"Kojo Mojo\",\"CellNumber\":303,\"Officers\":[{\"OfficerName\":\"First Officer\",\"Department\":\"Java\"}],\"TotalOfficerSalary\":0.0},{\"Id\":3,\"Name\":\"Vanila Ice\",\"CellNumber\":404,\"Officers\":[{\"OfficerName\":\"Ban Ban\",\"Department\":\"Jail\"}],\"TotalOfficerSalary\":0.0}]";

    private const string jsonSeedData =
        "[{'Id':2,'Name':'Diana Express','CellNumber':666,'Officers':[{'FullName':'Chiba KenChi Mariko','Department':'DogStyle'},{'FullName':'Papia Fernandez','Department':'IndexOutOfBound'}]},{'Id':1,'Name':'Kojo Mojo','CellNumber':303,'Officers':[{'FullName':'First Officer','Department':'Java'}]},{'Id':3,'Name':'Vanila Ice','CellNumber':404,'Officers':[{'FullName':'Ban Ban','Department':'Jail'}]},{'Id':4,'Name':'Jenny Rhys','CellNumber':666,'Officers':[{'FullName':'Paddy Weiner','Department':'Jail'}]}]";

    private static readonly Assembly CurrentAssembly = typeof(StartUp).Assembly;

    [SetUp]
    public void Setup()
    {
        Mapper.Reset();
        Mapper.Initialize(cfg => cfg.AddProfile(GetType("SoftJailProfile")));

        this.serviceProvider = ConfigureServices<SoftJailDbContext>("SoftJail");
    }

    [Test]
    public void ExportPrisonersByCellsZeroTest1()
    {
        var context = serviceProvider.GetService<SoftJailDbContext>();

        SeedDatabase(context);

        var expectedOutput = JToken.Parse(jsonOutputData);

        var actualOutput =
            JToken.Parse(Serializer.ExportPrisonersByCells(context, new[] { 1, 2, 3, 4 }));

        var expectedOutputJson = expectedOutput.ToString(Formatting.None);
        var actualOutputJson = actualOutput.ToString(Formatting.None);

        Assert.That(actualOutputJson, Is.EqualTo(expectedOutputJson).NoClip,
            $"{nameof(Serializer.ExportPrisonersByCells)} output is incorrect!");

        //var jsonsAreEqual = JToken.DeepEquals(expectedOutput, actualOutput);
        //Assert.That(jsonsAreEqual, Is.True, "ExportPrisonersByCells output is incorrect!");
    }

    private static void SeedDatabase(SoftJailDbContext context)
    {

        var definition = new[] {  new {
                Id = 0,
                Name = "",
                CellNumber = 0,
                Officers = new[]
                {
                    new
                    {
                        FullName = "",
                        Department = ""
                    }
                }
            }
        };

        var dataSet = JsonConvert.DeserializeAnonymousType(jsonSeedData, definition);

        var prisoner = dataSet.Select(x => new Prisoner()
        {
            Id = x.Id,
            FullName = x.Name,
            Cell = new Cell()
            {
                CellNumber = x.CellNumber
            },
            PrisonerOfficers = new List<OfficerPrisoner>(x.Officers.Select(o => new OfficerPrisoner()
            {
                Officer = new Officer()
                {
                    FullName = o.FullName,
                    Department = new Department()
                    {
                        Name = o.Department
                    }
                }
            }).ToList())


        }).ToArray();
        context.Prisoners.AddRange(prisoner);
        context.SaveChanges();

    }

    private static IServiceProvider ConfigureServices<TContext>(string databaseName)
        where TContext : DbContext
    {
        var services = ConfigureDbContext<TContext>(databaseName);

        var context = services.GetService<TContext>();

        try
        {
            context.Model.GetEntityTypes();
        }
        catch (InvalidOperationException ex) when (ex.Source == "Microsoft.EntityFrameworkCore.Proxies")
        {
            services = ConfigureDbContext<TContext>(databaseName, useLazyLoading: true);
        }

        return services;
    }

    private static IServiceProvider ConfigureDbContext<TContext>(string databaseName, bool useLazyLoading = false)
        where TContext : DbContext
    {
        var services = new ServiceCollection();

        services
            .AddDbContext<TContext>(
                options => options
                    .UseInMemoryDatabase(databaseName)
                    .UseLazyLoadingProxies(useLazyLoading)
            );

        var serviceProvider = services.BuildServiceProvider();
        return serviceProvider;
    }

    private static Type GetType(string modelName)
    {
        var modelType = CurrentAssembly
            .GetTypes()
            .FirstOrDefault(t => t.Name == modelName);

        Assert.IsNotNull(modelType, $"{modelName} model not found!");

        return modelType;
    }
}