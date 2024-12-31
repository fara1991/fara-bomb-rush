using System;

namespace FaraBombRush.Exceptions;

public class FaraBombException : Exception
{
    public FaraBombException(string message) : base(message)
    {
        
    }
    public FaraBombException(string message, Exception inner) : base(message, inner) {}
}
