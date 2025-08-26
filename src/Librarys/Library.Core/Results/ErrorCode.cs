namespace Library.Core.Results;

public static class ErrorCodes
{
    public static class Common
    {
        public const string Unknown = "common.unknown";
        public const string Validation = "common.validation";
        public const string NotFound = "common.not_found";
        public const string Conflict = "common.conflict";
        public const string Unauthorized = "common.unauthorized";
        public const string Forbidden = "common.forbidden";
        public const string Timeout = "common.timeout";
        public const string DependencyFailure = "common.dependency_failure";
        public const string Concurrency = "common.concurrency";
    }

    public static class User
    {
        public const string NotFound = "user.not_found";
        public const string EmailAlreadyUsed = "user.email_already_used";
        public const string Locked = "user.locked";
        public const string InvalidCredentials = "user.invalid_credentials";
    }
}

/*
用法示例（搭配先前的 Result 型別）：
return Result<T>.Fail(ErrorCodes.Common.Unknown);
*/
