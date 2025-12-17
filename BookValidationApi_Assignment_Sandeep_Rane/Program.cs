using BookValidationApi_Assignment_Sandeep_Rane.Services;

namespace BookValidationApi_Assignment_Sandeep_Rane
{
    public class Program
    {
        public static void Main(string[] args)
        {

            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllers();
            
            var xmlPath = Path.Combine(builder.Environment.WebRootPath, "books.xml");
            builder.Services.AddSingleton(new XmlBookParser(xmlPath));

            var app = builder.Build();

            app.UseStaticFiles();

            app.MapControllers();

            app.Run();
        }
    }
}
