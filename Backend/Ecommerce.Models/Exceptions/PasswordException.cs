namespace Ecommerce.Models.Exceptions;

public class PasswordException : Exception
{
    public PasswordException(string message) : base(message)
    {

    }
}

public class InvalidTokenException : Exception
{
    public InvalidTokenException(string message) : base(message) { }
}

public class TokenExpiredException : Exception
{
    public TokenExpiredException(string message) : base(message) { }
}


public class EmailSendException : Exception
{
    public EmailSendException(string message) : base(message) { }
}
