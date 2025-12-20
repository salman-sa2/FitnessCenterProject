using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using FitnessCenterProject.Models;

namespace FitnessCenterProject.Data
{
    public class ApplicationUser : IdentityUser
    {
        
        public string? Gender { get; set; }
        public DateTime? CreatedAt { get; set; } = DateTime.UtcNow;

        
        public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
    }
}
