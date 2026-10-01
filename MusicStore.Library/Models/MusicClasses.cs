using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicStore.Library.Models
{
    public class MusicClasses
    {
        public int Id { get; set; }
        [Required, MaxLength(150)]
        public string Title { get; set; } = "";
        [Required, MaxLength(50)]
        public string Artist { get; set; } = "";
        [Required, MaxLength(50)]
        public string Publisher { get; set; } = "";
        public int TrackCount { get; set; }
        [Required, MaxLength(50)]
        public string Genre { get; set; } = "";
        public int ReleaseYear { get; set; }
        public decimal Cost { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public DateTime AddedAt { get; set; } = DateTime.Now;

        public ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();
        public ICollection<Sale> Sales { get; set; } = new List<Sale>();
        public ICollection<WriteOff> WriteOffs { get; set; } = new List<WriteOff>();
    }

    public class AppUser
    {
        public int Id { get; set; }
        [Required, MaxLength(50)]
        public string Login { get; set; } = "";
        [Required, MaxLength(50)]
        public string Password { get; set; } = "";
        [Required, MaxLength(50)]
        public string Role { get; set; } = "Customer";
        public int? CustomerId {  get; set; }
        public Customer? Customer { get; set; }
    }

    public class Customer
    {
        public int Id { get; set; }
        [Required, MaxLength(100)]
        public string FullName { get; set; } = "";
        [MaxLength(50)]
        public string Phone { get; set; } = "";
        public decimal TotalSpend { get; set; }
        public ICollection<AppUser> Users { get; set; } = new List<AppUser>();
        public ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();
        public ICollection<Sale> Sales { get; set; } = new List<Sale>();
    }

    public class Promotion
    {
        public int Id { get; set; }
        [Required, MaxLength(100)]
        public string Name { get; set; } = "";
        [Required, MaxLength(100)]
        public string Genre { get; set; } = "";
        public decimal DiscountPercent { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate {  get; set; }
    }

    public class Reservation
    {
        public int Id { get; set; }
        public int MusicClassesId { get; set; }
        public MusicClasses? MusicClasses { get; set; }
        public int CustomerId { get; set; }
        public Customer? Customer { get; set; }
        public int Quantity { get; set; }
        public DateTime ReservedAt { get; set; } = DateTime.Now;
        public string Status { get; set; } = "Active";
    }

    public class Sale
    {
        public int Id { get; set; }
        public int MusicClassesId { get; set; }
        public MusicClasses? MusicClasses { get; set; }
        public int? CustomerId { get; set; }
        public Customer? Customer { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal DiscountPercent { get; set; }
        public DateTime SoldAt { get; set; } = DateTime.Now;
    }

    public class WriteOff
    {
        public int Id { get; set; }
        public int MusicClassesId { get; set; }
        public MusicClasses? MusicClasses { get; set; }
        public int Quantity { get; set; }
        public string Reason { get; set; } = "";
        public DateTime Date { get; set; } = DateTime.Now;
    }
}
