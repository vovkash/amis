using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Text.RegularExpressions;

namespace TestWriting.View.Validations
{
    public class EmailValidationRule : ValidationRule
    {
        public EmailValidationRule()
        { }


        public override ValidationResult Validate(object value, System.Globalization.CultureInfo cultureInfo)
        {

            bool result = Regex.IsMatch(value.ToString(), @"\A[a-z0-9]+([-._][a-z0-9]+)*@([a-z0-9]+(-[a-z0-9]+)*\.)+[a-z]{2,4}\z")
    && Regex.IsMatch(value.ToString(), @"^(?=.{1,64}@.{4,64}$)(?=.{6,100}$).*");

                if (result)
                {
                    return new ValidationResult(true, null);
                }
                else
                {
                    return new ValidationResult(false, "Entered email is invalid! Please try again");
                }
         
        }

    }
}
