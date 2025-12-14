using System.ComponentModel.DataAnnotations;

namespace FitnessCenterProject.Models
{
    public class Trainer
    {
        public int TrainerId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Specialization { get; set; }

        [Display(Name = "Gym")]
        public int GymId { get; set; }
        public Gym? Gym { get; set; }

        public ICollection<TrainerService> TrainerServices { get; set; } = new List<TrainerService>();
        public ICollection<TrainerAvailability> Availabilities { get; set; } = new List<TrainerAvailability>();

        public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
    }

}
