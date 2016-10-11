using System;
using System.Collections;
using System.Linq;

namespace Expressao
{
	public class TabelaSimbolo
	{
		private Hashtable tabelaSimbolo;
		
		public TabelaSimbolo (Hashtable tabela)
		{
			tabelaSimbolo = tabela;
		}
		
		public Simbolo ObterSimbolo (string stringToken)
		{
			string key = null;
			Simbolo result = null;
			
			foreach (string k in tabelaSimbolo.Keys) {
				
				if (k == stringToken) {
					key = k;
					break;
				}
			}
			
			if (key != null)
				result = (Simbolo) tabelaSimbolo [key];
		
			return result;
		}
		
		public void AdicionarSimbolo (object key, object value)
		{
			tabelaSimbolo.Add (key, value);
		}
		
		public Hashtable TabSimbolo {
			get { 
				return (Hashtable) tabelaSimbolo.Clone (); 
			}
		}
		
		public Simbolo [] Variaveis {
			get {

				Simbolo [] lstVariaveis = new Simbolo [this.tabelaSimbolo.Values.Count];
				this.tabelaSimbolo.Values.CopyTo (lstVariaveis,0);
								
				var variaveis = 
					from v in lstVariaveis
						where v.categoriaToken == CategoriaToken.Variavel
						select v;
				
				var rst = variaveis.ToArray ();
				
				return rst;
			}
			     
		}
	}
}