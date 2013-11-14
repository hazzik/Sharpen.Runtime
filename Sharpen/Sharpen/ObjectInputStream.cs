namespace Sharpen
{
	using System;
	using System.IO;

	public class ObjectInputStream : InputStream
	{
		private BinaryReader reader;

		public ObjectInputStream (InputStream s)
		{
		    this.reader = new BinaryReader(s.GetWrappedStream());
		}

		public int ReadInt ()
		{
			return this.reader.ReadInt32 ();
		}

		public object ReadObject ()
		{
			throw new NotImplementedException ();
		}

	    protected virtual object ResolveObject(object obj)
	    {
	        throw new NotImplementedException();
	    }

	    protected Type ResolveClass(ObjectStreamClass desc)
	    {
	        throw new NotImplementedException();
	    }

	    public bool ReadBoolean()
	    {
	        return reader.ReadBoolean();
	    }

	    public int ReadByte()
	    {
	        return reader.ReadByte();
	    }

	    public int ReadShort()
	    {
	        return reader.ReadInt16();
	    }
	}

    public class ObjectStreamClass
    {
    }
}
