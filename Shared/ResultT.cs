namespace AwsS3.Shared
{
    public class Result<T> : Result
    {
        private readonly T? _value;

        protected internal Result(bool isSuccess, T value, Error? error) : base(isSuccess, error)
        {
            _value = value;
        }

        public T Value =>
            IsSuccess
                ? _value!
                : throw new InvalidOperationException("There is no value for failure.");

        public static implicit operator T(Result<T> result) => result.Value;

        public static implicit operator Result<T>(T value) => new Result<T>(true, value, Error.None);
    }
}
