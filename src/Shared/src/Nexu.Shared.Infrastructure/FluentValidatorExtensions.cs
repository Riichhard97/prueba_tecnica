using System;
using System.Collections.Generic;
using System.Linq;
using FluentValidation;
using Nexu.Shared.Properties;

namespace Nexu.Shared.Infrastructure
{
    public static class FluentValidatorExtensions
    {
        private static readonly HashSet<string> LanguageNames = new()
        {
            Languages.English,
            Languages.Spanish,
            Languages.Japanese,
        };

        public static IRuleBuilderOptions<T, ICollection<TElement>> ListNotEmpty<T, TElement>(this IRuleBuilder<T, ICollection<TElement>> ruleBuilder)
        {
            return ruleBuilder.Must(list => list is null || list.Count > 0);
        }

        public static IRuleBuilderOptions<T, ICollection<TElement>> ListMaxCount<T, TElement>(this IRuleBuilder<T, ICollection<TElement>> ruleBuilder, int count)
        {
            if (count < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(count));
            }

            return ruleBuilder.Must(list => list.Count <= count);
        }

        public static IRuleBuilderOptions<T, ICollection<TElement>> ListMinCount<T, TElement>(this IRuleBuilder<T, ICollection<TElement>> ruleBuilder, int count)
        {
            if (count < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(count));
            }

            return ruleBuilder.Must(list => list is null || list.Count >= count);
        }

        public static IRuleBuilderOptions<T, IList<TElement>> UniqueValues<T, TElement>(
            this IRuleBuilder<T, IList<TElement>> ruleBuilder, IEqualityComparer<TElement> comparer = null)
        {
            return ruleBuilder.Must(list => list.GroupBy(x => x, comparer).All(x => x.Count() == 1));
        }

        public static IRuleBuilderOptions<T, IList<TElement>> UniqueValueFor<T, TElement, TKey>(
            this IRuleBuilder<T, IList<TElement>> ruleBuilder, Func<TElement, TKey> keySelector, IEqualityComparer<TKey> comparer = null)
        {
            return ruleBuilder.Must(list => list.GroupBy(keySelector, comparer).All(x => x.Count() == 1));
        }

        public static IRuleBuilderOptions<T, string> AbsoluteUrl<T>(this IRuleBuilder<T, string> ruleBuilder)
        {
            return ruleBuilder.Must(BeAnAbsoluteUrl)
                .WithMessage(_ => Resources.InvalidUrl);
        }

        public static IRuleBuilderOptions<T, string> Language<T>(this IRuleBuilder<T, string> ruleBuilder)
        {
            return ruleBuilder.Must(BeAValidLanguage)
                .WithMessage(_ => Resources.InvalidLanguage);
        }

        public static IRuleBuilderOptions<T, string> ServiceName<T>(this IRuleBuilder<T, string> ruleBuilder)
        {
            return ruleBuilder.Must(BeAValidServiceName)
                .WithMessage(_ => Resources.InvalidLanguage);
        }

        public static IRuleBuilderOptions<T, string> HexColor<T>(this IRuleBuilder<T, string> ruleBuilder)
        {
            return ruleBuilder.Must(BeAValidHexColor)
                .WithMessage(_ => Resources.InvalidColor);
        }

        public static IRuleBuilder<T, string> PhoneNumber<T>(this IRuleBuilder<T, string> ruleBuilder)
        {
            return ruleBuilder
                .MaximumLength(AppConstants.PhoneNumberLength)
                .Must(BeAValidPhoneNumber)
                .WithMessage(_ => Resources.InvalidPhoneNumber);
        }

        public static IRuleBuilder<T, string> TimeZone<T>(this IRuleBuilder<T, string> ruleBuilder)
        {
            return ruleBuilder.Must(timeZone => string.IsNullOrEmpty(timeZone) || TimeZoneExtensions.IsTimeZone(timeZone))
                .WithMessage(_ => Resources.InvalidTimeZone);
        }

        private static bool BeAValidPhoneNumber(string phoneNumber)
        {
            if (string.IsNullOrEmpty(phoneNumber))
            {
                return true;
            }

            return PhoneNumberUtils.IsValid(phoneNumber);
        }

        private static bool BeAValidHexColor(string arg)
        {
            if (arg is null)
            {
                return true;
            }
            if (arg.IndexOf("#") != 0)
            {
                return false;
            }

            var code = arg.Substring(1);
            if (code.Length == 3 || code.Length == 6)
            {
                return code.All(IsHexadecimal);
            }

            return false;
        }

        private static bool IsHexadecimal(char c)
        {
            return char.IsDigit(c) || (c >= 'A' && c <= 'F') || (c >= 'a' && c <= 'f');
        }

        private static bool BeAValidLanguage(string arg)
        {
            if (arg is null)
            {
                return true;
            }

            return LanguageNames.Contains(arg);
        }

        private static bool BeAValidServiceName(string arg)
        {
            if (arg is null)
            {
                return true;
            }

            return ServiceNames.IsServiceName(arg);
        }

        private static bool BeAnAbsoluteUrl(string url)
        {
            if (url is null)
            {
                return true;
            }

            return Uri.IsWellFormedUriString(url, UriKind.Absolute)
                // Validate protocol is http or https
                && url.StartsWith("http", StringComparison.OrdinalIgnoreCase);
            //try
            //{
            //    new Uri(url, UriKind.Absolute);
            //    return true;
            //}
            //catch (UriFormatException)
            //{
            //    return false;
            //}
        }
    }
}
