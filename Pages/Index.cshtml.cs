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
            // Initialize with default values
            BP = new BloodPressure() { Systolic = 100, Diastolic = 60 };
            Category = null;
            
            // Check for result from redirect
            if (TempData["CategoryResult"] is string category)
            {
                Category = category;
                TempData.Keep("CategoryResult"); // Keep for next request
            }
            
            // Check for error from redirect
            if (TempData["ErrorMessage"] is string error)
            {
                ModelState.AddModelError("", error);
                TempData.Keep("ErrorMessage");
            }
        }

        public IActionResult OnPost()
        {
            // Check validation FIRST to prevent auto-400
            if (!ModelState.IsValid)
            {
                // Redirect to fresh page on validation error
                return RedirectToPage();
            }

            // Your custom validation
            if (!(BP.Systolic > BP.Diastolic))
            {
                TempData["ErrorMessage"] = "Systolic must be greater than Diastolic";
                return RedirectToPage();
            }

            // Compute category
            Category = BP.Category.ToString();
            
            // Store result to show after redirect
            TempData["CategoryResult"] = Category;

            // Logging
            _logger.LogInformation(
                "Blood Pressure reading {Systolic}/{Diastolic} categorized as {Category}",
                BP.Systolic, BP.Diastolic, Category
            );

            // Always redirect after POST
            return RedirectToPage();
        }
    }
}
