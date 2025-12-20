using FitnessCenterProject.Data;
using System.ComponentModel.DataAnnotations;

namespace FitnessCenterProject.Models
{
    
    public class Appointment
    {
        public int AppointmentId { get; set; }
        
        [Required(ErrorMessage = "Tarih gereklidir")]
        [Display(Name = "Tarih")]
        public DateTime Date { get; set; }
        
        [Required(ErrorMessage = "Başlangıç saati gereklidir")]
        [Display(Name = "Başlangıç Saati")]
        public TimeSpan StartTime { get; set; }
        
        [Required(ErrorMessage = "Bitiş saati gereklidir")]
        [Display(Name = "Bitiş Saati")]
        public TimeSpan EndTime { get; set; }
        
        [StringLength(50, ErrorMessage = "Durum en fazla 50 karakter olabilir")]
        [Display(Name = "Durum")]
        public string? Status { get; set; }
        
        [Required(ErrorMessage = "Fiyat gereklidir")]
        [Range(0, 100000, ErrorMessage = "Fiyat 0 ile 100000 arasında olmalıdır")]
        [Display(Name = "Fiyat")]
        public decimal Price { get; set; }
        
        [Display(Name = "Oluşturulma Tarihi")]
        public DateTime? CreatedAt { get; set; }

        [Display(Name = "Kullanıcı")]
        public string? UserId { get; set; }      
        public ApplicationUser? User { get; set; }
        
        [Required(ErrorMessage = "Servis seçimi gereklidir")]
        [Display(Name = "Service")]
        public int ServiceId { get; set; }
        public Service? Service { get; set; }
        
        [Required(ErrorMessage = "Antrenör seçimi gereklidir")]
        [Display(Name = "Trainer")]
        public int TrainerId { get; set; }
        public Trainer? Trainer { get; set; }
        
        [Required(ErrorMessage = "Gym seçimi gereklidir")]
        [Display(Name = "Gym")]
        public int GymId { get; set; }
        public Gym? Gym { get; set; }
    }

}
