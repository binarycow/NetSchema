#nullable enable

using System;
using NetSchema.Common;

// ReSharper disable once CheckNamespace
namespace NetSchema.Data
{
    internal static class ResultExtensions
    {
        public static Result<TTo> Convert<TFrom, TTo>(Result<TFrom> result) 
            where TFrom : TTo 
            where TTo : notnull 
            => result.Try(out var value, out var error)
                ? value
                : (Result<TTo>)error;
    }
}