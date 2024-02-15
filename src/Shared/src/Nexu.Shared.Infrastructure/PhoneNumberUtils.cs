using System;
using PhoneNumbers;

namespace Nexu.Shared.Infrastructure
{
    public static class PhoneNumberUtils
    {
        private static readonly PhoneNumberUtil PhoneNumberU = PhoneNumberUtil.GetInstance();

        /// <summary>
        /// Determines whether the provided <paramref name="phoneNumber"/> is valid, optionally checking if it
        /// corresponds to the provided <paramref name="countryCode"/>.
        /// </summary>
        /// <param name="phoneNumber">The E.164 phone number.</param>
        /// <param name="countryCode">The 2-letter ISO country code.</param>
        /// <returns></returns>
        public static bool IsValid(string phoneNumber, string countryCode = null)
        {
            if (phoneNumber is null)
            {
                throw new ArgumentNullException(nameof(phoneNumber));
            }

            if (!phoneNumber.StartsWith("+"))
            {
                return false;
            }
            try
            {
                var n = PhoneNumberU.Parse(phoneNumber, null);
                if (countryCode != null)
                {
                    return PhoneNumberU.IsValidNumberForRegion(n, countryCode);
                }

                return PhoneNumberU.IsValidNumber(n);
            }
            catch (NumberParseException)
            {
                return false;
            }
        }

        /// <summary>
        /// Gets the country 2-letter ISO code for the given number. Note that the number must be previously validated via
        /// <see cref="IsValid(string)"/> method, or this invocation may throw a <see cref="NumberParseException" />.
        /// </summary>
        /// <param name="phoneNumber">The E.164 phone number.</param>
        public static string GetCountryIsoCode(string phoneNumber)
        {
            var phone = PhoneNumberU.Parse(phoneNumber, null);
            return PhoneNumberU.GetRegionCodeForCountryCode(phone.CountryCode);
        }
    }
}
