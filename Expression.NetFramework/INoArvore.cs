using System;
namespace Expressao
{
	public interface INoArvore
	{
		Token Token { get; }
		
		TipoExpressao TipoExpressao { get; }
		
		TipoOperador TipoOperador { get; }
		
		int NumeroFilhos { get; }
	}
}