namespace FitnessCenterProject.Models
{
    // Models/Service.cs
    public class Service
    {
        public int ServiceId { get; set; }
        public string Name { get; set; }
        public int Duration { get; set; }   // minutes
        public decimal Price { get; set; }

        public int GymId { get; set; }
        public Gym Gym { get; set; }

        public ICollection<TrainerService> TrainerServices { get; set; }
        public ICollection<Appointment> Appointments { get; set; }
    }

}
