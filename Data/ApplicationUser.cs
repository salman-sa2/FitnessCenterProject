using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using FitnessCenterProject.Models;

namespace FitnessCenterProject.Data
{
    public class ApplicationUser : IdentityUser
    {
        
        public string Gender { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // İlişki: 1 kullanıcı, N randevu
        public ICollection<Appointment> Appointments { get; set; }
    }
}
