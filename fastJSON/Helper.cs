using System;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace fastJSON
{
    public class Helper
    {
        public static bool IsNullable(Type t)
        {
            if (!t.IsGenericType) return false;
            Type g = t.GetGenericTypeDefinition();
            return (g.Equals(typeof(Nullable<>)));
        }

        public static Type UnderlyingTypeOf(Type t)
        {
            return Reflection.Instance.GetGenericArguments(t)[0];
        }

        public static DateTimeOffset CreateDateTimeOffset(int year, int month, int day, int hour, int min, int sec, int milli, int extraTicks, TimeSpan offset)
        {
            var dt = new DateTimeOffset(year, month, day, hour, min, sec, milli, offset);

            if (extraTicks > 0)
                dt += TimeSpan.FromTicks(extraTicks);

            return dt;
        }

        public static bool BoolConv(object v)
        {
            bool oset = false;
            if (v is bool)
                oset = (bool)v;
            else if (v is long)
                oset = (long)v > 0 ? true : false;
            else if (v is string)
            {
                var s = (string)v;
                s = s.ToLowerInvariant();
                if (s == "1" || s == "true" || s == "yes" || s == "on")
                    oset = true;
            }

            return oset;
        }

        public static long AutoConv(object value, JSONParameters param)
        {
            if (value is string)
            {
                if (param.AutoConvertStringToNumbers == true)
                {
                    string s = (string)value;
                    return CreateLong(s, 0, s.Length);
                }
                else
                    throw new Exception("AutoConvertStringToNumbers is disabled for converting string : " + value);
            }
            else if (value is long)
                return (long)value;
            else
                return Convert.ToInt64(value);
        }

        public static unsafe long CreateLong(string s, int index, int count)
        {
            long num = 0;
            int neg = 1;
            fixed (char* v = s)
            {
                char* str = v;
                str += index;
                if (*str == '-')
                {
                    neg = -1;
                    str++;
                    count--;
                }
                if (*str == '+')
                {
                    str++;
                    count--;
                }
                while (count > 0)
                {
                    num = num * 10
                       //(num << 4) - (num << 2) - (num << 1) 
                       + (*str - '0');
                    str++;
                    count--;
                }
            }
            return num * neg;
        }

        public static unsafe long CreateLong(char[] s, int index, int count)
        {
            long num = 0;
            int neg = 1;
            fixed (char* v = s)
            {
                char* str = v;
                str += index;
                if (*str == '-')
                {
                    neg = -1;
                    str++;
                    count--;
                }
                if (*str == '+')
                {
                    str++;
                    count--;
                }
                while (count > 0)
                {
                    num = num * 10
                        //(num << 4) - (num << 2) - (num << 1) 
                        + (*str - '0');
                    str++;
                    count--;
                }
            }
            return num * neg;
        }

        public static unsafe int CreateInteger(string s, int index, int count)
        {
            int num = 0;
            int neg = 1;
            fixed (char* v = s)
            {
                char* str = v;
                str += index;
                if (*str == '-')
                {
                    neg = -1;
                    str++;
                    count--;
                }
                if (*str == '+')
                {
                    str++;
                    count--;
                }
                while (count > 0)
                {
                    num = num * 10
                        //(num << 4) - (num << 2) - (num << 1) 
                        + (*str - '0');
                    str++;
                    count--;
                }
            }
            return num * neg;
        }

        //public static int[] powof10 = new int[10]
        //{
        //    1,
        //    10,
        //    100,
        //    1000,
        //    10000,
        //    100000,
        //    1000000,
        //    10000000,
        //    100000000,
        //    1000000000
        //};
        //public static decimal ParseDecimal(string input)
        //{
        //    int len = input.Length;
        //    if (len != 0)
        //    {
        //        bool negative = false;
        //        long n = 0;
        //        int start = 0;
        //        if (input[0] == '-')
        //        {
        //            negative = true;
        //            start = 1;
        //        }
        //        if (len <= 19)
        //        {
        //            int decpos = len;
        //            for (int k = start; k < len; k++)
        //            {
        //                char c = input[k];
        //                if (c == '.')
        //                {
        //                    decpos = k + 1;
        //                }
        //                else
        //                {
        //                    n = (n * 10) + (int)(c - '0');
        //                }
        //            }
        //            return new decimal((int)n, (int)(n >> 32), 0, negative, (byte)(len - decpos));
        //        }
        //        else
        //        {
        //            if (len > 30)
        //            {
        //                len = 30;
        //            }
        //            int decpos = len;
        //            for (int k = start; k < 19; k++)
        //            {
        //                char c = input[k];
        //                if (c == '.')
        //                {
        //                    decpos = k + 1;
        //                }
        //                else
        //                {
        //                    n = (n * 10) + (int)(c - '0');
        //                }
        //            }
        //            int n2 = 0;
        //            bool secondhalfdec = false;
        //            for (int k = 19; k < len; k++)
        //            {
        //                char c = input[k];
        //                if (c == '.')
        //                {
        //                    decpos = k + 1;
        //                    secondhalfdec = true;
        //                }
        //                else
        //                {
        //                    n2 = (n2 * 10) + (int)(c - '0');
        //                }
        //            }
        //            byte decimalPosition = (byte)(len - decpos);
        //            return new decimal((int)n, (int)(n >> 32), 0, negative, decimalPosition) * powof10[len - (!secondhalfdec ? 19 : 20)] + new decimal(n2, 0, 0, negative, decimalPosition);
        //        }
        //    }
        //    return 0;
        //}

        public static object CreateEnum(Type pt, object v)
        {
            // FEATURE : optimize create enum
#if !SILVERLIGHT
            return Enum.Parse(pt, v.ToString(), true);
#else
            return Enum.Parse(pt, v, true);
#endif
        }

        public static Guid CreateGuid(string s)
        {
            if (s.Length > 30)
                return new Guid(s);
            else
                return new Guid(Convert.FromBase64String(s));
        }

        public static StringDictionary CreateSD(Dictionary<string, object> d)
        {
            StringDictionary nv = new StringDictionary();

            foreach (var o in d)
                nv.Add(o.Key, (string)o.Value);

            return nv;
        }

        public static NameValueCollection CreateNV(Dictionary<string, object> d)
        {
            NameValueCollection nv = new NameValueCollection();

            foreach (var o in d)
                nv.Add(o.Key, (string)o.Value);

            return nv;
        }

        public static object CreateDateTimeOffset(string value)
        {
            //                   0123456789012345678 9012 9/3 0/4  1/5
            // datetime format = yyyy-MM-ddTHH:mm:ss .nnn  _   +   00:00

            // ISO8601 roundtrip formats have 7 digits for ticks, and no space before the '+'
            // datetime format = yyyy-MM-ddTHH:mm:ss .nnnnnnn  +   00:00  
            // datetime format = yyyy-MM-ddTHH:mm:ss .nnnnnnn  Z  

            int year;
            int month;
            int day;
            int hour;
            int min;
            int sec;
            int ms = 0;
            int usTicks = 0; // ticks for xxx.x microseconds
            int th = 0;
            int tm = 0;

            year = CreateInteger(value, 0, 4);
            month = CreateInteger(value, 5, 2);
            day = CreateInteger(value, 8, 2);
            hour = CreateInteger(value, 11, 2);
            min = CreateInteger(value, 14, 2);
            sec = CreateInteger(value, 17, 2);

            int p = 20;

            if (value.Length > 21 && value[19] == '.')
            {
                ms = CreateInteger(value, p, 3);
                p = 23;

                // handle 7 digit case
                if (value.Length > 25 && char.IsDigit(value[p]))
                {
                    usTicks = CreateInteger(value, p, 4);
                    p = 27;
                }
            }

            if (value[p] == 'Z')
                // UTC
                return CreateDateTimeOffset(year, month, day, hour, min, sec, ms, usTicks, TimeSpan.Zero);

            if (value[p] == ' ')
                ++p;

            // +00:00
            th = CreateInteger(value, p + 1, 2);
            tm = CreateInteger(value, p + 1 + 2 + 1, 2);

            if (value[p] == '-')
                th = -th;

            return CreateDateTimeOffset(year, month, day, hour, min, sec, ms, usTicks, new TimeSpan(th, tm, 0));
        }

        public static DateTime CreateDateTime(string value, bool UseUTCDateTime)
        {
            if (value.Length < 19)
                return DateTime.MinValue;

            bool utc = false;
            //                   0123456789012345678 9012 9/3
            // datetime format = yyyy-MM-ddTHH:mm:ss .nnn  Z
            int year;
            int month;
            int day;
            int hour;
            int min;
            int sec;
            int ms = 0;

            year = CreateInteger(value, 0, 4);
            month = CreateInteger(value, 5, 2);
            day = CreateInteger(value, 8, 2);
            hour = CreateInteger(value, 11, 2);
            min = CreateInteger(value, 14, 2);
            sec = CreateInteger(value, 17, 2);
            if (value.Length > 21 && value[19] == '.')
                ms = CreateInteger(value, 20, 3);

            if (value[value.Length - 1] == 'Z')
                utc = true;

            if (UseUTCDateTime == false && utc == false)
                return new DateTime(year, month, day, hour, min, sec, ms);
            else
                return new DateTime(year, month, day, hour, min, sec, ms, DateTimeKind.Utc).ToLocalTime();
        }

        //private static readonly char CharNegative = '-';
        //private static readonly char CharPositive = '+';
        //private static readonly char CharDecimalSeparator = '.';// CurrentCulture.NumberFormat.NumberDecimalSeparator[0];
        ///// <summary>High performance double parser with rudimentary flexibility.</summary>
        ///// <returns>Returns true only if we can be certain we parsed the string correctly.</returns>
        ///// <remarks>Does not support thousand separators or whitespace.</remarks>
        ///// <remarks>Supports all culture-specific symbols specified by the NumberFormatInfo of the
        ///// <see cref="CultureInfo.CurrentCulture"/> at the time this static class is instantiated.
        ///// So long as all culture symbols are a single character in length.
        ///// TODO: In theory, this class could be made instantiable, take the culture as an argument,
        /////       and support changing the culture at runtime in case the file the user is uploading
        /////       was generated on a machine with different culture settings.</remarks>
        ///// <remarks>Supports leading negative signs and positive signs, scientific notation,
        ///// as well as Infinity, Negative Infinity, and NaN, string representations.</remarks>
        ///// <remarks>A string containing only a negative sign (usually "-") intentionally returns a
        ///// value of zero. This is because it's a common representation of 0 in accounting.</remarks>
        //public static bool FastTryParseDouble(string input, out double result)
        //{
        //    int length = input.Length;
        //    if (length <= 0)
        //    {
        //        result = Double.NaN;
        //        return false;
        //    }
        //    double sign = 1d;
        //    int currentIndex = 0;
        //    char nextChar = input[0];

        //    /**************** Sign (+/-) and Special Case String Representations *****************/
        //    // Handle all cases where the string does not start with a numeric character
        //    if (nextChar < '0' || nextChar > '9')
        //    {
        //        // Non-numeric 1-character strings must match one of the supported special cases.
        //        if (length == 1)
        //            return CheckForSpecialCaseDoubleStrings(input, out result);
        //        // For anything more than one character, this should be a sign character.
        //        if (nextChar == CharNegative)
        //            sign = -1d;
        //        // The very next character may also be the decimal separator.
        //        else if (nextChar == CharDecimalSeparator)
        //        {
        //            // In this case, we treat the integer part as 0 and skip to the fractional part.
        //            result = 0d;
        //            goto SkipIntegerPart;
        //        }
        //        // Finally, unless it was a '+' sign, input must match one of a set of special cases.
        //        else if (nextChar != CharPositive)
        //            return CheckForSpecialCaseDoubleStrings(input, out result);

        //        // Once the sign is consumed, advance to the next character for further parsing
        //        nextChar = input[unchecked(++currentIndex)];
        //        // We must once more check whether the character is numeric before proceeding.
        //        if (nextChar < '0' || nextChar > '9')
        //        {
        //            // If not numeric, at this point, the character can only be a decimal separator
        //            // (as in "-.123" or "+.123"), or else it must be part of a special case string
        //            // (as in "-∞"). So check for those.
        //            if (nextChar != CharDecimalSeparator)
        //                return CheckForSpecialCaseDoubleStrings(input, out result);
        //            result = 0d;
        //            goto SkipIntegerPart;
        //        }
        //    }

        //    /********************************** "Integer Part" ***********************************/
        //    // Treat all subsequent numeric characters as the "integer part" of the result.
        //    // Since we've already checked that the next character is numeric,
        //    // We can save 2 ops by initializing the result directly.
        //    unchecked
        //    {
        //        result = nextChar - '0';
        //        while (++currentIndex < length)
        //        {
        //            nextChar = input[currentIndex];
        //            if (nextChar < '0' || nextChar > '9') break;
        //            result = result * 10d + (nextChar - '0');
        //        }
        //    }

        //    // This label and corresponding goto statements is a performance optimization to
        //    // allow us to efficiently skip "integer part" parsing in cases like ".123456"
        //    // Please don't be mad.
        //    SkipIntegerPart:

        //    // The expected case is that the next character is a decimal separator, however
        //    // this section might be skipped in normal use cases (e.g. as in "1e18")
        //    // TODO: If we broke out of the while loop above due to reaching the end of the
        //    //       string, this operation is superfluous. Is there a way to skip it?
        //    if (nextChar == CharDecimalSeparator)
        //    {
        //        /******************************* "Fractional Part" *******************************/
        //        // Track the index at the start of the fraction part.
        //        unchecked
        //        {
        //            int fractionPos = ++currentIndex;
        //            // Continue shifting and adding to the result as before
        //            do
        //            {
        //                nextChar = input[currentIndex];
        //                // Note that we flip the OR here, because it's now more likely that
        //                // nextChar > '9' ('e' or 'E'), leading to an early exit condition.
        //                if (nextChar > '9' || nextChar < '0') break;
        //                result = result * 10d + (nextChar - '0');
        //            } while (++currentIndex < length);

        //            // Update this to store the number of digits in the "fraction part".
        //            // We will use this to adjust down the magnitude of the double.
        //            fractionPos = currentIndex - fractionPos;
        //            // Use our tiny array of negative powers of 10 if possible, but fallback to
        //            // our larger array (still fast), whose higher indices store negative powers.
        //            // Finally, while practically unlikely, ridiculous strings (>300 characters)
        //            // can still be supported with a final fallback to native Math.Pow
        //            // TODO: Is it possible to combine this magnitude adjustment with any
        //            //       applicable adjustment due to scientific notation?
        //            result *= fractionPos < NegPow10Length ?
        //                NegPow10[fractionPos] : fractionPos < MaxDoubleExponent ?
        //                Pow10[MaxDoubleExponent + fractionPos] : Math.Pow(10, -fractionPos);
        //        }
        //    }

        //    // Apply the sign now that we've added all digits that belong to the significand
        //    result *= sign;
        //    // If we have consumed every character in the string, return now.
        //    if (currentIndex >= length) return true;

        //    // The next character encountered must be an exponent character
        //    if (nextChar != 'e' && nextChar != 'E')
        //        return false;

        //    /**************************** "Scientific Notation Part" *****************************/
        //    unchecked
        //    {
        //        // If we're at the end of the string (last character was 'e' or 'E'), that's an error
        //        if (++currentIndex >= length) return false;
        //        // Otherwise, advance the current character and begin parsing the exponent
        //        nextChar = input[currentIndex];
        //        bool exponentIsNegative = false;
        //        // The next character can only be a +/- sign, or a numeric character
        //        if (nextChar < '0' || nextChar > '9')
        //        {
        //            if (nextChar == CharNegative)
        //                exponentIsNegative = true;
        //            else if (nextChar != CharPositive)
        //                return false;
        //            // Again, require there to be at least one more character in the string after the sign
        //            if (++currentIndex >= length) return false;
        //            nextChar = input[currentIndex];
        //            // And verify that this next character is numeric
        //            if (nextChar < '0' || nextChar > '9') return false;
        //        }

        //        // Since we know the next character is a digit, we can initialize the exponent int
        //        // directly and avoid 2 wasted ops (multiplying by and adding to zero).
        //        int exponent = nextChar - '0';
        //        // Shift and add any additional digit characters
        //        while (++currentIndex < length)
        //        {
        //            nextChar = input[currentIndex];
        //            // If we encounter any non-numeric characters now, it's definitely an error
        //            if (nextChar < '0' || nextChar > '9') return false;
        //            exponent = exponent * 10 + nextChar - '0';
        //        }
        //        // Apply the exponent. If negative, our index jump is a little different.
        //        if (exponentIsNegative)
        //            result *= exponent < Pow10Length - MaxDoubleExponent ?
        //                // Fallback to Math.Pow if the lookup array doesn't cover it.
        //                Pow10[exponent + MaxDoubleExponent] : Math.Pow(10, -exponent);
        //        // If positive, our array covers all possible positive exponents - ensure its valid.
        //        else if (exponent > MaxDoubleExponent)
        //            return false;
        //        else
        //            result *= Pow10[exponent];
        //    }
        //    // Doubles that underwent scientific notation parsing should be checked for overflow
        //    // (Otherwise, this isn't really a risk we don't expect strings of >308 characters)
        //    return !Double.IsInfinity(result);
        //}

        ///// <summary>Checks if the string matches one of a few supported special case
        ///// double strings. If so, assigns the result and returns true.</summary>
        //public static bool CheckForSpecialCaseDoubleStrings(string input, out double result)
        //{
        //    if (input == NumberFormat.PositiveInfinitySymbol)
        //        result = Double.PositiveInfinity;
        //    else if (input == NumberFormat.NegativeInfinitySymbol)
        //        result = Double.NegativeInfinity;
        //    else if (input == NumberFormat.NaNSymbol)
        //        result = Double.NaN;
        //    // Special Case: Excel has been known to format zero as "-".
        //    // We intentionally support it by returning zero now (most parsers would not)
        //    else if (input == NumberFormat.NegativeSign)
        //        result = 0d;
        //    // Special Case: Our organization treats the term "Unlimited" as referring
        //    // to Double.MaxValue (most parsers would not)
        //    else if (input.Equals("unlimited", StringComparison.OrdinalIgnoreCase))
        //        result = Double.MaxValue;
        //    // Anything else is not a valid input
        //    else
        //    {
        //        result = Double.NaN;
        //        return false;
        //    }
        //    return true;
        //}

        ///// <summary>The largest exponent (or smallest when negative) that can be given to a Double.</summary>
        //private const int MaxDoubleExponent = 308;

        ///// <summary>The number of elements that will be generated in the Pow10 array.</summary>
        //private const int Pow10Length = MaxDoubleExponent * 2 + 1;

        ///// <summary>A cache of all possible positive powers of 10 that might be required to
        ///// apply an exponent to a double (Indices 0-308), as well as the first 308 negative
        ///// exponents. (Indices 309-616)</summary>
        //private static readonly double[] Pow10 =
        //    Enumerable.Range(0, MaxDoubleExponent + 1).Select(i => Math.Pow(10, i))
        //        .Concat(Enumerable.Range(1, MaxDoubleExponent).Select(i => Math.Pow(10, -i)))
        //        .ToArray();

        ///// <summary>The number of negative powers to pre-compute and store in a small array.</summary>
        //private const int NegPow10Length = 16;

        ///// <summary>A cache of the first 15 negative powers of 10 for quick
        ///// magnitude adjustment of common parsed fractional parts of doubles.</summary>
        ///// <remarks>Even though this overlaps with the Pow10 array, it is kept separate so that
        ///// users that don't use scientific notation or extremely long fractional parts
        ///// might get a speedup by being able to reference the smaller array, which has a better
        ///// chance of being served out of L1/L2 cache.</remarks>
        //private static readonly double[] NegPow10 =
        //    Enumerable.Range(0, NegPow10Length).Select(i => Math.Pow(10, -i)).ToArray();
    }
}
