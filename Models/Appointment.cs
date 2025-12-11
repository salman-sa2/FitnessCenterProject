namespace FitnessCenterProject.Models
{
   
    public class Appointment
    {
        public int AppointmentId { get; set; }
        public DateTime Date { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public string Status { get; set; }
        public decimal Price { get; set; }
        public DateTime CreatedAt { get; set; }

        public int UserId { get; set; }
        public User User { get; set; }

        public int ServiceId { get; set; }
        public Service Service { get; set; }

        public int TrainerId { get; set; }
        public Trainer Trainer { get; set; }

        public int GymId { get; set; }
        public Gym Gym { get; set; }
    }

}
