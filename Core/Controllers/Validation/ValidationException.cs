using System;

namespace VoteMyst.Controllers.Validation
{
    [System.Serializable]
    public class ValidationException : Exception
    {
        public ValidationException() { }
        public ValidationException(string message) : base(message) { }
    }
}