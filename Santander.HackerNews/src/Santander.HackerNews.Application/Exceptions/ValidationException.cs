﻿namespace Santander.HackerNews.Application.Exceptions;
public class ValidationException : ApplicationException
{
    public ValidationException(string? message) : base(message)
    {
    }
}
