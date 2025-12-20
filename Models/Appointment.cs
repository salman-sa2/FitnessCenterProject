using FitnessCenterProject.Data;
using System.ComponentModel.DataAnnotations;

namespace FitnessCenterProject.Models
{
    
    public class Appointment
    {
        public int AppointmentId { get; set; }
        public DateTime Date { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public string? Status { get; set; }
        public decimal Price { get; set; }
        public DateTime? CreatedAt { get; set; }

        public string? UserId { get; set; }      
        public ApplicationUser? User { get; set; }
        [Display(Name = "Service")]
        public int ServiceId { get; set; }
        public Service? Service { get; set; }
        [Display(Name = "Trainer")]
        public int TrainerId { get; set; }
        public Trainer? Trainer { get; set; }
        [Display(Name = "Gym")]
        public int GymId { get; set; }
        public Gym? Gym { get; set; }
    }

}
