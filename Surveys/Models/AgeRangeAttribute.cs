using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Surveys.Models
{
    public class AgeRangeAttribute : ValidationAttribute
    {
        private readonly int _minAge;
        private readonly int _maxAge;

        public AgeRangeAttribute(int minAge, int maxAge)
        {
            _minAge = minAge;
            _maxAge = maxAge;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value != null && value is DateTime)
            {
                DateTime dateOfBirth = (DateTime)value;
                int age = CalculateAge(dateOfBirth);

                if (age < _minAge || age > _maxAge)
                {
                    return new ValidationResult($"The age must be between {_minAge} and {_maxAge} years.");
                }
            }

            return ValidationResult.Success;
        }

        private int CalculateAge(DateTime dateOfBirth)
        {
            DateTime today = DateTime.Today;
            int age = today.Year - dateOfBirth.Year;
            if (dateOfBirth.Date > today.AddYears(-age))
            {
                age--; // If birthday hasn't occurred yet this year, subtract one year
            }
            return age;
        }
    }
}