using System.ComponentModel.DataAnnotations;

namespace VivesRental.Domains.Enums
{
    public enum ArticleStatus
    {
        [Display(Name = "Beschikbaar")]
        Beschikbaar = 0,

        [Display(Name = "Verhuurd")]
        Verhuurd = 1,

        [Display(Name = "Gereserveerd")]
        Gereserveerd = 2,

        [Display(Name = "Kapot")]
        Kapot = 3
    }

}
