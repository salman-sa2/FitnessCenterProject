namespace FitnessCenterProject.Models
{
    public class User
    {
        public int UserId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public string Role { get; set; }      // Admin / Member
        public string Gender { get; set; }
        public DateTime CreatedAt { get; set; }

        public ICollection<Appointment> Appointments { get; set; }
    }

}
