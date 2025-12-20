using System.ComponentModel.DataAnnotations;

namespace FitnessCenterProject.Models
{
    public class Trainer
    {
        public int TrainerId { get; set; }
        
        [Required(ErrorMessage = "Antrenör adı gereklidir")]
        [StringLength(100, ErrorMessage = "Antrenör adı en fazla 100 karakter olabilir")]
        [Display(Name = "Antrenör Adı")]
        public string Name { get; set; }
        
        [Required(ErrorMessage = "E-posta gereklidir")]
        [EmailAddress(ErrorMessage = "Geçerli bir e-posta adresi giriniz")]
        [StringLength(200, ErrorMessage = "E-posta en fazla 200 karakter olabilir")]
        [Display(Name = "E-posta")]
        public string Email { get; set; }
        
        [Required(ErrorMessage = "Uzmanlık alanı gereklidir")]
        [StringLength(100, ErrorMessage = "Uzmanlık alanı en fazla 100 karakter olabilir")]
        [Display(Name = "Uzmanlık Alanı")]
        public string Specialization { get; set; }

        [Required(ErrorMessage = "Gym seçimi gereklidir")]
        [Display(Name = "Gym")]
        public int GymId { get; set; }
        public Gym? Gym { get; set; }

        public ICollection<TrainerService> TrainerServices { get; set; } = new List<TrainerService>();
        public ICollection<TrainerAvailability> Availabilities { get; set; } = new List<TrainerAvailability>();

        public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
    }

}
