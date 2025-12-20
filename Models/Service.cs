using System.ComponentModel.DataAnnotations;

namespace FitnessCenterProject.Models
{
    public class Service
    {
        public int ServiceId { get; set; }
        
        [Required(ErrorMessage = "Servis adı gereklidir")]
        [StringLength(100, ErrorMessage = "Servis adı en fazla 100 karakter olabilir")]
        [Display(Name = "Servis Adı")]
        public string Name { get; set; }
        
        [Required(ErrorMessage = "Süre gereklidir")]
        [Range(1, 480, ErrorMessage = "Süre 1-480 dakika arasında olmalıdır")]
        [Display(Name = "Süre (dakika)")]
        public int Duration { get; set; }
        
        [Required(ErrorMessage = "Fiyat gereklidir")]
        [Range(0.01, 999999.99, ErrorMessage = "Fiyat 0.01 ile 999999.99 arasında olmalıdır")]
        [Display(Name = "Fiyat")]
        public decimal Price { get; set; }
        
        [Required(ErrorMessage = "Gym seçimi gereklidir")]
        [Display(Name = "Gym")]
        public int GymId { get; set; }
        // IMPORTANT: nullable yap
        public Gym? Gym { get; set; }

        // varsa bunları da init et (required hatası yemesin)
        public ICollection<TrainerService> TrainerServices { get; set; } = new List<TrainerService>();
        public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
    }

}
