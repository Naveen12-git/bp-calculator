using System;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;

namespace BPCalculator
{
    // BP categories
    public enum BPCategory
    {
        [Display(Name="Low Blood Pressure")] Low,
        [Display(Name="Ideal Blood Pressure")] Ideal,
        [Display(Name="Pre-High Blood Pressure")] PreHigh,
        [Display(Name="High Blood Pressure")] High
    };

    public class BloodPressure
    {
        public const int SystolicMin = 70;
        public const int SystolicMax = 190;
        public const int DiastolicMin = 40;
        public const int DiastolicMax = 100;

        [Range(SystolicMin, SystolicMax, ErrorMessage = "Invalid Systolic Value")]
        public int Systolic { get; set; }                       // mmHG

        [Range(DiastolicMin, DiastolicMax, ErrorMessage = "Invalid Diastolic Value")]
        public int Diastolic { get; set; }                      // mmHG

        // calculate BP category - using the enum
        public BPCategory Category
        {
            get
            {
                if (Systolic >= 140 || Diastolic >= 90)
                    return BPCategory.High;
                else if (Systolic >= 120 || Diastolic >= 80)
                    return BPCategory.PreHigh;
                else if (Systolic >= 90 && Diastolic >= 60)
                    return BPCategory.Ideal;
                else
                    return BPCategory.Low;
            }
        }

        // Add this method for string output and testing
        public string CalculateCategory()
        {
            return Category.ToString();
        }
        
        // Static method for testing
        public static string CalculateCategory(int systolic, int diastolic)
        {
            if (systolic >= 140 || diastolic >= 90)
                return "High";
            else if (systolic >= 120 || diastolic >= 80)
                return "PreHigh";
            else if (systolic >= 90 && diastolic >= 60)
                return "Ideal";
            else
                return "Low";
        }
    }

}




// BMI Calculator Feature
public class BMI
{
    public const double WeightMin = 30;
    public const double WeightMax = 300;
    public const double HeightMin = 1.0;
    public const double HeightMax = 2.5;

    public double Weight { get; set; }  // kg
    public double Height { get; set; }  // meters

    public double BMIScore => Weight / (Height * Height);

    public string Category
    {
        get
        {
            return BMIScore switch
            {
                < 18.5 => "Underweight",
                >= 18.5 and < 25 => "Normal", 
                >= 25 and < 30 => "Overweight",
                _ => "Obese"
            };
        }
    }

    // Static method for testing (ADD THIS)
    public static string CalculateCategory(double weight, double height)
    {
        var bmiScore = weight / (height * height);
        
        return bmiScore switch
        {
            < 18.5 => "Underweight",
            >= 18.5 and < 25 => "Normal",
            >= 25 and < 30 => "Overweight", 
            _ => "Obese"
        };
    }
}
