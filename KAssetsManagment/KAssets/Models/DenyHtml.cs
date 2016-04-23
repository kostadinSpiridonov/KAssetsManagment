using KAssets.Resources.Translation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace KAssets.Models
{
    class DenyHtmlAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value != null)
            {
                string val=value.ToString();
                if (val.Contains("<!")
                    || val.Contains("&#")
                    || Regex.IsMatch(val, "<[a-zA-Z]"))
                {
                    return new ValidationResult(Common.HTMLNotAllowed);
                }
                else {
                    return ValidationResult.Success;
                }
            }
            else {
                return ValidationResult.Success;
            }
        }
    }
}
