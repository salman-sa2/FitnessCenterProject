using System.ComponentModel.DataAnnotations;

namespace FitnessCenterProject.Models
{
    public class TrainerService
    {
        public int TrainerServiceId { get; set; }
        [Display(Name = "Trainer")]
        public int TrainerId { get; set; }
        public Trainer? Trainer { get; set; }
        [Display(Name = "Service")]
        public int ServiceId { get; set; }
        public Service? Service { get; set; }
    }

}
