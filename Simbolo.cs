using System;

namespace Expressao
{
	public class Simbolo
	{
		public Token token;
		public CategoriaToken categoriaToken;
		
		public Simbolo (Token token, CategoriaToken categToken)
		{
			this.token = token;
			this.categoriaToken = categToken;
		}
	}
}