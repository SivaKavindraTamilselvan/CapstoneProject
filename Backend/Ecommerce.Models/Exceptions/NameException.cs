namespace Ecommerce.Models.Exceptions;
public class NameException : Exception
{
    private static string message = "Name Entered Is Not Valid.";
    public NameException() : base(message)
    {
        
    }
}