namespace AssemblyAI.Realtime.Enums
{
    public enum WebsocketStatus
    {
        None = 0,

        /// <summary>
        /// Sample rate must be a positive integer.
        /// </summary>
        BadSampleRate = 4000,

        /// <summary>
        /// Not Authorized.
        /// </summary>
        AuthFailed = 4001,

        /// <summary>
        /// Insufficient Funds, or free user.
        /// 
        /// This feature is paid-only and requires you to add a credit card. Please visit https://app.assemblyai.com/ to add a credit card to your account.
        /// </summary>
        InsufficientFunds = 4002,

        /// <summary>
        /// Session not found.
        /// </summary>
        AttemptToConnectToNonexistentSessionId = 4004,

        /// <summary>
        /// Session Expired.
        /// </summary>
        SessionExpired = 4008,

        /// <summary>
        /// Session previously closed.
        /// </summary>
        AttemptToConnectToClosedSession = 4010,

        /// <summary>
        /// Client sent audio too fast.
        /// </summary>
        RateLimited = 4029,

        /// <summary>
        /// Session is handled by another WebSocket.
        /// </summary>
        UniqueSessionViolation = 4030,

        /// <summary>
        /// Session idle for too long.
        /// </summary>
        SessionTimesOut = 4031,

        /// <summary>
        /// Audio duration is too short.
        /// </summary>
        AudioTooShort = 4032,

        /// <summary>
        /// Audio duration is too long.
        /// </summary>
        AudioTooLong = 4033,

        /// <summary>
        /// Endpoint received invalid JSON.
        /// </summary>
        BadJson = 4100,

        /// <summary>
        /// Endpoint received a message with an invalid schema.
        /// </summary>
        BadSchema = 4101,

        /// <summary>
        /// This account has exceeded the number of allowed streams.
        /// </summary>
        TooManyStreams = 4102,

        /// <summary>
        /// This session has been reconnected. This WebSocket is no longer valid.
        /// </summary>
        Reconnected = 4103,

        /// <summary>
        /// Temporary server condition forced blocking client's request.
        /// </summary>
        ReconnectAttemptsExhausted = 1013
    }

    public static class ResultCodeExtensions
    {
        public static string? GetResultMessage(WebsocketStatus resultCode)
        {
            switch (resultCode)
            {
                case WebsocketStatus.BadSampleRate:
                    return "Sample rate must be a positive integer";

                case WebsocketStatus.AuthFailed:
                    return "Not Authorized";

                case WebsocketStatus.InsufficientFunds:
                    return "Insufficient Funds, or free tier user.  This feature is paid-only and requires you to add a credit card. Please visit https://app.assemblyai.com/ to add a credit card to your account";

                case WebsocketStatus.AttemptToConnectToNonexistentSessionId:
                    return "Session not found";

                case WebsocketStatus.SessionExpired:
                    return "Session Expired";

                case WebsocketStatus.AttemptToConnectToClosedSession:
                    return "Session previously closed";

                case WebsocketStatus.RateLimited:
                    return "Client sent audio too fast";

                case WebsocketStatus.UniqueSessionViolation:
                    return "Session is handled by another WebSocket";

                case WebsocketStatus.SessionTimesOut:
                    return "Session idle for too long";

                case WebsocketStatus.AudioTooShort:
                    return "Audio duration is too short";

                case WebsocketStatus.AudioTooLong:
                    return "Audio duration is too long";

                case WebsocketStatus.BadJson:
                    return "Endpoint received invalid JSON";

                case WebsocketStatus.BadSchema:
                    return "Endpoint received a message with an invalid schema";

                case WebsocketStatus.TooManyStreams:
                    return "This account has exceeded the number of allowed streams";

                case WebsocketStatus.Reconnected:
                    return "This session has been reconnected. This WebSocket is no longer valid.";

                case WebsocketStatus.ReconnectAttemptsExhausted:
                    return "Temporary server condition forced blocking client's request";

                default:
                    return null;
            }
        }
    }
}
