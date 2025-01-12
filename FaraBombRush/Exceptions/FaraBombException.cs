using System;
using System.Runtime.Serialization;
using static FaraBombRush.Enums.ErrorCodeEnum;

namespace FaraBombRush.Exceptions;

[Serializable]
public class FaraBombException : Exception
{
    private int ErrorCode { get; set; }
    
    // メッセージを受け取るコンストラクタ
    public FaraBombException(string errorMessage) : base(errorMessage)
    {
        Plugin.Logger.Error(errorMessage);
    }

    // メッセージとエラーコードを受け取るコンストラクタ
    public FaraBombException(string errorMessage, ErrorCode errorCode) : base(errorMessage)
    {
        Plugin.Logger.Error($"ErrorMessage: {errorMessage}, ErrorCode: {errorCode}");
    }

    // メッセージと内部例外を受け取るコンストラクタ
    public FaraBombException(string errorMessage, Exception innerException) : base(errorMessage, innerException)
    {
        Plugin.Logger.Error($"ErrorMessage: {errorMessage}, InnerException: {innerException.Message}");
    }

    // 逆シリアル化コンストラクタ。このクラスの逆シリアル化のために必須。
    protected FaraBombException(SerializationInfo info, StreamingContext context) : base(info, context)
    {
    }
}