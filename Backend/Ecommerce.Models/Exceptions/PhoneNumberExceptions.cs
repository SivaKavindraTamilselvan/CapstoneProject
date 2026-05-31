namespace Ecommerce.Models.Exceptions;
public class PhoneNumberException : Exception
{
    private static string message = "Phone Number Entered Is Not Valid.";
    public PhoneNumberException() : base(message)
    {
        
    }
}