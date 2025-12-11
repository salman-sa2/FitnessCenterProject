namespace FitnessCenterProject.Models
{
    public class TrainerAvailability
    {
        public int AvailabilityId { get; set; }

        public int Day { get; set; }       // 0-6 or use enum DayOfWeek
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }

        public int TrainerId { get; set; }
        public Trainer Trainer { get; set; }
    }

}
