using AutoMapper;
using DefaultProject.Mapping;
using DefaultProject.Models;
using DefaultProject.Persistence;
using DefaultProject.Persistence.Repositories.Implementation;
using DefaultProject.Persistence.Repositories.Interfaces;
using DefaultProject.Persistence.Repositories.UnitOfWork;
using DefaultProject.Security;
using DefaultProject.Security.Middlewares;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

    // Add services to the container.

// Database Connection Configuration
builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.LogTo(
        Console.WriteLine,
    new[] { DbLoggerCategory.Database.Command.Name },
        LogLevel.Information).EnableSensitiveDataLogging();
    var serverVersion = new MySqlServerVersion(new Version(8, 0, 26));
    options.UseMySql(builder.Configuration.GetConnectionString("DefaultConnection"), serverVersion);
    //options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

// Dependency Injection Configuration
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

builder.Services.AddControllers(options =>
{
    var policy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build();
    var filter = new AuthorizeFilter(policy);
    options.Filters.Add(filter);
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(config =>
{
    config.SwaggerDoc("v1", new OpenApiInfo { Title = "Default.API", Version = "v1" });

    var securityScheme = new OpenApiSecurityScheme
    {
        Name = "Token",
        Description = "JWT Authorization header using the Bearer scheme.",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT"
    };
    config.AddSecurityDefinition("Bearer", securityScheme);
    config.AddSecurityRequirement(new OpenApiSecurityRequirement
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
            new string[] { }
        }
    });
});

// mapper config
builder.Services.AddAutoMapper(typeof(Program));

// jwt security config
var security = builder.Services.AddIdentityCore<Usuario>();
var identity = new IdentityBuilder(security.UserType, builder.Services);

identity.AddEntityFrameworkStores<AppDbContext>();
identity.AddSignInManager<SignInManager<Usuario>>();

builder.Services.AddSingleton<ISystemClock, SystemClock>();
builder.Services.AddScoped<IJwtGenerator, JwtGenerator>();
builder.Services.AddScoped<IUsuarioSesion, UsuarioSesion>();
builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();

var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("0E1A53F4056B4D8B8C6B3D6A0E5B4F9E53C6A87A6B85D7F956A9F5C8F4D68B2EA"));
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = key,
            ValidateAudience = false,
            ValidateIssuer = false,
        };
    });

// cors config
builder.Services.AddCors(options =>
{
    options.AddPolicy("MyPolicy", options =>
    {
        options.WithOrigins("*").AllowAnyMethod().AllowAnyHeader();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// middleware config
app.UseMiddleware<ManagerMiddleware>();

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();
app.UseCors("MyPolicy");


app.MapControllers();

//
using (var ambiente = app.Services.CreateScope())
{
    var services = ambiente.ServiceProvider;
    try
    {
        var userManager = services.GetRequiredService<UserManager<Usuario>>();
        var context = services.GetRequiredService<AppDbContext>();
        await context.Database.MigrateAsync();
        await LoadDataSeed.InsertData(context, userManager);
    }
    catch (Exception e)
    {
        var logging = services.GetRequiredService<ILogger<Program>>();
        logging.LogError(e, "Ocurrio un error en la migración.");
    }
}

app.Run();
