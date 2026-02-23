using Acadmart.Data;
using Acadmart.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// 1️⃣ Database
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// 2️⃣ Identity
builder.Services.AddDefaultIdentity<IdentityUser>()
    .AddEntityFrameworkStores<ApplicationDbContext>();

// 3️⃣ MVC
builder.Services.AddControllersWithViews();

// 4️⃣ Session for Cart
builder.Services.AddSession();

var app = builder.Build();

// ✅ Middleware
// app.UseHttpsRedirection(); // <-- comment out for now
app.UseStaticFiles();
app.UseRouting();
app.UseSession();
app.UseAuthorization();

// ✅ Routing
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

// 5️⃣ Seed dummy categories and products
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

    // ===== Seed Categories =====
    if (!db.Categories.Any())
    {
        db.Categories.AddRange(
            new Category { Name = "Apparel" },
            new Category { Name = "Accessories" },
            new Category { Name = "Books" }
        );
        db.SaveChanges();
    }

    // ===== Seed Products =====
    if (!db.Products.Any())
    {
        var categories = db.Categories.ToList();

        db.Products.AddRange(
            new Product { Name = "College Hoodie", Price = 500, Stock = 10, ImageURL = "/images/hoodie.jpg", Description = "Warm hoodie", Vendor_email = "seller1@college.edu", CategoryID = categories[0].CategoryID },

            new Product { Name = "College Mug", Price = 300, Stock = 20, ImageURL = "/images/mug.jpg", Description = "Ceramic mug", Vendor_email = "seller2@college.edu", CategoryID = categories[1].CategoryID },

            new Product { Name = "Notebook", Price = 150, Stock = 15, ImageURL = "/images/notebook.jpg", Description = "College notebook", Vendor_email = "seller1@college.edu", CategoryID = categories[2].CategoryID },

            new Product { Name = "T-Shirt", Price = 450, Stock = 25, ImageURL = "/images/tshirt.jpg", Description = "Cotton T-Shirt", Vendor_email = "seller3@college.edu", CategoryID = categories[0].CategoryID }
        );

        db.SaveChanges();
    }
}

app.Run();