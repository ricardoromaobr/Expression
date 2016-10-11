using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace Expressao
{
	public class Parse
	{
		private Scan scan;
		private Buffer bf;
		private Token token;
		private Token tokenAnterior;
		private ulong linha;
		private Hashtable tabelaSimbolos;
		private NoArvoreComposite arvoreSintatica;
		private TabelaSimbolo tabelaSimbolo;
		private bool parseFeito = false;
		private int numeroErro = 0;
		private int nivelNoArvore = 0;
		private bool temOpLogic = false;

		public Parse (Buffer bf)
		{
			this.bf = bf;
			tabelaSimbolos = new Hashtable ();
			tabelaSimbolo = new TabelaSimbolo (tabelaSimbolos);
			scan = new Scan (this.bf, tabelaSimbolo);
		}

		public Parse (string expression) : this (new Buffer (expression))
		{
		}

		public bool TemOpLogic {
			get { return temOpLogic; }
		}

		public void DoParse ()
		{
			linha = 0;
			
			/* inicia a tabela de simbolo com as palavras reservadas */
			IniciarTabelaSimbolo ();
			token = scan.ObterToken ();

			INoArvore no = ListaExpresao ();
			
			if (token.TipoToken != Tokens.NENHUM)
				EmiteErroSintaxe ("error in the expression, compile finish before the end of expression.");
			//TODO: throw the exception for imcomplete parse
			
			if (no == null)
				return;
				
			if (no.NumeroFilhos == 0)
				arvoreSintatica = new NoArvoreComposite (no);
			else
				arvoreSintatica = (NoArvoreComposite)no;
			
			if (arvoreSintatica != null) {
				parseFeito = true;
				DefineExpressaoLogica (arvoreSintatica);
			}
		}

		public int Errors {
			get { return numeroErro; }
		}

		public NoArvoreComposite ArvoreSintatica {
			get {
				if (parseFeito)
					return arvoreSintatica;
				else
					return null;
			}
		}
		//TODO: Refactory - definir uma interface unica para uso da tabela de simbolo
		public TabelaSimbolo TabelaSimboloInterna {
			get { return tabelaSimbolo; }
		}

		private void IniciarTabelaSimbolo ()
		{
			tabelaSimbolos.Clear ();
			// inicia a tabela de simbolo com as palavra chave, ja com um token definido para cada um
			tabelaSimbolos.Add ("log", new Simbolo (new Token (Tokens.LOG, "log", 0, 0), CategoriaToken.PalavraChave));
			tabelaSimbolos.Add ("log10", new Simbolo (new Token (Tokens.LOG10, "log10", 0, 0), CategoriaToken.PalavraChave));
			tabelaSimbolos.Add ("log2", new Simbolo (new Token (Tokens.LOG2, "log2", 0, 0), CategoriaToken.PalavraChave));
			tabelaSimbolos.Add ("sin", new Simbolo (new Token (Tokens.SEN, "sin", 0, 0), CategoriaToken.PalavraChave));
			tabelaSimbolos.Add ("cos", new Simbolo (new Token (Tokens.COS, "cos", 0, 0), CategoriaToken.PalavraChave));
			tabelaSimbolos.Add ("tan", new Simbolo (new Token (Tokens.TANG, "tan", 0, 0), CategoriaToken.PalavraChave));
			tabelaSimbolos.Add ("asin", new Simbolo (new Token (Tokens.ASEN, "asin", 0, 0), CategoriaToken.PalavraChave));
			tabelaSimbolos.Add ("acos", new Simbolo (new Token (Tokens.ACOS, "acos", 0, 0), CategoriaToken.PalavraChave));
			tabelaSimbolos.Add ("atan", new Simbolo (new Token (Tokens.ATANG, "atan", 0, 0), CategoriaToken.PalavraChave));
			tabelaSimbolos.Add ("pi", new Simbolo (new Token (Tokens.PI, "pi", 0, 0), CategoriaToken.PalavraChave));
			tabelaSimbolos.Add ("e", new Simbolo (new Token (Tokens.NEP, "e", 0, 0), CategoriaToken.PalavraChave));
			tabelaSimbolos.Add ("and", new Simbolo (new Token (Tokens.OP_LOGIC_E, "and", 0, 0), CategoriaToken.PalavraChave));
			tabelaSimbolos.Add ("or", new Simbolo (new Token (Tokens.OP_LOGIC_OU, "or", 0, 0), CategoriaToken.PalavraChave));
			tabelaSimbolos.Add ("sqrt", new Simbolo (new Token (Tokens.RQD, "sqrt", 0, 0), CategoriaToken.PalavraChave));
		}

		public Hashtable SimbolTable {
			get {
				return tabelaSimbolo.TabSimbolo;
			}
		}
		/*
		public void ImprimirTabelaSimbolo ()
		{
			if (parseFeito) {
				Console.WriteLine ("\n<< TABELA SIMBOLO >>\n");
				foreach (Simbolo simbolo in tabelaSimbolo.TabSimbolo.Values) {
					Console.WriteLine ("Simbolo => {0} Categoria {1}", simbolo.token.Lexema,simbolo.categoriaToken);
				}
			}
		}


		public void ImprimirArvoreSintatica ()
		{
			if (!parseFeito || arvoreSintatica.NumeroFilhos == 0)
			{
				Console.WriteLine ("\nParse nao foi feito. <<<\n");
				return;
			}
			
			nivelNoArvore = 0;
			Console.WriteLine ("\n<<ARVORE SINTATICA>>\n");
			Console.WriteLine ("Cada filho esta uma linha abaixo a direita do PAI");
			ImprimeNo (arvoreSintatica);		
		}
		
		private void ImprimeNo (INoArvore noArvore)
		{
			NoArvoreComposite no;
			// imprime tabulacoes representando o nivel da arvore
			
			Console.SetCursorPosition (0,Console.CursorTop);
			
			for (int i = 0 ; i < nivelNoArvore-1 ; i++)
					Console.Write ("\t");
			
			if (noArvore.NumeroFilhos == 0) {
				
				if (nivelNoArvore == 0)
					Console.WriteLine ("{0}",noArvore.Token.Lexema);
				else 
					Console.WriteLine (":------{0}",noArvore.Token.Lexema);
					
			}
			else {
				
				if (nivelNoArvore == 0)
					Console.WriteLine ("{0}",noArvore.Token.Lexema);
				else 
					Console.WriteLine (":------{0}",noArvore.Token.Lexema);	
				
				nivelNoArvore++;
				no = (NoArvoreComposite) noArvore;
				
				for (int i = 1; i <= noArvore.NumeroFilhos-1; i++) {
					
					if (nivelNoArvore > 1)
						for (int j = 0 ; j < nivelNoArvore-1 ; j++)
							if (j == nivelNoArvore - 2)
								;//Console.Write (":\t");
							else
								Console.Write ("\t");
			
					ImprimeNo (no [i]);
				}
				nivelNoArvore--;
			}
		}
		
		*/
		private void DefineExpressaoLogica (INoArvore noArvore)
		{
			NoArvoreComposite no;
			// imprime tabulacoes representando o nivel da arvore


			if (noArvore.TipoOperador == TipoOperador.OperadorLogico) {
				temOpLogic = true;
				return;
			} else {
				if (noArvore.NumeroFilhos > 0) {
					no = (NoArvoreComposite)noArvore;

					for (int i = 1; i <= noArvore.NumeroFilhos - 1; i++) {
						DefineExpressaoLogica (no [i]);
						if (temOpLogic)
							break;
					}
				}
			}		
		}

		private INoArvore ListaExpresao ()
		{
			INoArvore no = null; 
			NoArvoreComposite noPai;
			
			no = ExpressaoLogicaE (); 
			
			while (token.TipoToken == Tokens.OP_LOGIC_OU) {
				Casamento (Tokens.OP_LOGIC_OU);
				noPai = new NoArvoreComposite (new NoArvore (tokenAnterior, TipoExpressao.ExpressaoLogica, TipoOperador.OperadorLogico));
				
				if (no != null)
					noPai.Add (no);
				else
					EmiteErroSintaxe ("error expression");
				
				no = ExpressaoLogicaE (); 
				
				if (no != null)
					noPai.Add (no);
				else
					EmiteErroSintaxe ("error expression"); 
				
				no = noPai;
			}
			 
			return no;
		}

		private INoArvore ExpressaoLogicaE ()
		{
			INoArvore no = null; 
			NoArvoreComposite noPai = null; 
			
			no = ExpressaoLogica (); 
			
			while (token.TipoToken == Tokens.OP_LOGIC_E) {
				Casamento (Tokens.OP_LOGIC_E); 
				noPai = new NoArvoreComposite (new NoArvore (tokenAnterior, TipoExpressao.ExpressaoLogica, TipoOperador.OperadorLogico));
				if (no != null)
					noPai.Add (no); // expressao lado esquerda do operador
				else
					EmiteErroSintaxe ("left expression not defined");
				
				no = ExpressaoLogica ();
				if (no != null)
					noPai.Add (no);
				else
					EmiteErroSintaxe ("right expression not defined");
				
				no = noPai;
			}
			
			return no;
			
		}

		private INoArvore ExpressaoLogica ()
		{
			INoArvore no = null; 
			NoArvoreComposite noPai;
				
			no = Expressao ();
			switch (token.TipoToken) {
			case Tokens.OP_REL_MAIOR:
				Casamento (Tokens.OP_REL_MAIOR);
				noPai = new NoArvoreComposite (new NoArvore (tokenAnterior, TipoExpressao.Expressao, TipoOperador.OperadorLogico));
				if (no != null)
					noPai.Add (no);   // expressao a esquerda do operador logico relacional
				else
					EmiteErroSintaxe ("left expression not defined");
				
				no = Expressao (); 
				if (no != null)
					noPai.Add (no); // expressa a direita do operador logico relaciona 
				else
					EmiteErroSintaxe ("right expression not defined");
				no = noPai;
				break;
			case Tokens.OP_REL_MENOR:
				Casamento (Tokens.OP_REL_MENOR);
				noPai = new NoArvoreComposite (new NoArvore (tokenAnterior, TipoExpressao.Expressao, TipoOperador.OperadorLogico));
				if (no != null)
					noPai.Add (no);   // expressao a esquerda do operador logico relacional
				else
					EmiteErroSintaxe ("left expression not defined");
				
				no = Expressao (); 
				if (no != null)
					noPai.Add (no); // expressao a direita do operador logico relaciona 
				else
					EmiteErroSintaxe ("right expression not defined");
				no = noPai;
				break;
			case Tokens.OP_REL_MAIOR_QUE:
				Casamento (Tokens.OP_REL_MAIOR_QUE);
				noPai = new NoArvoreComposite (new NoArvore (tokenAnterior, TipoExpressao.Expressao, TipoOperador.OperadorLogico));
				if (no != null)
					noPai.Add (no);   // expressao a esquerda do operador logico relacional
				else
					EmiteErroSintaxe ("left expression not defined");
				
				no = Expressao (); 
				if (no != null)
					noPai.Add (no); // expressa a direita do operador logico relaciona 
				else
					EmiteErroSintaxe ("right expression not defined");
				no = noPai;
				break;
			case Tokens.OP_REL_MENOR_QUE:
				Casamento (Tokens.OP_REL_MENOR_QUE);
				noPai = new NoArvoreComposite (new NoArvore (tokenAnterior, TipoExpressao.Expressao, TipoOperador.OperadorLogico));
				if (no != null)
					noPai.Add (no);   // expressao a esquerda do operador logico relacional
				else
					EmiteErroSintaxe ("left expression not defined");
				
				no = Expressao (); 
				if (no != null)
					noPai.Add (no); // expressa a direita do operador logico relaciona 
				else
					EmiteErroSintaxe ("right expression not defined");
				
				no = noPai;
				break;
			case Tokens.OP_REL_IGUAL:
				Casamento (Tokens.OP_REL_IGUAL);
				noPai = new NoArvoreComposite (new NoArvore (tokenAnterior, TipoExpressao.Expressao, TipoOperador.OperadorLogico));
				if (no != null)
					noPai.Add (no);   // expressao a esquerda do operador logico relacional
				else
					EmiteErroSintaxe ("left operando not defined");
				
				no = Expressao (); 
				if (no != null)
					noPai.Add (no); // expressa a direita do operador logico relaciona 
				else
					EmiteErroSintaxe ("right operand not defined");
				
				no = noPai;
				break;	
			}
			
			return no;
		}

		private INoArvore Expressao ()
		{
			INoArvore no = null; 
			
			no = Termo ();
			
			while (token.TipoToken == Tokens.OP_SOMA || token.TipoToken == Tokens.OP_SUB) {
				NoArvoreComposite noPai = new NoArvoreComposite (Soma ());
				if (no != null)
					noPai.Add (no);  // filho1
				else
					EmiteErroSintaxe ("left operand not defined");
				
				no = Termo ();
				
				if (no != null)
					noPai.Add (no); // filho2
				else
					EmiteErroSintaxe ("right operand not defined");
				no = noPai;
			}

			return no;
		}

		private INoArvore Soma ()
		{
			INoArvore noArvore = null;
			switch (token.TipoToken) {
			case Tokens.OP_SOMA:
				Casamento (Tokens.OP_SOMA);
				noArvore = new NoArvore (tokenAnterior, TipoExpressao.Expressao, TipoOperador.Add);
				break;
			case Tokens.OP_SUB:
				Casamento (Tokens.OP_SUB);
				noArvore = new NoArvore (tokenAnterior, TipoExpressao.Expressao, TipoOperador.Sub);
				break;
			}
			
			return noArvore;
		}

		private INoArvore Termo ()
		{
			INoArvore no = null;
			NoArvoreComposite noPai;
			
			switch (token.TipoToken) {
			case Tokens.NUMERO:
			case Tokens.ID:
			case Tokens.PARENTESE_ESQUERDO:
			case Tokens.COS:
			case Tokens.SEN:
			case Tokens.TANG:
			case Tokens.ACOS:
			case Tokens.ASEN:
			case Tokens.ATANG:
			case Tokens.LOG:
			case Tokens.LOG2:
			case Tokens.LOG10:
			case Tokens.PI:
			case Tokens.NEP:
			case Tokens.RQD:
				no = Fator ();
				
				if (token.TipoToken == Tokens.OP_FATORIAL) {
					Casamento (Tokens.OP_FATORIAL);
					noPai = new NoArvoreComposite (new NoArvore (tokenAnterior, TipoExpressao.Expressao, TipoOperador.Cientifico));
					noPai.Add (no);
					no = noPai;
				}
				
				while (token.TipoToken == Tokens.OP_MULT || token.TipoToken == Tokens.OP_DIV ||
				       token.TipoToken == Tokens.OP_MOD || token.TipoToken == Tokens.OP_EXPOENTE) {
					noPai = new NoArvoreComposite (Mult ());
					if (no != null)
						noPai.Add (no); // filho1
					else
						EmiteErroSintaxe ("Erro expect na expression");
					no = Fator ();
					if (no != null)
						noPai.Add (no); // filho2
					else
						EmiteErroSintaxe ("Error expect an expression");
					no = noPai;
				}
				
				break;
			//case Tokens.NENHUM:
			//	break;
			default:
				EmiteErroSintaxe ("Error sintaxe");
				break;
			}
			
			return no;
		}

		private INoArvore Mult ()
		{
			INoArvore noArvore = null;
			
			switch (token.TipoToken) {
			case Tokens.OP_MULT:
				Casamento (Tokens.OP_MULT);
				noArvore = new NoArvore (tokenAnterior, TipoExpressao.Expressao, TipoOperador.Mult);
				break;
			case Tokens.OP_DIV:
				Casamento (Tokens.OP_DIV);
				noArvore = new NoArvore (tokenAnterior, TipoExpressao.Expressao, TipoOperador.Div);
				break;
			case Tokens.OP_EXPOENTE:
				Casamento (Tokens.OP_EXPOENTE);
				noArvore = new NoArvore (tokenAnterior, TipoExpressao.Expressao, TipoOperador.Exp);
				break;
			case Tokens.OP_MOD:
				Casamento (Tokens.OP_MOD);
				noArvore = new NoArvore (tokenAnterior, TipoExpressao.Expressao, TipoOperador.Mod);
				break;
			}
			
			return noArvore;
		}

		private INoArvore OpCientifico ()
		{
			INoArvore no = null;
			
			switch (token.TipoToken) {
			case Tokens.LOG:
				Casamento (Tokens.LOG);
				no = new NoArvore (tokenAnterior, TipoExpressao.Expressao, TipoOperador.Cientifico);
				break;
			case Tokens.LOG2:
				Casamento (Tokens.LOG2);
				no = new NoArvore (tokenAnterior, TipoExpressao.Expressao, TipoOperador.Cientifico);
				break;
			case Tokens.LOG10:
				Casamento (Tokens.LOG10);
				no = new NoArvore (tokenAnterior, TipoExpressao.Expressao, TipoOperador.Cientifico);
				break;
			case Tokens.SEN:
				tokenAnterior = token;
				Casamento (Tokens.SEN);
				no = new NoArvore (tokenAnterior, TipoExpressao.Expressao, TipoOperador.Cientifico);
				break;
			case Tokens.COS:
				Casamento (Tokens.COS);
				no = new NoArvore (tokenAnterior, TipoExpressao.Expressao, TipoOperador.Cientifico);
				break;
			case Tokens.TANG:
				Casamento (Tokens.TANG);
				no = new NoArvore (tokenAnterior, TipoExpressao.Expressao, TipoOperador.Cientifico);
				break;
			case Tokens.ASEN:
				Casamento (Tokens.ASEN);
				no = new NoArvore (tokenAnterior, TipoExpressao.Expressao, TipoOperador.Cientifico);
				break;
			case Tokens.ACOS:
				Casamento (Tokens.ACOS);
				no = new NoArvore (tokenAnterior, TipoExpressao.Expressao, TipoOperador.Cientifico);
				break;
			case Tokens.PI:
				Casamento (Tokens.PI);
				no = new NoArvore (tokenAnterior, TipoExpressao.Expressao, TipoOperador.Cientifico);
				break;
			case Tokens.NEP:
				Casamento (Tokens.NEP);
				no = new NoArvore (tokenAnterior, TipoExpressao.Expressao, TipoOperador.Cientifico);
				break;
			case Tokens.RQD:
				Casamento (Tokens.RQD);
				no = new NoArvore (tokenAnterior, TipoExpressao.Expressao, TipoOperador.Cientifico);
				break;
				
			default:
				EmiteErroSintaxe (token.Lexema);
				break;
			}
			return no;
		}

		private INoArvore Fator ()
		{
			NoArvoreComposite noPai;
			INoArvore no = null;
			
			switch (token.TipoToken) {
			case Tokens.ID:
				Casamento (Tokens.ID);
				no = new NoArvore (tokenAnterior, TipoExpressao.Identificador, TipoOperador.Nenhum);
				break;
			case Tokens.NUMERO:
				Casamento (Tokens.NUMERO);
				no = new NoArvore (tokenAnterior, TipoExpressao.Numero, TipoOperador.Nenhum);
				break;
			case Tokens.PARENTESE_ESQUERDO:
				Casamento (Tokens.PARENTESE_ESQUERDO);
				no = Expressao ();
				Casamento (Tokens.PARENTESE_DIREITO);
				break;
			case Tokens.COS:
			case Tokens.SEN:
			case Tokens.TANG:
			case Tokens.ACOS:
			case Tokens.ASEN:
			case Tokens.ATANG:
			case Tokens.LOG:
			case Tokens.LOG2:
			case Tokens.LOG10:
			case Tokens.RQD:
				no = OpCientifico ();
				noPai = new NoArvoreComposite (no);
				Casamento (Tokens.PARENTESE_ESQUERDO);
				no = Expressao ();
				noPai.Add (no);
				no = noPai;
				Casamento (Tokens.PARENTESE_DIREITO);
				break;
			case Tokens.PI:
				Casamento (Tokens.PI);
				no = new NoArvore (tokenAnterior, TipoExpressao.Constante, TipoOperador.Nenhum);
				break;
			case Tokens.NEP:
				Casamento (Tokens.NEP);
				no = new NoArvore (tokenAnterior, TipoExpressao.Constante, TipoOperador.Nenhum);
				break;
			default:
				EmiteErroSintaxe ("Error  of sintaxe on the line:" + token.Linha + " next to line: " +
				token.Coluna + " invalid token" + token.Lexema + "'");
				break;
			}
			return no;
		}

		private void Casamento (Tokens tokenEsperado)
		{
			if (tokenEsperado == token.TipoToken) {
				tokenAnterior = token;
				token = scan.ObterToken ();
			} else
				EmiteErroSintaxe ("ERROR: expect token" + ObterLexemaToken (tokenEsperado) +
				" Found token: " + token.Lexema);
		}

		private void EmiteErroSintaxe (string msg)
		{
			Console.WriteLine (msg);
			//TODO: corrigir mensagem de erro
			numeroErro++;
		}

		private string ObterLexemaToken (Tokens tipoToken)
		{
			string result = null;
			
			switch (tipoToken) {
			case Tokens.ID:
				result = "Identificador";
				break;
			case Tokens.NUMERO:
				result = "Numero";
				break;
			case Tokens.OP_SOMA:
				result = "+";
				break;
			case Tokens.OP_SUB:
				result = "-";
				break;
			case Tokens.OP_MULT:
				result = "*";
				break;
			case Tokens.OP_DIV:
				result = "/";
				break;
			case Tokens.OP_EXPOENTE:
				result = "^";
				break;
			case Tokens.OP_MOD:
				result = "%";
				break;
			}
			
			return result;
		}
	}
}