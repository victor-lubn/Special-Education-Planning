using System;

namespace SpecialEducationPlanning
.Business.BusinessCore
{
    public struct CoreResult<T, TError>
    {
        public bool IsSuccessful { get; }
        public T Value { get; }
        public TError Exception { get; }

        public static CoreResult<T, TError> Success(T value) => new CoreResult<T, TError>(value);

        public static CoreResult<T, TError> Error(TError exception) => new CoreResult<T, TError>(exception);

        public CoreResult(T value, TError exception, bool isSuccessful)
        {
            IsSuccessful = isSuccessful;
            Value = value;
            Exception = exception;
        }

        public CoreResult(TError exception)
        {
            Value = default(T);
            IsSuccessful = false;
            Exception = exception;
        }
        public CoreResult(T value)
        {
            IsSuccessful = true;
            Value = value;
            Exception = default(TError);
        }

        public void Match(Action<T> success, Action<TError> error)
        {
            if (IsSuccessful)
            {
                success(Value);
            }
            else
            {
                error(Exception);
            }
        }

        public TResult Match<TResult>(Func<T, TResult> success, Func<TError, TResult> error) => IsSuccessful ? success(Value) : error(Exception);

        public CoreResult<TResult, TError> Map<TResult>(Func<T, TResult> mapping) =>
            Match(
                success: value => CoreResult<TResult, TError>.Success(mapping(value)),
                error: exception => CoreResult<TResult, TError>.Error(exception)
            );

        public CoreResult<TResult, TError> FlatMap<TResult>(Func<T, CoreResult<TResult, TError>> mapping) =>
            Match(
                success: mapping,
                error: exception => CoreResult<TResult, TError>.Error(exception)
                );
    }
}
