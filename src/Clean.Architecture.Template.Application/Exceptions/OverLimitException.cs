namespace Clean.Architecture.Template.Application.Exceptions
{
    public class OverLimitException : ExceptionBase
    {
        public OverLimitException() : this(null) { }
        public OverLimitException(string message) : base(message) { }
    }
}
