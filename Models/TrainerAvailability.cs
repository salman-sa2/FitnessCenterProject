using System;
using System.ComponentModel.DataAnnotations;

namespace FitnessCenterProject.Models
{
    public class TrainerAvailability
    {
        [Key]
        public int AvailabilityId { get; set; }

        public DayOfWeek Day { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        [Display(Name = "Trainer")]
        public int TrainerId { get; set; }
        public Trainer? Trainer { get; set; }
    }

}
