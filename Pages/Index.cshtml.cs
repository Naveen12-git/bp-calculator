using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using BPCalculator;

namespace BPCalculator.Pages
{
    public class BloodPressureModel : PageModel
    {
        private readonly ILogger<BloodPressureModel> _logger;

        public BloodPressureModel(ILogger<BloodPressureModel> logger)
        {
            _logger = logger;
        }

        [BindProperty]
        public BloodPressure BP { get; set; }

        public string Category { get; set; }

        public void OnGet()
        {
            BP = new BloodPressure() { Systolic = 100, Diastolic = 60 };
            Category = null;
            
            if (TempData["CategoryResult"] is string category)
            {
                Category = category;
                TempData.Keep("CategoryResult");
            }
            
            if (TempData["ErrorMessage"] is string error)
            {
                ModelState.AddModelError("", error);
                TempData.Keep("ErrorMessage");
            }
        }

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
            {
                return RedirectToPage();
            }

            if (!(BP.Systolic > BP.Diastolic))
            {
                TempData["ErrorMessage"] = "Systolic must be greater than Diastolic";
                return RedirectToPage();
            }

            Category = BP.Category.ToString();
            TempData["CategoryResult"] = Category;

            _logger.LogInformation(
                "Blood Pressure reading {Systolic}/{Diastolic} categorized as {Category}",
                BP.Systolic, BP.Diastolic, Category
            );

            return RedirectToPage();
        }
    }
}
