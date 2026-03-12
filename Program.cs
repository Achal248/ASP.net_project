var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// ✅ Add Session Service
builder.Services.AddSession();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// ✅ Enable Session Middleware
app.UseSession();

app.UseAuthorization();


// ✅ SplashScreen route (FIRST PAGE)
app.MapControllerRoute(
name: "splash",
pattern: "{controller=SplashScreen}/{action=SplashScreen}/{id?}");


// ✅ Your existing route (UNCHANGED)
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Register}/{id?}");

app.Run();