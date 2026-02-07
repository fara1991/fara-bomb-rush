using System;
using System.Runtime.Serialization;
using FaraBombRush.Enums;

namespace FaraBombRush.Exceptions;

[Serializable]
internal class FaraBombException : Exception
{
    // メッセージを受け取るコンストラクタ
    internal FaraBombException(string errorMessage) : base(errorMessage)
    {
        Plugin.Logger.Critical(errorMessage);
    }

    // メッセージとエラーコードを受け取るコンストラクタ
    internal FaraBombException(string errorMessage, ErrorCodeEnum errorCodeEnum) : base(errorMessage)
    {
        Plugin.Logger.Critical($"ErrorMessage: {errorMessage}, ErrorCode: {errorCodeEnum}");
    }

    // メッセージと内部例外を受け取るコンストラクタ
    internal FaraBombException(string errorMessage, Exception innerException) : base(errorMessage, innerException)
    {
        Plugin.Logger.Critical($"ErrorMessage: {errorMessage}, InnerException: {innerException.Message}");
    }

    // 逆シリアル化コンストラクタ。このクラスの逆シリアル化のために必須。
    protected FaraBombException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}
