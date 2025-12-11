namespace FitnessCenterProject.Models
{
    // Models/TrainerService.cs
    public class TrainerService
    {
        public int TrainerServiceId { get; set; }

        public int TrainerId { get; set; }
        public Trainer Trainer { get; set; }

        public int ServiceId { get; set; }
        public Service Service { get; set; }
    }

}
