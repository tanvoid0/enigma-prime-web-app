using System.ComponentModel.DataAnnotations;

namespace enigma_prime.Models
{
    public class UserRole
    {
        [Key]
        public int Id { get; set; }
        public string UserId { get; set; }
        public string RoleName { get; set; }
    }
}