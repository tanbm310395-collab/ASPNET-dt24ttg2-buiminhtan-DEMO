using Microsoft.EntityFrameworkCore;
using money_management.Models;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddRazorPages();
builder.Services.AddSession();



var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    try
    {
        db.Database.CanConnect(); // ✔️ Kiểm tra kết nối ở đây là hợp lý
        Console.WriteLine("✅ Đã kết nối với database thành công!");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"❌ Lỗi kết nối database: {ex.Message}");
    }
}

// Middleware và chạy ứng dụng
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapRazorPages();
app.UseSession();
app.UseStaticFiles();
app.Run();
