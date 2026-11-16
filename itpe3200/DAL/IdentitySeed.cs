using Microsoft.AspNetCore.Identity;

namespace itpe3200;

public static class IdentitySeed
{
    public static async Task SeedAsync(WebApplication app)
    {
        //Create scope to access services such as db and Identity
        using var scope = app.Services.CreateScope();

        //Create a RoleManager
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        //Manage a UserManager
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
        
        string[] roles = { "Teacher", "Student" };
        foreach (var role in roles)
        {
            //If the role does not exists, create role
            if (!await roleManager.RoleExistsAsync(role))
            {
                var result = await roleManager.CreateAsync(new IdentityRole(role));
                if (!result.Succeeded)
                {
                    throw new InvalidOperationException($"Could not create role {role}");
                }
            }
        }
        if (app.Environment.IsDevelopment())
        {
            var teacherEmail = app.Configuration["DemoTeacherEmail"];
            //Ensures teacher email is not empty
            if (!string.IsNullOrWhiteSpace(teacherEmail))
            {
                //Tries to find teacher email in db
                var teacher = await userManager.FindByEmailAsync(teacherEmail);
                if (teacher != null)
                {
                    //The user is assigned Teacher role if the user does not already have it
                    if (!await userManager.IsInRoleAsync(teacher, "Teacher"))
                    {
                        var result = await userManager.AddToRoleAsync(teacher, "Teacher");

                        if (!result.Succeeded){
                            throw new InvalidOperationException("Could not assign Teacher role.");
                        }
                    }
                }
            }
        }
    }
}