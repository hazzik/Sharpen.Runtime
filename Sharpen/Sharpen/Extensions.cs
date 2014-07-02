using System.Reflection;
using System.Net;
using System.Linq;
using System.Net.Sockets;
using System.Threading;
using System.Collections.Specialized;

namespace Sharpen
{
	using ICSharpCode.SharpZipLib.Zip.Compression;
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.Diagnostics;
	using System.Globalization;
	using System.IO;
	using System.Runtime.CompilerServices;
	using System.Text;
	using System.Text.RegularExpressions;

	public static class Extensions
	{
		private static readonly long EPOCH_TICKS;

		static Extensions ()
		{
			DateTime time = new DateTime (1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
			EPOCH_TICKS = time.Ticks;
		}
		
		public static void Add<T> (this IList<T> list, int index, T item)
		{
			list.Insert (index, item);
		}
		
		public static void AddFirst<T> (this IList<T> list, T item)
		{
			list.Insert (0, item);
		}
		
		public static void AddLast<T> (this IList<T> list, T item)
		{
			list.Add (item);
		}
		
		public static void RemoveLast<T> (this IList<T> list)
		{
			if (list.Count > 0)
				list.Remove (list.Count - 1);
		}

		public static StringBuilder AppendRange (this StringBuilder sb, string str, int start, int end)
		{
			return sb.Append (str, start, end - start);
		}

		public static StringBuilder Delete (this StringBuilder sb, int start, int end)
		{
			return sb.Remove (start, end - start);
		}

		public static void SetCharAt (this StringBuilder sb, int index, char c)
		{
			sb[index] = c;
		}

		public static int IndexOf (this StringBuilder sb, string str)
		{
			return sb.ToString ().IndexOf (str);
		}

		public static Iterable<T> AsIterable<T> (this IEnumerable<T> s)
		{
			return new EnumerableWrapper<T> (s);
		}

		public static int BitCount (int val)
		{
			uint num = (uint)val;
			int count = 0;
			for (int i = 0; i < 32; i++) {
				if ((num & 1) != 0) {
					count++;
				}
				num >>= 1;
			}
			return count;
		}

		public static string Decode(this Encoding e, byte[] chars, int start, int len)
		{
			try
			{
				byte[] bom = e.GetPreamble();
				if (bom != null && bom.Length > 0)
				{
					if (len >= bom.Length)
					{
						int pos = start;
                        bool hasBom = true;
                        for (int n = 0; n < bom.Length && hasBom; n++)
                        {
                            if (bom[n] != chars[pos++])
                                hasBom = false;
                        }
                        if (hasBom)
                        {
                            len -= pos - start;
                            start = pos;
                        }
                    }
                }
                return e.GetString(chars, start, len);
            }
            catch (DecoderFallbackException)
            {
                throw new CharacterCodingException();
            }
        }

        public static string Decode(this Encoding e, ByteBuffer buffer)
        {
            return e.Decode(buffer.Array(), buffer.ArrayOffset() + buffer.Position(), buffer.Limit() - buffer.Position());
        }

        private static UTF8Encoding UTF8Encoder = new UTF8Encoding(false, true);

        public static Encoding GetEncoding(string name)
        {
            //			Encoding e = Encoding.GetEncoding (name, EncoderFallback.ExceptionFallback, DecoderFallback.ExceptionFallback);
            try
            {
                Encoding e = Encoding.GetEncoding(name.Replace('_', '-'));
                if (e is UTF8Encoding)
                    return UTF8Encoder;
                return e;
            }
            catch (ArgumentException)
            {
                throw new UnsupportedCharsetException(name);
            }
        }

        public static ICollection<KeyValuePair<T, U>> EntrySet<T, U>(this IDictionary<T, U> s)
        {
            return s;
        }

	    public static void PutAll<T, U>(this IDictionary<T, U> d, IDictionary<T, U> values)
        {
            foreach (KeyValuePair<T, U> val in values)
                d[val.Key] = val.Value;
        }

        public static T GetLast<T>(this IList<T> list)
        {
            return ((list.Count == 0) ? default(T) : list[list.Count - 1]);
        }

        public static InputStream GetResourceAsStream(this Type type, string name)
        {
            string str2 = type.Assembly.GetName().Name + ".resources";
            string[] textArray1 = new string[] { str2, ".", type.Namespace, ".", name };
            string str = string.Concat(textArray1);
            Stream manifestResourceStream = type.Assembly.GetManifestResourceStream(str);
            if (manifestResourceStream == null)
            {
                return null;
            }
            return InputStream.Wrap(manifestResourceStream);
        }

        public static long GetTime(this DateTime dateTime)
        {
            return new DateTimeOffset(DateTime.SpecifyKind(dateTime, DateTimeKind.Utc), TimeSpan.Zero).ToMillisecondsSinceEpoch();
        }

        public static bool IsEmpty<T>(this ICollection<T> col)
        {
            return col.Count <= 0;
        }

        public static bool IsEmpty(this ICollection col)
        {
            return col.Count <= 0;
        }

        public static ListIterator<T> ListIterator<T>(this IList<T> col, int n)
        {
            return new ListIterator<T>(col, n);
        }

        public static DateTime CreateDate(long milliSecondsSinceEpoch)
        {
            long num = EPOCH_TICKS + (milliSecondsSinceEpoch*10000);
            return new DateTime(num);
        }

        public static DateTimeOffset MillisToDateTimeOffset(long milliSecondsSinceEpoch, long offsetMinutes)
        {
            TimeSpan offset = TimeSpan.FromMinutes(offsetMinutes);
            long num = EPOCH_TICKS + (milliSecondsSinceEpoch*10000);
            return new DateTimeOffset(num + offset.Ticks, offset);
        }

        public static T Remove<T>(this IList<T> list, int i)
        {
            T old = list[i];
            list.RemoveAt(i);
            return old;
        }

        public static string ReplaceAll(this string str, string regex, string replacement)
        {
            Regex rgx = new Regex(regex);

            if (replacement.IndexOfAny(new char[] { '\\', '$' }) != -1)
            {
                // Back references not yet supported
                StringBuilder sb = new StringBuilder();
                for (int n = 0; n < replacement.Length; n++)
                {
                    char c = replacement[n];
                    if (c == '$')
                        throw new NotSupportedException("Back references not supported");
                    if (c == '\\')
                        c = replacement[++n];
                    sb.Append(c);
                }
                replacement = sb.ToString();
            }

            return rgx.Replace(str, replacement);
        }

        public static bool RegionMatches(this string str, int toOffset, string other, int ooffset, int len)
        {
            return RegionMatches(str, false, toOffset, other, ooffset, len);
        }

        public static bool RegionMatches(this string str, bool ignoreCase, int toOffset, string other, int ooffset, int len)
        {
            return toOffset >= 0 && ooffset >= 0 && toOffset + len <= str.Length && ooffset + len <= other.Length && string.Compare(str, toOffset, other, ooffset, len, ignoreCase) == 0;
        }

	    public static T Set<T>(this IList<T> list, int index, T item)
        {
            T old = list[index];
            list[index] = item;
            return old;
        }

        public static void RemoveAll<T, U>(this ICollection<T> col, ICollection<U> items) where U : T
        {
            foreach (var u in items)
                col.Remove(u);
        }

        public static bool ContainsAll<T, U>(this ICollection<T> col, ICollection<U> items) where U : T
        {
            foreach (var u in items)
                if (!col.Any(n => (object.ReferenceEquals(n, u)) || n.Equals(u)))
                    return false;
            return true;
        }

        public static bool Contains<T>(this ICollection<T> col, T item)
        {
            if (!(item is T))
                return false;
            return col.Any(n => (object.ReferenceEquals(n, item)) || n.Equals(item));
        }

        public static string[] Split(this string str, string regex)
        {
            return str.Split(regex, 0);
        }

        public static string[] Split(this string str, string regex, int limit)
        {
            Regex rgx = new Regex(regex);
            List<string> list = new List<string>();
            int startIndex = 0;
            if (limit != 1)
            {
                int nm = 1;
                foreach (Match match in rgx.Matches(str))
                {
                    list.Add(str.Substring(startIndex, match.Index - startIndex));
                    startIndex = match.Index + match.Length;
                    if (limit > 0 && ++nm == limit)
                        break;
                }
            }
            if (startIndex < str.Length)
            {
                list.Add(str.Substring(startIndex));
            }
            if (limit >= 0)
            {
                int count = list.Count - 1;
                while ((count >= 0) && (list[count].Length == 0))
                {
                    count--;
                }
                list.RemoveRange(count + 1, (list.Count - count) - 1);
            }
            return list.ToArray();
        }

        public static IList<T> SubList<T>(this IList<T> list, int start, int len)
        {
            List<T> sublist = new List<T>(len);
            for (int i = start; i < (start + len); i++)
            {
                sublist.Add(list[i]);
            }
            return sublist;
        }

        public static long ToMillisecondsSinceEpoch(this DateTime dateTime)
        {
            if (dateTime.Kind != DateTimeKind.Utc)
            {
                throw new ArgumentException("dateTime is expected to be expressed as a UTC DateTime", "dateTime");
            }
            return new DateTimeOffset(DateTime.SpecifyKind(dateTime, DateTimeKind.Utc), TimeSpan.Zero).ToMillisecondsSinceEpoch();
        }

        public static long ToMillisecondsSinceEpoch(this DateTimeOffset dateTimeOffset)
        {
            return (((dateTimeOffset.Ticks - dateTimeOffset.Offset.Ticks) - EPOCH_TICKS)/TimeSpan.TicksPerMillisecond);
        }

        public static string ToHexString(int val)
        {
            return Convert.ToString(val, 16);
        }

        public static string ToString(object val)
        {
            return val.ToString();
        }

        public static HttpURLConnection OpenConnection(this Uri uri)
        {
            return new HttpsURLConnection(uri);
        }


        public static Uri Resolve(this Uri uri, string str)
        {
            //TODO: Check implementation
            return new Uri(uri, str);
        }
    }
}
