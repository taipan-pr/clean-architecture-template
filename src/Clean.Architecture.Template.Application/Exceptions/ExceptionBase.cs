using System;

namespace Clean.Architecture.Template.Application.Exceptions
{
    public class ExceptionBase : Exception
    {
        public ExceptionBase() : this(null) { }
        public ExceptionBase(string message) : base(message) { }
    }
}
