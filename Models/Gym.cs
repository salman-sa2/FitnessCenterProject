namespace FitnessCenterProject.Models
{
   
    public class Gym
    {
        public int GymId { get; set; }
        public string Name { get; set; }
        public TimeSpan OpenTime { get; set; }
        public TimeSpan CloseTime { get; set; }

        // Navigation
        public ICollection<Service> Services { get; set; } = new List<Service>();
        public ICollection<Trainer> Trainers { get; set; } = new List<Trainer>();
        public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
    }

}
