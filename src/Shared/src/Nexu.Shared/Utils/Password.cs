using System;

namespace Nexu.Shared.Utils
{
    public class Password
    {
        /// <summary>
        /// Generate a password RANDOM with length populate from method
        /// </summary>
        /// <param name="length">Length to output password</param>
        /// <returns>Password RANDOM</returns>
        public static string GenerateByLength(int length)
        {
            string password = string.Empty;
            string[] character = { "a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l", "m", "n", "o", "p", "q", "r", "s", "t", "u", "v", "w", "x", "y", "z",
                                   "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z"};
            Random Randomize = new Random();

            for (int i = 0; i < length; i++)
            {
                int characterRandom = Randomize.Next(0, 100);
                int NumberRadom = Randomize.Next(0, 9);

                if (characterRandom < character.Length)
                {
                    password += character[characterRandom];
                }
                else
                {
                    password += NumberRadom.ToString();
                }
            }
            return "G#" + password;
        }
    }
}
