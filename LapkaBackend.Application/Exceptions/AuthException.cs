using System.Globalization;


namespace LapkaBackend.Application.Exceptions
{
    public class AuthException : Exception
    {
        public int StatusCode { get; set; }
        public AuthException() : base()
        { 

        }

        public AuthException(string message) : base(message)
        {

        }

        public AuthException(string message, params object[] args) : base(String.Format(CultureInfo.CurrentCulture, message, args))
        {

        }
        
        public AuthException(string message, int statusCode) : base(message)
        {
            StatusCode = statusCode;
        }
        
        public enum StatusCodes
        {
            BadRequest = 400,
            Forbidden = 403,
            InternalServerError = 500
        }
    }
}