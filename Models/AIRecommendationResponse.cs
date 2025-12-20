namespace FitnessCenterProject.Models
{
    public class AIRecommendationResponse
    {
        public string ExerciseRecommendation { get; set; } = string.Empty;
        public string DietRecommendation { get; set; } = string.Empty;
        public decimal? CalculatedBMI { get; set; }
        public string? BMICategory { get; set; }
        public string? Analysis { get; set; }
    }
}
