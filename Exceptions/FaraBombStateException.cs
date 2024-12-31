using FaraBombRush.Enums;

namespace FaraBombRush.Exceptions;

public class FaraBombStateException : FaraBombException
{
    public FaraBombStateException(FaraBombStateEnum currentState, FaraBombStateEnum attemptedState)
        : base($"Invalid state transition from {currentState} to {attemptedState}") {}
}