using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace AOK.ViewModels
{
    public class AccountViewModel
    {
        [Required(ErrorMessage = "Käyttäjätunnus on pakollinen")]
        public string Username { get; set; }

        [MinLength(3, ErrorMessage = "Pelinimi oltava vähintään 3 merkkiä pitkä")]
        [MaxLength(20, ErrorMessage = "Pelinimi ei voi olla yli 40 merkkiä pitkä")]
        [DisplayName("Pelinimi")]
        public string? Nickname { get; set; }

        [DisplayName("Yhteisö")]
        public string Community { get; set; }

        [MinLength(3, ErrorMessage = "Etunimen oltava vähintään 3 merkkiä pitkä")]
        [MaxLength(20, ErrorMessage = "Etunimi ei voi olla yli 20 merkkiä pitkä")]
        [DisplayName("Etunimi")]
        public string? FirstName { get; set; }

        [MinLength(3, ErrorMessage = "Sukunimen oltava vähintään 3 merkkiä pitkä")]
        [MaxLength(20, ErrorMessage = "Sukunimi ei voi olla yli 20 merkkiä pitkä")]
        [DisplayName("Sukunimi")]
        public string? LastName { get; set; }

        [EmailAddress(ErrorMessage = "Syötä kelvollinen sähköpostiosoite")]
        [DataType(DataType.EmailAddress)]
        [DisplayName("Sähköposti")]
        public string Email { get; set; }

        public List<SelectListItem> Communities { get; set; } = new();
        public string? SelectedCommunity { get; set; }
    }
}
