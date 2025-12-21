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
        var adminPassword = "sau";

        // Role yoksa oluştur
        if (!await roleManager.RoleExistsAsync(adminRole))
            await roleManager.CreateAsync(new IdentityRole(adminRole));

        // Mevcut admin kullanıcıyı bul
        var adminUser = await userManager.FindByEmailAsync(adminEmail);

        if (adminUser == null)
        {
            // Kullanıcı yoksa yeni oluştur
            adminUser = new ApplicationUser
            {
                UserName = adminEmail,
                Email = adminEmail,
                EmailConfirmed = true
            };

            var createResult = await userManager.CreateAsync(adminUser, adminPassword);
            if (!createResult.Succeeded)
                throw new Exception("Admin create failed: " +
                    string.Join(" | ", createResult.Errors.Select(e => e.Description)));
        }
        else
        {
            // Kullanıcı varsa: EmailConfirmed true yap ve şifreyi güncelle
            adminUser.EmailConfirmed = true;
            await userManager.UpdateAsync(adminUser);

            // Şifreyi güncelle: RemovePassword + AddPassword yöntemi
            var hasPassword = await userManager.HasPasswordAsync(adminUser);
            if (hasPassword)
            {
                var removeResult = await userManager.RemovePasswordAsync(adminUser);
                if (!removeResult.Succeeded)
                    throw new Exception("Admin password remove failed: " +
                        string.Join(" | ", removeResult.Errors.Select(e => e.Description)));
            }

            var addResult = await userManager.AddPasswordAsync(adminUser, adminPassword);
            if (!addResult.Succeeded)
                throw new Exception("Admin password add failed: " +
                    string.Join(" | ", addResult.Errors.Select(e => e.Description)));
        }

        // Role ata
        if (!await userManager.IsInRoleAsync(adminUser, adminRole))
            await userManager.AddToRoleAsync(adminUser, adminRole);
    }
}
