using System;

namespace Expressao
{
	public class DeslocamentoInvalidoException : Exception
	{
		private string mensagem;
		
		public DeslocamentoInvalidoException (string mensagem)
		{
			this.mensagem = mensagem;
		}
		public override string Message {
			get {
				return this.ToString () + ":" + mensagem;
			}
		}
	}
}