
using MapCompereAPI.Connectors;
using MapCompereAPI.Repositories;
using NLog.Web;

namespace MapCompereAPI
{
	public class Program
	{
		public static void Main(string[] args)
		{
			var builder = WebApplication.CreateBuilder(args);

			// Add services to the container.

			builder.Services.AddControllers();
			// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
			builder.Services.AddEndpointsApiExplorer();
			builder.Services.AddSwaggerGen();
			builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

			builder.Services.AddSingleton<IDocumentDatabase, DataBaseMongo>();
			builder.Services.AddSingleton<IMapService, MapService>();
			builder.Services.AddScoped<IScrapperConnector, ScrapperConnector>();

			// Configure NLog
			builder.Logging.ClearProviders();
			builder.Host.UseNLog();

			var app = builder.Build();

			// Configure the HTTP request pipeline.
			if (app.Environment.IsDevelopment())
			{
				app.UseSwagger();
				app.UseSwaggerUI();
			}

			app.UseHttpsRedirection();

			app.UseAuthorization();


			app.MapControllers();

			app.Run();
		}
	}
}
