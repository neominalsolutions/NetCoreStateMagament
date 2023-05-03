var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddSession(); // servis olarak session ekledik.
builder.Services.AddMemoryCache(); // InMemory Cache 
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
  app.UseExceptionHandler("/Home/Error");
  // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
  app.UseHsts();
}


// session middleware aktif hale getirdik.
app.UseSession();
//app.UseResponseCaching();
// Cookie desteklemesi için UseCookiePolicies() diye bir ara yazýlým var. // cookie ile çalýþmamýzý saðlayan servis
app.UseCookiePolicy();
app.UseHttpsRedirection();
app.UseStaticFiles();


app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
