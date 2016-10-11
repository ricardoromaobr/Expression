using System;
namespace Expressao
{
	public struct Token
	{
		private Tokens token;
		private string lexema;
		private ulong linha;
		private int coluna; 
		
		public Token (Tokens token, string lexema, ulong linha, int coluna)
		{
			this.token = token;
			this.lexema = lexema;
			this.linha = linha;
			this.coluna = coluna;
		}
		
		public Tokens TipoToken {
			get { return token; }
		}
		
		public string Lexema {
			get { return lexema; }
		}
		
		public ulong Linha {
			get { return linha; }
		}
		
		public int Coluna {
			get { return coluna; }
		}
	}
}