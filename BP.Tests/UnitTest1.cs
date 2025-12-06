using Xunit;
using BPCalculator;

namespace BPCalculator.Tests
{
    public class BloodPressureTests
    {
        // Existing BP Tests
        [Fact]
        public void Category_Is_High_When_Sys150_Dia95()
        {
            var result = BloodPressure.CalculateCategory(150, 95);
            Assert.Equal("High", result);
        }

        [Fact]
        public void Category_Is_PreHigh_When_Sys130_Dia85()
        {
            var result = BloodPressure.CalculateCategory(130, 85);
            Assert.Equal("PreHigh", result);
        }

        [Fact]
        public void Category_Is_Ideal_When_Sys110_Dia70()
        {
            var result = BloodPressure.CalculateCategory(110, 70);
            Assert.Equal("Ideal", result);
        }

        [Fact]
        public void Category_Is_Low_When_Sys80_Dia50()
        {
            var result = BloodPressure.CalculateCategory(80, 50);
            Assert.Equal("Low", result);
        }

        // New BMI Tests
        [Fact]
        public void BMI_NormalRange_ReturnsNormalCategory()
        {
            var bmi = new BMI { Weight = 70, Height = 1.75 };
            Assert.Equal("Normal", bmi.Category);
            Assert.InRange(bmi.BMIScore, 22.8, 22.9);
        }

        [Fact]
        public void BMI_Underweight_ReturnsUnderweightCategory()
        {
            var bmi = new BMI { Weight = 50, Height = 1.75 };
            Assert.Equal("Underweight", bmi.Category);
        }

        [Fact]
        public void BMI_Overweight_ReturnsOverweightCategory()
        {
            var bmi = new BMI { Weight = 85, Height = 1.75 };
            Assert.Equal("Overweight", bmi.Category);
        }

        [Fact]
        public void BMI_Obese_ReturnsObeseCategory()
        {
            var bmi = new BMI { Weight = 100, Height = 1.75 };
            Assert.Equal("Obese", bmi.Category);
        }

        [Fact]
        public void BMI_StaticMethod_CalculatesCorrectly()
        {
            var category = BMI.CalculateCategory(70, 1.75);
            Assert.Equal("Normal", category);
        }
    }
}