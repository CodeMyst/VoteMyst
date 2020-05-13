namespace VoteMyst.Controllers.Validation
{
    public sealed class ValidationProvider
    {
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
    }
}