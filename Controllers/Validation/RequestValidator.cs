using System;

namespace VoteMyst.Controllers.Validation
{
    public class RequestValidator
    {
        private readonly Action<ValidationProvider> _validation;

        private Action<ValidationException> _invalidHandle;

        public RequestValidator(Action<ValidationProvider> validation)
        {
            _validation = validation;
        }

        public RequestValidator HandleInvalid(Action<ValidationException> invalidHandle)
        {
            _invalidHandle = invalidHandle;
            return this;
        }

        public bool Run()
        {
            try
            {
                _validation(new ValidationProvider());
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