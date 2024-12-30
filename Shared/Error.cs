namespace AwsS3.Shared
{
    public class Error(string message)
    {
        public string Message { get; } = message;

        public static readonly Error None = new(string.Empty);

        public static implicit operator Error(string message) => new(message);
    }
}
