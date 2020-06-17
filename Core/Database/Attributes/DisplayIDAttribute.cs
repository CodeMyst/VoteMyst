using System;

namespace VoteMyst.Database
{
    [AttributeUsage(AttributeTargets.Property, Inherited = false, AllowMultiple = false)]
    public sealed class DisplayIDAttribute : Attribute
    {
        public int Length { get; }

        public DisplayIDAttribute(int length)
        {
            if (length < 0 || length > 28)
                throw new ArgumentException("The display ID length cannot be shorter than 0 or longer than 28 characters.");

            Length = length;
        }
    }
}