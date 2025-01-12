using System;
using FaraBombRush.Exceptions;

namespace FaraBombRush.Enums;

public class ErrorCodeEnum
{
    public enum ErrorCode
    {
        // FaraBombSystemManager
        DoesNotExistPrefab = 100000,
        // FaraBombManagementPoolController
        InitializeInstanceError = 101000,
        SpawnError = 101001,
        DespawnError = 101002,
        CleanupError = 101003,
    }
    
    public static int GetErrorCodeIndex(string errorCode)
    {
        if (Enum.TryParse(errorCode, out ErrorCode code))
        {
            return (int)code;
        }

        throw new FaraBombException($"Unknown ErrorCode: {errorCode}");
    }
}