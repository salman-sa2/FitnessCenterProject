using System.ComponentModel.DataAnnotations;

namespace FitnessCenterProject.Models
{
    public class Service
    {
        public int ServiceId { get; set; }
        public string Name { get; set; }
        public int Duration { get; set; }
        public decimal Price { get; set; }
        [Display(Name = "Gym")]
        public int GymId { get; set; }
        // IMPORTANT: nullable yap
        public Gym? Gym { get; set; }

        // varsa bunları da init et (required hatası yemesin)
        public ICollection<TrainerService> TrainerServices { get; set; } = new List<TrainerService>();
        public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
    }

}
