using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

#nullable enable

namespace NetSchema.Common
{
    public readonly struct Result : IEquatable<Result>
    {
        public static readonly Result SuccessfulResult = new (true, null, null);
        
        public bool IsSuccess { get; }
        public bool IsError => !this.IsSuccess;
        public string? ErrorMessage { get; }
        public string? ErrorAppTag { get; }

        public bool Try(
            out string? errorMessage, 
            out string? errorAppTag
        )
        {
            (errorMessage, errorAppTag) = this;
            return this.IsSuccess;
        }
        public bool Try(
            out Result error
        )
        {
            error = this;
            return this.IsSuccess;
        }

        public void Deconstruct(
            out string? errorMessage, 
            out string? errorAppTag
        )
        {
            errorMessage = this.ErrorMessage;
            errorAppTag = this.ErrorAppTag;
        }
        
        private Result(bool isSuccess, string? errorMessage, string? errorAppTag)
        {
            this.IsSuccess = isSuccess;
            this.ErrorMessage = errorMessage;
            this.ErrorAppTag = errorAppTag;
        }

        public override string ToString()
        {
            return (this.IsSuccess, this.ErrorMessage, this.ErrorAppTag) switch
            {
                (true, _, _) => "Success",
                (_, null, null) => "Error",
                (_, _, null) => $"Error: {this.ErrorMessage}",
                (_, null, _) => $"Error: {this.ErrorAppTag}",
                _ => $"Error: {this.ErrorMessage} ({this.ErrorAppTag})"
            };
        }


        public static implicit operator Result(Exception exception) => Result.CreateError(exception.Message);
        public static implicit operator bool(Result value) => value.IsSuccess;
        public bool Equals(Result other) => this.IsSuccess == other.IsSuccess && this.ErrorMessage == other.ErrorMessage && this.ErrorAppTag == other.ErrorAppTag;
        public override bool Equals(object? obj) => obj is Result other && this.Equals(other);
        public override int GetHashCode() => HashCode.Combine(this.IsSuccess, this.ErrorMessage, this.ErrorAppTag);
        public static bool operator ==(Result left, Result right) => left.Equals(right);
        public static bool operator !=(Result left, Result right) => !left.Equals(right);

        public static Result CreateError(string? errorMessage = null, string? errorAppTag = null) => new (isSuccess: false, errorMessage, errorAppTag);
        public Result<T> SuccessOrError<T>(T value) where T : notnull 
            => this.IsError 
                ? Result<T>.CreateError(this.ErrorMessage, this.ErrorAppTag)
                : Result<T>.CreateSuccess(value);

        public static Result<T> CreateSuccess<T>(T value) where T : notnull => Result<T>.CreateSuccess(value);
    }
    
    public readonly struct Result<T> : IEquatable<Result<T>> where T : notnull
    {
        public T? Value { get; }
        public string? ErrorMessage { get; }
        public string? ErrorAppTag { get; }

        public bool IsSuccess => this.Value is not null;
        public bool IsError => this.Value is null;

        public bool Try(
            [NotNullWhen(true)] out T? value
        )
        {
            value = this.Value;
            return value is not null;
        }
        
        public bool Try(
            [NotNullWhen(true)] out T? value, 
            out Result error
        )
        {
            value = this.Value;
            error = value is null 
                ? Result.CreateError(this.ErrorMessage, this.ErrorAppTag) 
                : Result.SuccessfulResult;
            return value is not null;
        }

        public Result(T value) : this(value ?? throw new ArgumentNullException(nameof(value)), null, null)
        {
            
        }
        private Result(T? value, string? errorMessage, string? errorAppTag)
        {
            this.Value = value;
            this.ErrorMessage = errorMessage;
            this.ErrorAppTag = errorAppTag;
        }

        public static implicit operator Result<T>(T value) => Result<T>.CreateSuccess(value);
        public static implicit operator T?(Result<T> value) => value.Value;
        public static implicit operator bool(Result<T> value) => value.IsSuccess;

        public static explicit operator Result<T>(Result value)
        {
            return value.IsSuccess
                ? throw new ArgumentException($"{nameof(value)} is a successful result, cannot convert to {nameof(Result<T>)}", nameof(value))
                : Result<T>.CreateError(value.ErrorMessage, value.ErrorAppTag);
        }

        public static implicit operator Result(Result<T> value)
        {
            return value.IsSuccess
                ? Result.SuccessfulResult
                : Result.CreateError(value.ErrorMessage, value.ErrorAppTag);
        }
        
        public override string ToString()
        {
            return (this.Value, this.ErrorMessage, this.ErrorAppTag) switch
            {
                (null, null, null) => "Error",
                (_, null, null) => $"{this.Value}",
                (_, _, null) => $"Error: {this.ErrorMessage}",
                (_, null, _) => $"Error: {this.ErrorAppTag}",
                _ => $"Error: {this.ErrorMessage} ({this.ErrorAppTag})"
            };
        }

        public bool Equals(Result<T> other) => EqualityComparer<T?>.Default.Equals(this.Value, other.Value) && this.ErrorMessage == other.ErrorMessage && this.ErrorAppTag == other.ErrorAppTag;
        public override bool Equals(object? obj) => obj is Result<T> other && this.Equals(other);
        public override int GetHashCode() => HashCode.Combine(this.Value, this.ErrorMessage, this.ErrorAppTag);
        public static bool operator ==(Result<T> left, Result<T> right) => left.Equals(right);
        public static bool operator !=(Result<T> left, Result<T> right) => !left.Equals(right);
        public static Result<T> CreateSuccess(T value) => new (value, null, null);
        public static Result<T> CreateError(string? errorMessage = null, string? errorAppTag = null) => new (default, errorMessage, errorAppTag);
    }


    public static class ResultExtensions
    {
        public static Result All(this IEnumerable<Result> results)
        {
            foreach (var result in results)
            {
                if (!result.Try(out var error))
                    return error;
            }
            return Result.SuccessfulResult;
        }
    }
}