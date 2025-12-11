namespace FitnessCenterProject.Models
{
    public class Trainer
    {
        public int TrainerId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Specialization { get; set; }

        public int GymId { get; set; }
        public Gym Gym { get; set; }

        public ICollection<TrainerService> TrainerServices { get; set; }
        public ICollection<TrainerAvailability> Availabilities { get; set; }
        public ICollection<Appointment> Appointments { get; set; }
    }

}
