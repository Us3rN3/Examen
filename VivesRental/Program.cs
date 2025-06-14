using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using VivesRental.Data;
using VivesRental.Domains.EntitiesDB;
using VivesRental.Repositories;
using VivesRental.Repositories.Interfaces;
using VivesRental.Services;
using VivesRental.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<VivesRental.Domains.DataDB.RentalDbContext>(options =>
    options.UseSqlServer(connectionString));


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

builder.Services.AddControllers(); // Voor web API controllers
builder.Services.AddControllersWithViews(); // Voor MVC controllers en views

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    c.IncludeXmlComments(xmlPath);
});

builder.Services.AddAutoMapper(typeof(Program)); // zoekt automatisch alle Profile-klassen in je project

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.MapControllers(); // voor web API controllers
app.MapDefaultControllerRoute(); // <-- voor MVC

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.MapRazorPages()
   .WithStaticAssets();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.Run();
