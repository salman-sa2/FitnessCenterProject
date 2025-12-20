using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using FitnessCenterProject.Data;

public static class AdminSeeder
{
    public static async Task SeedAdminAsync(IServiceProvider services)
    {
        using var scope = services.CreateScope();

        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

        const string adminRole = "Admin";
        var adminEmail = "g211210574@sakarya.edu.tr";
        var adminPassword = "Sau@12345"; // ✅ güçlü şifre

        // Role yoksa oluştur
        if (!await roleManager.RoleExistsAsync(adminRole))
            await roleManager.CreateAsync(new IdentityRole(adminRole));

        // Kullanıcı var mı?
        var adminUser = await userManager.FindByEmailAsync(adminEmail);

        if (adminUser == null)
        {
            adminUser = new ApplicationUser
            {
                UserName = adminEmail,
                Email = adminEmail,
                EmailConfirmed = true // ✅ RequireConfirmedAccount true olduğu için
            };

            var createResult = await userManager.CreateAsync(adminUser, adminPassword);
            if (!createResult.Succeeded)
                throw new Exception("Admin create failed: " +
                    string.Join(" | ", createResult.Errors.Select(e => e.Description)));
        }
        else
        {
            // varsa: EmailConfirmed true yap + şifreyi resetle (sen yeni şifre verdin diye)
            adminUser.EmailConfirmed = true;
            await userManager.UpdateAsync(adminUser);

            var token = await userManager.GeneratePasswordResetTokenAsync(adminUser);
            var resetResult = await userManager.ResetPasswordAsync(adminUser, token, adminPassword);

            if (!resetResult.Succeeded)
                throw new Exception("Admin password reset failed: " +
                    string.Join(" | ", resetResult.Errors.Select(e => e.Description)));
        }

        // Role ata
        if (!await userManager.IsInRoleAsync(adminUser, adminRole))
            await userManager.AddToRoleAsync(adminUser, adminRole);
    }
}
