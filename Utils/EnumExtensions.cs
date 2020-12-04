using System;

namespace AdventOfCode.Utils
{
    public static class EnumExtensions
    {
        public static TEnum ParseOrDefault<TEnum>(string value, TEnum defaultValue) where TEnum : struct, IConvertible
        {
            if (!typeof(TEnum).IsEnum)
            {
                throw new ArgumentException($"{typeof(TEnum).Name} is not an Enum type.");
            }

            try
            {
                if(string.IsNullOrWhiteSpace(value))
                    return default(TEnum);
                return Enum.Parse<TEnum>(value);
            }
            catch
            {
                return defaultValue;
            }
        }
    }
}