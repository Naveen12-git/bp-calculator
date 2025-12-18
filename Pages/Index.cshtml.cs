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
            
            // 🔧 FIX: Retrieve TempData values after redirect
            if (TempData["CategoryResult"] is string category)
            {
                Category = category;
                TempData.Keep("CategoryResult"); // Keep for display
            }
            
            if (TempData["ErrorMessage"] is string error)
            {
                ModelState.AddModelError("", error);
                TempData.Keep("ErrorMessage");
            }
            
            if (TempData["ValidationError"] is string validationError)
            {
                ModelState.AddModelError("", validationError);
                TempData.Keep("ValidationError");
            }
        }

        public IActionResult OnPost()
        {
            // 🔧 FIX: Check validation FIRST
            if (!ModelState.IsValid)
            {
                TempData["ValidationError"] = "Please check your input values";
                return RedirectToPage(); // Redirect instead of Page()
            }

            // Your custom validation
            if (!(BP.Systolic > BP.Diastolic))
            {
                TempData["ErrorMessage"] = "Systolic must be greater than Diastolic";
                return RedirectToPage(); // Redirect instead of Page()
            }

            // Compute category
            Category = BP.Category.ToString();
            
            // 🔧 FIX: Store result for redirect
            TempData["CategoryResult"] = Category;

            // Telemetry logging
            _logger.LogInformation(
                "Blood Pressure reading {Systolic}/{Diastolic} categorized as {Category}",
                BP.Systolic, BP.Diastolic, Category
            );

            // 🔧 FIX: ALWAYS redirect after POST
            return RedirectToPage();
        }
    }
}
