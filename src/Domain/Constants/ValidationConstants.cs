using System;
using System.Collections.Generic;
using System.Text;

namespace CleanArchitecture.Domain.Constants;

public static class ValidationConstants
{
    public const int PhoneMinLength = 10;
    public const int PhoneMaxLength = 15;
    public const int NameMinLength = 2;
    public const int NameMaxLength = 100;
    public const int PasswordMinLength = 6;
    public const int ReviewCommentMaxLength = 1000;
    public const int MinRating = 1;
    public const int MaxRating = 5;
}
