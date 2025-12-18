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
            // Initialize with default values for the form
            BP = new BloodPressure() { Systolic = 100, Diastolic = 60 };
            Category = null; // Clear any previous result
        }

        public IActionResult OnPost()
        {
            // 🔧 CRITICAL FIX: Check model validation FIRST
            if (!ModelState.IsValid)
            {
                // Return the page to show validation errors (like "Required field")
                return Page();
            }

            // Your custom validation: systolic must be > diastolic
            if (!(BP.Systolic > BP.Diastolic))
            {
                ModelState.AddModelError("", "Systolic must be greater than Diastolic");
                return Page();
            }

            // Compute category using the enum-based property
            Category = BP.Category.ToString();

            // Telemetry logging
            _logger.LogInformation(
                "Blood Pressure reading {Systolic}/{Diastolic} categorized as {Category}",
                BP.Systolic, BP.Diastolic, Category
            );

            return Page();
        }
    }
}
