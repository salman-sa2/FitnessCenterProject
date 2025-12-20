using System.ComponentModel.DataAnnotations;

namespace FitnessCenterProject.Models
{
    public class AIRecommendationRequest
    {
        [Display(Name = "Boy (cm)")]
        [Range(100, 250, ErrorMessage = "Boy 100-250 cm arasında olmalıdır")]
        public decimal? Height { get; set; }

        [Display(Name = "Kilo (kg)")]
        [Range(30, 200, ErrorMessage = "Kilo 30-200 kg arasında olmalıdır")]
        public decimal? Weight { get; set; }

        [Display(Name = "Yaş")]
        [Range(10, 100, ErrorMessage = "Yaş 10-100 arasında olmalıdır")]
        public int? Age { get; set; }

        [Display(Name = "Cinsiyet")]
        public string? Gender { get; set; }

        [Display(Name = "Hedef")]
        public string? Goal { get; set; } // Kilo verme, Kas kazanma, Genel fitness, vb.

        [Display(Name = "Aktivite Seviyesi")]
        public string? ActivityLevel { get; set; } // Sedanter, Hafif, Orta, Yüksek

        [Display(Name = "Fotoğraf")]
        public IFormFile? Photo { get; set; }
    }
}
