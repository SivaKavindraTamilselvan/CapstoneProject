namespace Ecommerce.Models.Exceptions;
public class DataNotFoundException : Exception
{
    public DataNotFoundException(string message) : base(message)
    {
        
    }
}