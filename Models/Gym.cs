namespace FitnessCenterProject.Models
{
   
    public class Gym
    {
        public int GymId { get; set; }
        public string Name { get; set; }
        public TimeSpan OpenTime { get; set; }
        public TimeSpan CloseTime { get; set; }

        // Navigation
        public ICollection<Service> Services { get; set; }
        public ICollection<Trainer> Trainers { get; set; }
        public ICollection<Appointment> Appointments { get; set; }
    }

}
