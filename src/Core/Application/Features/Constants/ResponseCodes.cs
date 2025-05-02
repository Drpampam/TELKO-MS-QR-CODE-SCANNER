namespace Application.Features.Constants
{
    public static class ResponseCodes
    {
        public const string SUCCESS = "00";
        public const string CREATED = "01";
        public const string UPDATED = "03";
        public const string DELETED = "04";
        public const string DUPLICATE_RESOURCE = "06";
        public const string DELETED_RESOURCE = "07";
        public const string SERVER_ERROR = "08";
        public const string VALIDATION_ERROR = "09";
        public const string PARTIAL_SUCCESS = "10";

        // Newly added
        public const string INVALID_REQUEST = "11";      // For bad or malformed requests
        public const string UNAUTHORIZED = "12";         // When auth fails (token missing/invalid)
        public const string FORBIDDEN = "13";            // Authenticated but no permission
        public const string CONFLICT = "14";             // For resource conflicts (e.g., version conflicts)
        public const string TOO_MANY_REQUESTS = "15";    // For rate-limiting scenarios
        public const string UNSUPPORTED_MEDIA_TYPE = "16"; // For unsupported file formats, etc.
        public const string TIMEOUT = "17";              // For timeout cases
        public const string NOT_IMPLEMENTED = "18";      // For unimplemented API endpoints
        public const string BAD_GATEWAY = "19";          // When dependent services fail
        public const string NOT_FOUND = "97";
        public const string FAILURE = "99";
    }
}