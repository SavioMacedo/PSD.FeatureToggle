using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Psd.FeatureToggle.CrossCutting.FeatureToggle.IoC;
using PSD.FeatureToggle.Entities;
using PSD.FeatureToggle.Features.CustomFilter;
using System.Text;

internal class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.

        builder.Services.AddControllers();
        builder.Services.AddAuthentication(authOptions =>
        {
            authOptions.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            authOptions.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer("ChapterDayScheme", configureOptions =>
        {
            configureOptions.RequireHttpsMetadata = false;
            configureOptions.SaveToken = true;
            configureOptions.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes("5621F303-290E-4798-9C92-C07B02995EA4")),
                ValidateIssuer = false,
                ValidateAudience = false
            };
        });

        builder.Services.AddRouting(options => options.LowercaseUrls = true);

        builder.Services.AddAuthorization(options =>
        {
            //options.AddPolicy("ChapterDayScheme", policy =>
            //{
            //    policy.AddRequirements(new ApiRequeriment());
            //});
        });

        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
        builder.Services.UseAwsFeatureManagement()
            .AddFeatureFilter<EscolaFeatureFilter>();
        builder.Services.AddScoped<IUsuarioService, UsuarioService>();
        builder.Services.AddScoped<ICardService, CardService>();
        builder.Services.AddHttpContextAccessor();

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