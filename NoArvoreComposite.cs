/* expressoes matematica com no maximo dois filhos
 * entao noArvore.Count tera no maximo 3 filhos 
 * exmplo  a + b  
 * 				+
 * 				:
 * 			   / \
 *  		  a    b
 * 
 * Autor: Ricardo Romao Soares
 */

using System;
using System.Collections.Generic;

namespace Expressao
{
	
	public class NoArvoreComposite : INoArvore
	{
		
		private List<INoArvore> noArvore; 
		private int indice = 0;
		private bool temExpressaoLogica = false;
			
		public NoArvoreComposite (INoArvore no)
		{
			noArvore = new List<INoArvore> (); 
			noArvore.Add (no);
			if (!temExpressaoLogica)
			if (no.TipoExpressao == TipoExpressao.ExpressaoLogica)
				temExpressaoLogica = true;
		}
		
		public Token Token {
			get {
				return noArvore [indice].Token;
			}
		}
		
		public TipoExpressao TipoExpressao {
			get {
				return noArvore[indice].TipoExpressao;
			}
		}
		
		public TipoOperador TipoOperador {
			get {
				return noArvore[indice].TipoOperador;
			}
		}
		
		public void Add (INoArvore noArvore)
		{
			this.noArvore.Add (noArvore);
		}
		
		public int NumeroFilhos {
			get {
				return noArvore.Count;
			}
		}
		
		public INoArvore this [int indice] {
			get {
				//this.indice = indice;
				return noArvore [indice];
			}
		}
	}
}