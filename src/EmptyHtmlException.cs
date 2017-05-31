using System;
using System.Runtime.Serialization;

namespace Jaguar.Reporting.Generators
{
    [Serializable]
    internal class EmptyHtmlException : Exception
    {
        public EmptyHtmlException()
        {
        }

        public EmptyHtmlException(string message) : base(message)
        {
        }

        public EmptyHtmlException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}