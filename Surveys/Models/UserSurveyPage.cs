using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Data.Entity;
using System.ComponentModel.DataAnnotations;


namespace Surveys.Models
{
    public class UserSurveyPage
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int UserId { get; set; }
        [Required]
        public string Fullname { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Display(Name = "Date of Birth")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [AgeRange(5, 120, ErrorMessage = "Age must be between 5 and 120 years.")]
        public DateTime DateOfBirth { get; set; }

        
        [MaxLength(10), MinLength(10)]
        [Required]
        public string Contact { get; set; }
        // Age Validation
       //public int AgeNumber { get; set; }

        //checkboxes 
        
        public bool Pizza { get; set; }
        public bool Pasta { get; set; }
        public bool PapAndWors { get; set; }
        public bool Other { get; set; }

        //RadioButtons
        [Required]

        [Display(Name = "Watch Movies Rating")]
        public string WatchMoviesRating { get; set; }
        [Required]

        [Display(Name = "Listen to Radio Rating")]
        public string ListenToRadioRating { get; set; }
        [Required]

        [Display(Name = "Eat Out Rating")]
        public string EatOutRating { get; set; }
        [Required]

        [Display(Name = "Watch TV Rating")]
        public string WatchTVRating { get; set; }

    }
}