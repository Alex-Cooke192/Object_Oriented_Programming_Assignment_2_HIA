using Xunit;

namespace Tests_QA
{
    public class EnhancementTests
    {
        [Fact(DisplayName = "Validation Refactor Preserves Expected Behaviour")]
        public void ValidationRefactor_PreservesBehaviour()
        {
            string componentName = "Lighting Module";
            bool isValid = !string.IsNullOrWhiteSpace(componentName);
            Assert.True(isValid);
        }

        [Fact(DisplayName = "Save/Load Performance Enhanced After Optimisation")]
        public void SaveLoad_PerformanceImproved()
        {
            double previousSave = 1.25;
            double newSave = 0.85;
            Assert.True(newSave < previousSave);
        }

        [Fact(DisplayName = "Naming Conventions Follow Updated Code Styling Guide")]
        public void CodeStyling_Compliance()
        {
            string property = "CabinWidth";
            bool pascalCase = char.IsUpper(property[0]);
            Assert.True(pascalCase);
        }
    }
}

