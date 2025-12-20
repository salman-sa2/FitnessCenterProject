using System.ComponentModel.DataAnnotations;

namespace FitnessCenterProject.Models
{
   
    public class Gym
    {
        public int GymId { get; set; }
        
        [Required(ErrorMessage = "Gym adı gereklidir")]
        [StringLength(100, ErrorMessage = "Gym adı en fazla 100 karakter olabilir")]
        [Display(Name = "Gym Adı")]
        public string Name { get; set; }
        
        [Required(ErrorMessage = "Açılış saati gereklidir")]
        [Display(Name = "Açılış Saati")]
        public TimeSpan OpenTime { get; set; }
        
        [Required(ErrorMessage = "Kapanış saati gereklidir")]
        [Display(Name = "Kapanış Saati")]
        public TimeSpan CloseTime { get; set; }

        // Navigation
        public ICollection<Service> Services { get; set; } = new List<Service>();
        public ICollection<Trainer> Trainers { get; set; } = new List<Trainer>();
        public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
    }

}
