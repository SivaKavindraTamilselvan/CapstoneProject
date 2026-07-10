namespace Ecommerce.Models.Exceptions;
public class EmailException : Exception
{
    private static string message = "Email Entered Is Not Valid.";
    public EmailException() : base(message)
    {
        
    }
}