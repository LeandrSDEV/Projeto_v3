using Microsoft.EntityFrameworkCore;
using Servidor_V3.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<BancoContext>(options =>
        options.UseMySql(builder.Configuration.GetConnectionString("DefaultConnection"), new MySqlServerVersion(new Version(3, 0, 38))));

builder.Services.AddScoped<ServidorService>();
builder.Services.AddScoped<CategoriaService>();
builder.Services.AddScoped<MatriculaService>();
builder.Services.AddScoped<SecretariaService>();
builder.Services.AddScoped<PerfilCalculo>();
builder.Services.AddScoped<CleanupService>();

var app = builder.Build();

app.UseDeveloperExceptionPage();
// if (!app.Environment.IsDevelopment())
// {
//     app.UseExceptionHandler("/Home/Error");
//     app.UseHsts();
// }

app.UseHttpsRedirection();
app.UseStaticFiles();



app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
