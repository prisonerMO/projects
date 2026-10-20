using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
namespace SQLChecker.ViewModels
{
    public class CreateUserViewModel
    {
        [Required]
        [MinLength(3)]
        [MaxLength(40)]
        [DisplayName("KäyttäjäId")]
        public string UserId { get; set; }

        [Required]
        [MinLength(3)]
        [MaxLength(40)]
        [DisplayName("Käyttäjätunnus")]
        public string Username { get; set; }

        [Required]
        [MinLength(3)]
        [MaxLength(40)]
        [DisplayName("CourseId")]
        public string CourseId { get; set; }

        [Required]
        public List<string> Roles { get; set; }


        public bool Register { get; set; }
    }
}
