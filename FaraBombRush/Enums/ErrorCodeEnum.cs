using System;
using FaraBombRush.Exceptions;

namespace FaraBombRush.Enums;

internal enum ErrorCodeEnum
{
    // FaraBombSystemManager
    DoesNotExistPrefab = 100000,

    // FaraBombManagementPoolController
    InitializeInstanceError = 101000,
    SpawnError = 101001,
    DespawnError = 101002,
    CleanupError = 101003,

    // LoadSteamController
    LoadSteamError = 102000
}

internal static class ErrorCodeEnumHelper
{
    internal static int GetErrorCodeIndex(string errorCode)
    {
        if (Enum.TryParse(errorCode, out ErrorCodeEnum errorCodeEnum)) return (int) errorCodeEnum;

        throw new FaraBombException($"Unknown ErrorCode: {errorCode}");
    }
}