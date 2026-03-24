<<<<<<< HEAD
﻿var builder = WebApplication.CreateBuilder(args);
=======
using ColorFill.Data;

var builder = WebApplication.CreateBuilder(args);
>>>>>>> main

// Add services to the container.
builder.Services.AddControllersWithViews();

<<<<<<< HEAD
// ✅ Add Session Service
=======
builder.Services.AddScoped<DbHelper>();
>>>>>>> main
builder.Services.AddSession();

var app = builder.Build();

<<<<<<< HEAD
=======
app.UseSession();

>>>>>>> main
// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
<<<<<<< HEAD
    app.UseHsts();
}

app.UseHttpsRedirection();
=======
}
>>>>>>> main
app.UseStaticFiles();

app.UseRouting();

<<<<<<< HEAD
// ✅ Enable Session Middleware (must come after UseRouting and before UseAuthorization)
app.UseSession();

=======
>>>>>>> main
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
<<<<<<< HEAD
    pattern: "{controller=Account}/{action=Register}/{id?}");

app.Run();
=======
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
>>>>>>> main
