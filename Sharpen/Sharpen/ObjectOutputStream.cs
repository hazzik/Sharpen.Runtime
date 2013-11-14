namespace Sharpen
{
	using System;
	using System.IO;

	public class ObjectOutputStream : OutputStream
	{
		private readonly BinaryWriter bw;

		public ObjectOutputStream (OutputStream os)
		{
			bw = new BinaryWriter (os.GetWrappedStream ());
		}

		public virtual void WriteInt (int i)
		{
			bw.Write (i);
		}

        public virtual void WriteBoolean(bool value)
	    {
	        bw.Write(value);
	    }

	    public void WriteObject(object value)
	    {
	        throw new NotImplementedException();
	    }

	    public void WriteByte(int value)
	    {
	        bw.Write((byte) value);
	    }
	}
}
