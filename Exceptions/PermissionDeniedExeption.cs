using System.Runtime.Serialization;

namespace MovieCatalogAPI.Exceptions
{
    public class PermissionDeniedExeption : Exception
    {
        public PermissionDeniedExeption()
        {
        }

        public PermissionDeniedExeption(string? message) : base(message)
        {
        }

        public PermissionDeniedExeption(string? message, Exception? innerException) : base(message, innerException)
        {
        }

        protected PermissionDeniedExeption(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
