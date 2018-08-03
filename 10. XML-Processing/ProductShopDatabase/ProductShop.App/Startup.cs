namespace ProductShop.App
{
    public class Startup
    {
        public static void Main()
        {
            var xmlProcessor = new XmlProcessor();

            xmlProcessor.ImportData();

            xmlProcessor.ExportData();
        }
    }
}
