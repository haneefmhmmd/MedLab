using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Configuration;
using Microsoft.OpenApi.Models;
using System.Text;
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

            // Access the secret key and other JWT settings from appsettings.json
            var jwtSettings = builder.Configuration.GetSection("JwtSettings");
            var secretKey = jwtSettings.GetValue<string>("SecretKey");

            // Add authentication services
            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = jwtSettings.GetValue<string>("Issuer"),
                        ValidAudience = jwtSettings.GetValue<string>("Audience"),
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)) // Use the secret key from appsettings
                    };
                });

            // Add services to the container
            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();

            // Add Swagger services
            builder.Services.AddSwaggerGen(c =>
            {
                // Define the BearerAuth security scheme
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "Enter 'Bearer' followed by your token."
                });

                // Add security requirement
                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        new string[] {}
                    }
                });
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthentication(); // Enable authentication middleware
            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
