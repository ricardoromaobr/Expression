using System;

namespace Expressao
{
	public class NoArvore : INoArvore
	{
		private Token token;
		private TipoExpressao tipoExpressao;
		private TipoOperador tipoOperador;
			
		public NoArvore (Token token, TipoExpressao tipoExpressao, TipoOperador tipoOperador)
		{
			this.token = token;
			this.tipoExpressao = tipoExpressao;
			this.tipoOperador = tipoOperador;
		}
		
		public Token Token {
			get { return token; }
		}
		
		public TipoExpressao TipoExpressao { 
			get { return tipoExpressao; }
		}
		
		public TipoOperador TipoOperador { 
			get { return tipoOperador; }
		}
		
		public int NumeroFilhos {
			get { return 0 ;}
		}
	}
}