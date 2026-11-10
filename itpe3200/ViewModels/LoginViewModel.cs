using System.ComponentModel.DataAnnotations;

namespace itpe3200.ViewModels;

public class LoginViewModel
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    [DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;

    [Display(Name = "Remember me?")]
    public bool RememberMe { get; set; }

    // Where to send the user after login (set when they were redirected to the login page)
    public string? ReturnUrl { get; set; }
}