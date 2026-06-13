using Microsoft.AspNetCore.Authentication.Cookies;
using Webbanhang_TH02.Repositories;

var builder = WebApplication.CreateBuilder(args);

// 1. Đăng ký dịch vụ hiển thị Giao diện MVC và Razor Pages
builder.Services.AddControllersWithViews()
                .AddRazorRuntimeCompilation();

builder.Services.AddRazorPages();

// Đăng ký dịch vụ Authentication & Cookie
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.LogoutPath = "/Account/Logout";
        options.AccessDeniedPath = "/Account/AccessDenied";
    });

// Đăng ký dịch vụ Session
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// 2. Đăng ký các dịch vụ Repository xử lý dữ liệu (Dùng Singleton để giữ dữ liệu in-memory)
builder.Services.AddSingleton<IProductRepository, MockProductRepository>();
builder.Services.AddSingleton<ICategoryRepository, MockCategoryRepository>();
builder.Services.AddSingleton<IUserRepository, MockUserRepository>();
builder.Services.AddSingleton<IOrderRepository, MockOrderRepository>();

var app = builder.Build();

// 3. Cấu hình Pipeline xử lý các yêu cầu HTTP (Middleware)
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// Sử dụng Session và Authentication trước Authorization
app.UseSession();
app.UseAuthentication();
app.UseAuthorization();

// 4. Định tuyến mặc định (Chuyển trang chủ chạy thẳng vào Product/Index)
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Product}/{action=Index}/{id?}");

app.MapRazorPages();

// QUAN TRỌNG: Dòng này phải viết đúng cú pháp này để giữ Server hoạt động liên tục, không tự ngắt
app.Run();