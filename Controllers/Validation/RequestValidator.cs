using System;

namespace VoteMyst.Controllers.Validation
{
    public class RequestValidator
    {
        private readonly Action<RequestValidator> _validation;

        private Action<ValidationException> _invalidHandle;

        public RequestValidator(Action<RequestValidator> validation)
        {
            _validation = validation;
        }

        public RequestValidator HandleInvalid(Action<ValidationException> invalidHandle)
        {
            _invalidHandle = invalidHandle;
            return this;
        }

        public void Verify(bool condition)
        {
            if (!condition)
                throw new ValidationException();
        }
        public void Verify(bool condition, string errorMessage)
        {
            if (!condition)
                throw new ValidationException(errorMessage);
        }

        public bool Run()
        {
            try
            {
                _validation(this);
                return true;
            }
            catch (ValidationException ex)
            {
                _invalidHandle?.Invoke(ex);
                return false;
            }
        }
    }
}