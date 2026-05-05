using SportMatrix.Frontend.Models.ApiClient;
using SportMatrix.Frontend.Services;
using SportMatrix.Frontend.Models;
using Microsoft.AspNetCore.Mvc;

namespace SportMatrix.Frontend.Controllers;

public class AthletesController : Controller
{
    private readonly AthleteApiService _athleteService;

    public AthletesController(AthleteApiService athleteService)
    {
        _athleteService = athleteService;
    }

    // GET: Athletes
    public async Task<IActionResult> Index()
    {
        var (athletes, error) = await LoadAthletesAsync();
        var viewModel = CreateAthleteListViewModel(athletes, error);
        return View(viewModel);
    }

    private async Task<(List<AthleteDto> athletes, string? error)> LoadAthletesAsync()
    {
        try
        {
            var athletes = await _athleteService.GetAllAthletesAsync();
            return (athletes, null);
        }
        catch (Exception ex)
        {
            return (new List<AthleteDto>(), $"Error loading athletes: {ex.Message}");
        }
    }

    private AthleteListViewModel CreateAthleteListViewModel(List<AthleteDto> athletes, string? error)
    {
        return new AthleteListViewModel
        {
            Athletes = athletes,
            Error = error,
            Loading = false
        };
    }

    // GET: Athletes/Details/5
    public async Task<IActionResult> Details(int id)
    {
        var viewModel = new AthleteDetailViewModel { Loading = true };
        try
        {
            viewModel.Athlete = await _athleteService.GetAthleteByIdAsync(id);
            if (viewModel.Athlete == null)
            {
                viewModel.Error = "Athlete not found.";
            }
        }
        catch (Exception ex)
        {
            viewModel.Error = $"Error loading athlete: {ex.Message}";
        }
        finally
        {
            viewModel.Loading = false;
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
        {
            return View(model);
        }

        model.Submitting = true;
        try
        {
            var request = new CreateAthleteRequest
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                Email = model.Email,
                DateOfBirth = model.DateOfBirth,
                Weight = model.Weight,
                Height = model.Height
            };

            var created = await _athleteService.CreateAthleteAsync(request);
            if (created == null)
            {
                model.Error = "Error creating athlete.";
                return View(model);
            }
            return RedirectToAction(nameof(Details), new { id = created.Id });
        }
        catch (Exception ex)
        {
            model.Error = $"Error creating athlete: {ex.Message}";
            return View(model);
        }
        finally
        {
            model.Submitting = false;
        }
    }

    // GET: Athletes/Edit/5
    public async Task<IActionResult> Edit(int id)
    {
        var viewModel = new AthleteEditViewModel { Loading = true };
        try
        {
            var athlete = await _athleteService.GetAthleteByIdAsync(id);
            if (athlete == null)
            {
                viewModel.Error = "Athlete not found.";
                return View(viewModel);
            }

            viewModel.Id = athlete.Id;
            viewModel.FirstName = athlete.FirstName;
            viewModel.LastName = athlete.LastName;
            viewModel.Email = athlete.Email;
            viewModel.DateOfBirth = athlete.DateOfBirth;
            viewModel.Weight = athlete.Weight;
            viewModel.Height = athlete.Height;
        }
        catch (Exception ex)
        {
            viewModel.Error = $"Fehlee beimeLeie  deseAee desneAer desneAe  desnaAen desn Athleten: {ex.Message}";
        }
        finally
        {
            viewModel.Loading = false;
        }
        return View(viewModel);
    }

    // POST: Athletes/Edit
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(AthleteEditViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        model.Submitting = true;
        try
        {
            var request = new UpdateAthleteRequest
            {
                Id = model.Id,
                FirstName = model.FirstName,
                LastName = model.LastName,
                Email = model.Email,
                DateOfBirth = model.DateOfBirth,
                Weight = model.Weight,
                Height = model.Height
            };

            await _athleteService.UpdateAthleteAsync(request);
            return RedirectToAction(nameof(Details), new { id = model.Id });
        }
        catch (Exception ex)
        {
            model.Error = $"Error updating athlete: {ex.Message}";
            return View(model);
        }
        finally
        {
            model.Submitting = false;
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
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            // For delete from Index page, we need to handle error differently
            // We'll redirect back with error message via TempData
            TempData["Error"] = $"Error deleting: {ex.Message}";
            return RedirectToAction(nameof(Index));
        }
    }
}
