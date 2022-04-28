// Copyright (c) Toni Kalajainen 2022
namespace Avalanche.Message;
using System;

/// <summary>Validation exception</summary>
public class ValidationException : AggregateException
{
    /// <summary></summary>
    public ValidationException() : base() { }
    /// <summary></summary>
    public ValidationException(String? message) : base(message) { }
    /// <summary></summary>
    public ValidationException(String? message, Exception innerException) : base(message, innerException) { }
    /// <summary></summary>
    public ValidationException(String? message, IEnumerable<Exception> innerException) : base(message, innerException) { }
    /// <summary></summary>
    public ValidationException(String? message, params Exception[] innerException) : base(message, innerException) { }
    /// <summary></summary>
    public ValidationException(IEnumerable<Exception> innerExceptions) { }
    /// <summary></summary>
    public ValidationException(params Exception[] innerExceptions) { }
}

