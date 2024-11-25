namespace Grime.Core
{
    public class InvalidElfException : Exception
    {
        public InvalidElfException(string? message) : base(message)
        {

        }
    }

    /// <summary>
    /// Raised when a virtual address does not exist within the allocated virtual memory
    /// </summary>
    public class AddressOutOfBoundsException : Exception
    {
        public AddressOutOfBoundsException(string? message) : base(message) { }
    }

    /// <summary>
    /// An operation was attempted on a section of memory that is disallowed
    /// </summary>
    public class AccessViolationException : Exception
    {
        public AccessViolationException(string? message) : base(message) { }
    }

    public class InvalidInstructionException : Exception
    {
        public InvalidInstructionException(string? message) : base(message) { }
    }
}
