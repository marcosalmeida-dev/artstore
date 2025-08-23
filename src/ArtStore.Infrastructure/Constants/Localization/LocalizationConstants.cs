// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace ArtStore.Infrastructure.Constants.Localization;

public static class LocalizationConstants
{
    public const string ResourcesPath = "Resources";
    /// <summary>
    /// Default language code. Set to English (en-US). 
    /// </summary>
    public const string DefaultLanguageCode = "en-US";

    public static readonly LanguageCode[] SupportedLanguages =
    {
        new()
        {
            Code = "en-US",
            DisplayName = "English (United States)"
        },
        new()
        {
            Code = "pt-BR",
            DisplayName = "Português (Brazil)"
        },
        new()
        {
            Code = "es-AR",
            DisplayName = "español (Argentina)"
        }
    };
}

public class LanguageCode
{
    public string DisplayName { get; set; } = "en-US";
    public string Code { get; set; } = "English";
}