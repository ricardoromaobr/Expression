using System;
using System.IO;

namespace Expressao
{
	public class Buffer
	{
		private Stream stream;
		
		public Buffer (Stream stream)
		{
			this.stream = stream;
			this.stream.Seek (0, SeekOrigin.Begin);
			
			if (stream.Length == 0)
				throw new IOException ("Error reading expression");
		}

		public Buffer (string formula)
		{
			var arrayChar = formula.ToCharArray ();
			this.stream = new MemoryStream (ConvertCharToByte (arrayChar));
			if (stream.Length == 0)
				throw new IOException ("Error reading expression");
		}

		byte[] ConvertCharToByte (char[] charArray)
		{
			byte[] byteArray = new byte[charArray.Length];
			for (int i = 0; i <= charArray.Length - 1; i++)
				byteArray [i] = (byte) charArray [i];
			return byteArray;
		}
	
	
		public char LerUmCaracter ()
		{
			char result;
			int ch = stream.ReadByte ();
			
			if (ch == -1)
				result = '\0';
			else 
				result = (char) ch;
			
			return result;
		}
		
		public void Reset ()
		{
			stream.Seek(0,SeekOrigin.Begin);
		}
		
		public long Avancar (long deslocamento)
		{
			return stream.Seek (deslocamento,SeekOrigin.Current);
		}
		
		public long Retroceder (long deslocamento)
		{
			return stream.Seek (-deslocamento, SeekOrigin.Current);
		}
	}
}