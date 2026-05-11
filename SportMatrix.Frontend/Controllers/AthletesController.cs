using SportMatrix.Frontend.Models.ApiClient;
using SportMatrix.Frontend.Services;
using SportMatrix.Frontend.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace SportMatrix.Frontend.Controllers;

[Authorize(Roles = "Admin")]
public class AthletesController : Controller
{
    private readonly AthleteApiService _athleteService;

    public AthletesController(AthleteApiService athleteService)
    {
        _athleteService = athleteService;
    }

    // GET: Athletes
    public async Task<IActionResult> Index(int page = 1)
    {
        if (page < 1) page = 1;
        const int pageSize = 8;

        var (allAthletes, error) = await LoadAthletesAsync();

        // Clamp the current page so a stale bookmark after deletion never shows an empty page
        var totalAthletes = allAthletes.Count;
        var totalPages = (int)Math.Ceiling((double)totalAthletes / pageSize);
        if (page > 1 && page > totalPages) page = Math.Max(1, totalPages);

        var pagedAthletes = allAthletes
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        var viewModel = new AthleteListViewModel
        {
            Athletes  = pagedAthletes,
            Error     = error,
            CurrentPage   = page,
            PageSize      = pageSize,
            TotalAthletes = totalAthletes,
            Loading   = false
        };

        return View(viewModel);
    }

    private async Task<(List<AthleteDto> athletes, string? error)> LoadAthletesAsync()
    {
        try
        {
            var athletes = await _athleteService.GetAllAthletesAsync();
            return (athletes ?? new List<AthleteDto>(), null);
        }
        catch (Exception ex)
        {
            return (new List<AthleteDto>(), $"Error loading athletes: {ex.Message}");
        }
    }

    // GET: Athletes/Details/5
    public async Task<IActionResult> Details(int id)
    {
        var viewModel = new AthleteDetailViewModel();
        try
        {
            viewModel.Athlete = await _athleteService.GetAthleteByIdAsync(id);
            if (viewModel.Athlete == null)
                viewModel.Error = "Athlete not found.";
        }
        catch (Exception ex)
        {
            viewModel.Error = $"Error loading athlete: {ex.Message}";
        }
        return View(viewModel);
    }

    // GET: Athletes/Create
    public IActionResult Create()
    {
        return View(new AthleteCreateViewModel());
    }

    // POST: Athletes/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(AthleteCreateViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        try
        {
            var request = new CreateAthleteRequest
            {
                FirstName   = model.FirstName.Trim(),
                LastName    = model.LastName.Trim(),
                Email       = model.Email.Trim(),
                DateOfBirth = model.DateOfBirth!.Value,
                Weight      = model.Weight,
                Height      = model.Height
            };

            var created = await _athleteService.CreateAthleteAsync(request);
            if (created == null)
            {
                model.Error = "The server did not return the created athlete. Please try again.";
                return View(model);
            }

            TempData["Success"] = $"{created.FirstName} {created.LastName} was added successfully.";
            return RedirectToAction(nameof(Details), new { id = created.Id });
        }
        catch (Exception ex)
        {
            model.Error = $"Error creating athlete: {ex.Message}";
            return View(model);
        }
    }

    // GET: Athletes/Edit/5
    public async Task<IActionResult> Edit(int id)
    {
        AthleteEditViewModel viewModel;
        try
        {
            var athlete = await _athleteService.GetAthleteByIdAsync(id);
            if (athlete == null)
                return RedirectToAction(nameof(Index));

            viewModel = new AthleteEditViewModel
            {
                Id          = athlete.Id,
                FirstName   = athlete.FirstName,
                LastName    = athlete.LastName,
                Email       = athlete.Email,
                DateOfBirth = athlete.DateOfBirth,
                Weight      = athlete.Weight,
                Height      = athlete.Height
            };
        }
        catch (Exception ex)
        {
            viewModel = new AthleteEditViewModel
            {
                Error = $"Error loading athlete: {ex.Message}"
            };
        }
        return View(viewModel);
    }

    // POST: Athletes/Edit
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(AthleteEditViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        try
        {
            var request = new UpdateAthleteRequest
            {
                Id          = model.Id,
                FirstName   = model.FirstName.Trim(),
                LastName    = model.LastName.Trim(),
                Email       = model.Email.Trim(),
                DateOfBirth = model.DateOfBirth!.Value,
                Weight      = model.Weight,
                Height      = model.Height
            };

            await _athleteService.UpdateAthleteAsync(request);
            TempData["Success"] = $"{model.FirstName} {model.LastName} was updated successfully.";
            return RedirectToAction(nameof(Details), new { id = model.Id });
        }
        catch (Exception ex)
        {
            model.Error = $"Error updating athlete: {ex.Message}";
            return View(model);
        }
    }

    // POST: Athletes/Delete/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            await _athleteService.DeleteAthleteAsync(id);
            TempData["Success"] = "Athlete deleted successfully.";
        }
        catch (Exception ex)
        {
            TempData["Error"] = $"Could not delete athlete: {ex.Message}";
        }
        return RedirectToAction(nameof(Index));
    }
}
