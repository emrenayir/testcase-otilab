using UnityEngine;

namespace Helper
{
    /// <summary>
    /// Static helper class for formatting numbers in a consistent way across the game
    /// </summary>
    public static class NumberFormatter
    {
        /// <summary>
        /// Formats a number to a readable string with K/M suffixes
        /// Example: 2048 becomes "2K", 262144 becomes "262.1K", and 1,000,000 becomes "1M"
        /// </summary>
        public static string FormatNumber(int value)
        {
            if (value >= 1000000)
            {
                float millions = value / 1000000f;
                return $"{millions:0.#}M";
            }
            else if (value >= 1000)
            {
                float thousands = value / 1000f;
                return $"{thousands:0.#}K";
            }
            else
            {
                return value.ToString();
            }
        }
    }
}