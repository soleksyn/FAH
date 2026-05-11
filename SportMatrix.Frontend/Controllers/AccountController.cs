using SportMatrix.Frontend.Models.Auth;
using SportMatrix.Frontend.Models.ApiClient;
using SportMatrix.Frontend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace SportMatrix.Frontend.Controllers;

public class AccountController : Controller
{
    private readonly SignInManager<IdentityUser> _signInManager;
    private readonly UserManager<IdentityUser>   _userManager;
    private readonly AthleteApiService           _athleteService;

    public AccountController(
        SignInManager<IdentityUser> signInManager,
        UserManager<IdentityUser>   userManager,
        AthleteApiService           athleteService)
    {
        _signInManager  = signInManager;
        _userManager    = userManager;
        _athleteService = athleteService;
    }

    // ── Login ──────────────────────────────────────────────────────────────
    [HttpGet]
    [AllowAnonymous]
    public IActionResult Login(string? returnUrl = null)
    {
        if (User.Identity?.IsAuthenticated == true)
            return RedirectToLocal(returnUrl);

        return View(new LoginViewModel { ReturnUrl = returnUrl });
    }

    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var result = await _signInManager.PasswordSignInAsync(
            model.Email,
            model.Password,
            model.RememberMe,
            lockoutOnFailure: false);

        if (result.Succeeded)
            return RedirectToLocal(model.ReturnUrl);

        ModelState.AddModelError(string.Empty, "Invalid email or password.");
        return View(model);
    }

    // ── Register ───────────────────────────────────────────────────────────
    [HttpGet]
    [AllowAnonymous]
    public IActionResult Register()
    {
        if (User.Identity?.IsAuthenticated == true)
            return RedirectToAction("Index", "Home");

        return View(new RegisterViewModel());
    }

    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var user   = new IdentityUser { UserName = model.Email, Email = model.Email, EmailConfirmed = true };
        var result = await _userManager.CreateAsync(user, model.Password);

        if (result.Succeeded)
        {
            await _userManager.AddToRoleAsync(user, "User");

            try
            {
                await _athleteService.CreateAthleteAsync(new CreateAthleteRequest
                {
                    FirstName = "New",
                    LastName = "Member",
                    Email = model.Email,
                    DateOfBirth = DateTime.Now.AddYears(-20),
                    Weight = 70,
                    Height = 175
                });
            }
            catch
            {
                // Profile creation failed, but user was created. 
                // We might want to log this or handle it.
            }

            await _signInManager.SignInAsync(user, isPersistent: false);
            return RedirectToAction("Index", "Home");
        }

        foreach (var error in result.Errors)
            ModelState.AddModelError(string.Empty, error.Description);

        return View(model);
    }

    // ── Logout ─────────────────────────────────────────────────────────────
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return RedirectToAction("Login");
    }

    // ── Access Denied ──────────────────────────────────────────────────────
    [AllowAnonymous]
    public IActionResult AccessDenied()
    {
        return View();
    }

    // ── Helpers ────────────────────────────────────────────────────────────
    private IActionResult RedirectToLocal(string? returnUrl)
    {
        if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            return Redirect(returnUrl);

        if (User.IsInRole("Admin"))
            return RedirectToAction("Index", "Athletes");

        // Regular users go to their own Dashboard
        return RedirectToAction("Index", "Dashboard");
    }
}
