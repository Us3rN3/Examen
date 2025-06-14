using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Reflection;
using System.Text;
using VivesRental.Data;
using VivesRental.Domains.EntitiesDB;
using VivesRental.Repositories;
using VivesRental.Repositories.Interfaces;
using VivesRental.Services;
using VivesRental.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Connection string
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

// Add DbContexts
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddDbContext<VivesRental.Domains.DataDB.RentalDbContext>(options =>
    options.UseSqlServer(connectionString));

// Identity setup
builder.Services.AddDefaultIdentity<IdentityUser>(options =>
{
    options.SignIn.RequireConfirmedAccount = true;
    // Configure password complexity etc. here if needed
})
    .AddEntityFrameworkStores<ApplicationDbContext>();

// JWT configuration from appsettings
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var secretKey = jwtSettings.GetValue<string>("SecretKey");
var issuer = jwtSettings.GetValue<string>("Issuer");
var audience = jwtSettings.GetValue<string>("Audience");

var key = Encoding.ASCII.GetBytes(secretKey);

// Add Authentication with JWT Bearer tokens
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidIssuer = issuer,
        ValidateAudience = true,
        ValidAudience = audience,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(secretKey)
        )
    };
});



// Register your application services and repositories
builder.Services.AddTransient<IService<Order>, OrderService>();
builder.Services.AddTransient<IDAO<Order>, OrderDAO>();

builder.Services.AddTransient<IService<Product>, ProductService>();
builder.Services.AddTransient<IDAO<Product>, ProductDAO>();

builder.Services.AddTransient<IService<Article>, ArticleService>();
builder.Services.AddTransient<IDAO<Article>, ArticleDAO>();

builder.Services.AddTransient<IService<ArticleReservation>, ArticleReservationService>();
builder.Services.AddTransient<IDAO<ArticleReservation>, ArticleReservationDAO>();

builder.Services.AddTransient<IService<Customer>, CustomerService>();
builder.Services.AddTransient<IDAO<Customer>, CustomerDAO>();

builder.Services.AddTransient<IService<OrderLine>, OrderLineService>();
builder.Services.AddTransient<IDAO<OrderLine>, OrderLineDAO>();

builder.Services.AddTransient<IUserService, UserService>();

// Controllers and MVC
builder.Services.AddControllers();
builder.Services.AddControllersWithViews();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "VivesRental API", Version = "v1" });

    // JWT Authorization in Swagger
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Voer hier je JWT in als: Bearer {token}"
    });

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

    // XML-comments (optioneel)
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    c.IncludeXmlComments(xmlPath);
});


// AutoMapper
builder.Services.AddAutoMapper(typeof(Program));

var app = builder.Build();

// Middleware pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();  // ** This must come before UseAuthorization()
app.UseAuthorization();

app.MapControllers();
app.MapDefaultControllerRoute();
app.MapRazorPages();

app.Run();
