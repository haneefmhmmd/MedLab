
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using medLab.Repositories;

namespace medLab
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Load AWS configuration from appsettings.json
            var awsOptions = builder.Configuration.GetAWSOptions();
            builder.Services.AddDefaultAWSOptions(awsOptions);
            builder.Services.AddAWSService<IAmazonDynamoDB>();
            builder.Services.AddScoped<IDynamoDBContext, DynamoDBContext>();


            // Add AutoMapper
            builder.Services.AddAutoMapper(typeof(LabProfile)); // Registering the LabProfile for AutoMapper

            // Add the repository
            builder.Services.AddScoped<ILabRepository, LabRepository>();

            // Add services to the container.

            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

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
