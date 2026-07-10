namespace Ecommerce.Models.Exceptions;
public class DataAlreadyRegisteredException : Exception
{
    public DataAlreadyRegisteredException(string message) : base(message)
    {
        
    }
}