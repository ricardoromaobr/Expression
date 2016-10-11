using System;
namespace Expressao
{
	public enum TipoExpressao
	{
		Expressao,
		ExpressaoLogica,
		Numero,
		Operador,
		Identificador,
		Constante
	}
	
	public enum TipoOperador
	{
		Add,
		Sub,
		Mult,
		Div,
		Exp,
		Mod,
		Cientifico,
		OperadorLogico,
		Nenhum
	}
}

