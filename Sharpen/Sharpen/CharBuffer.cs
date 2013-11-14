namespace Sharpen
{
	using System;

	public class CharBuffer : CharSequence
	{
		public string Wrapped;

		public override string ToString ()
		{
			return Wrapped;
		}

		public static CharBuffer Wrap (string str)
		{
			CharBuffer buffer = new CharBuffer ();
			buffer.Wrapped = str;
			return buffer;
		}

		public override int Length
		{
			get { return Wrapped.Length; }
		}

		public override char this[int i]
		{
			get { return Wrapped[i]; }
		}
	}
}
