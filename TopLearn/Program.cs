using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using TopLearn.Core.Convertors;
using TopLearn.Core.Services;
using TopLearn.Core.Services.Interfaces;
using TopLearn.DataLayer.Context;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();


#region DBContext
builder.Services.AddDbContext<TopLearnContext>(options =>
options.UseSqlServer("Integrated Security=SSPI;Persist Security Info=False;Initial Catalog=TopLearn;Data Source=."));
#endregion

#region IOC
builder.Services.AddTransient<IUserInterface, UserService>();
builder.Services.AddTransient<IViewRenderService, RenderViewToString>();
#endregion

#region Auth

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(option =>
    {
        option.LoginPath = "/Login";
        option.LogoutPath = "/Logout";
        option.ExpireTimeSpan = TimeSpan.FromDays(2);
    });
#endregion

var app = builder.Build();



// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseStaticFiles();

app.UseRouting();




app.UseAuthorization();



app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllerRoute(
      name: "areas",
      pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}"
    );
});
app.Run();
