using System;
using System.Collections.Generic;
using System.Linq;
using System.Globalization;

namespace Expressao
{
	internal class Calculadora
	{
		double valorTotal = 0;
		bool calculoRealizado = false;
		double[] registro = new double [10];
		INoArvore arvore;
		TabelaSimbolo tabelaSimbolo;
		Dictionary<string,Variabel> variaveis = new Dictionary<string, Variabel> ();

		public Calculadora (INoArvore arvore, TabelaSimbolo tabelaSimbolo)
		{
			this.arvore = arvore;
			this.tabelaSimbolo = tabelaSimbolo;
			IdentificarVariaveis ();
		}

		public string [] VariablesName {
			get {
				return variaveis.Keys.ToArray ();
			}
		}

		private void CarregaAcumulador (int acumulador, double valor)
		{
			registro [acumulador] = valor;
		}

		private void CarregaAcumuladorConstante (INoArvore no, int acumulador)
		{
			switch (no.Token.TipoToken) {
			case Tokens.PI:
				registro [acumulador] = Math.PI;
				break;
			case Tokens.NEP:
				registro [acumulador] = Math.E;
				break;
			}
		}

		private void GeraCalculoExpressao (Token token, int acm1, int acm2)
		{
			switch (token.TipoToken) {
			case Tokens.OP_SOMA:
				registro [acm1] = registro [acm1] + registro [acm2];
				break;
			case Tokens.OP_SUB:
				registro [acm1] = registro [acm1] - registro [acm2];
				break;
			case Tokens.OP_MULT:
				registro [acm1] = registro [acm1] * registro [acm2];
				break;
			case Tokens.OP_DIV:
				registro [acm1] = registro [acm1] / registro [acm2];
				break;
			case Tokens.OP_MOD:
				registro [acm1] = registro [acm1] % registro [acm2];
				break;
			case Tokens.OP_EXPOENTE:
				registro [acm1] = Expoencial (registro [acm1], registro [acm2]);
				break;
			}
		}

		private void GeraCalculoExpressao (Token token, int acm1)
		{
			switch (token.TipoToken) {
			case Tokens.LOG:
				registro [acm1] = Math.Log (registro [acm1]);
				break;
			case Tokens.LOG10:
				registro [acm1] = Math.Log10 (registro [acm1]);
				break;
			case Tokens.LOG2:
				registro [acm1] = Math.Log10 (registro [acm1]) / Math.Log10 (2);
				break;
			case Tokens.COS:
				registro [acm1] = Math.Cos (registro [acm1]);
				break;
			case Tokens.TANG:
				registro [acm1] = Math.Tan (registro [acm1]);
				break;
			case Tokens.SEN:
				registro [acm1] = Math.Sin (registro [acm1]);
				break;
			case Tokens.ACOS:
				registro [acm1] = Math.Acos (registro [acm1]);
				break;
			case Tokens.ASEN:
				registro [acm1] = Math.Asin (registro [acm1]);
				break;
			case Tokens.ATANG:
				registro [acm1] = Math.Atan (registro [acm1]);
				break;
			case Tokens.RQD:
				registro [acm1] = Math.Sqrt (registro [acm1]);
				break;
			case Tokens.OP_FATORIAL:
				registro [acm1] = Fatorial (registro [acm1]);
				break;
			
			default:
				throw new Exception ("Error compiling the expression");
			}
		}

		public double ResolverFormula ()
		{
			ValidarVariaveis ();
			Expressao (arvore, 1);
			valorTotal = registro [1];
			
			calculoRealizado = true;
			return valorTotal;
		}

		public double ResolverFormulaLogica ()
		{
			ValidarVariaveis ();
			ExpressaoLogica ((NoArvoreComposite)arvore, 1);
			calculoRealizado = true;
			valorTotal = registro [1];
			return valorTotal;
		}

		public double? ValorCalculado {
			get {
				if (calculoRealizado) {
					return valorTotal;
				} else
					return null;
			}
		}

		private void IdentificarVariaveis ()
		{
			Simbolo[] simbolos = tabelaSimbolo.Variaveis;
			foreach (Simbolo simbolo in simbolos) {
				Variabel variavel = new Variabel (simbolo.token.Lexema);
				variaveis.Add (variavel.Name, variavel);
			}
		}

		private void ValidarVariaveis ()
		{
			foreach (Variabel v in variaveis.Values) {
				if (v.Value == null)
					throw new Exception ("Variable <" + v.Name + ">" + "not assigned");
			}
		}

		public Variabel ObterVariavel (string nome)
		{
			return variaveis [nome];
		}

		public void DefinirValorVariavel (string nome, double valor)
		{
			bool chaveAchada = false;
			
			foreach (string chave in variaveis.Keys) {
				if (chave == nome) {
					chaveAchada = true;
					break;
				}
			}
			
			if (!chaveAchada) {
				throw new Exception ("Variable dosen't exists: " + nome);
			}
			
			variaveis [nome].Value = valor;
		}

		private void Expressao (INoArvore no, int acumulador)
		{
			switch (no.TipoExpressao) {
			case TipoExpressao.Expressao:
				ExpressaoAritmetica (no, acumulador);
				break;
			case TipoExpressao.Constante:
				CarregaAcumuladorConstante (no, acumulador);
				break;
			case TipoExpressao.Identificador:
				CarregaAcumulador (acumulador, (double)variaveis [no.Token.Lexema].Value);
				break;
			case TipoExpressao.Numero:
				NumberFormatInfo nif = new NumberFormatInfo ();
				nif.NumberDecimalSeparator = ".";
				CarregaAcumulador (acumulador, double.Parse (no.Token.Lexema, nif));
				break;
			default:
				throw new Exception ("Error in " + no.Token.Lexema + " next to " +
				((NoArvoreComposite)no) [1].Token.Lexema);
			}
		}

		private void ExpressaoAritmetica (INoArvore no, int acumulador)
		{
			switch (no.TipoOperador) {
			case TipoOperador.Add:
			case TipoOperador.Sub:
			case TipoOperador.Div:
			case TipoOperador.Mod:
			case TipoOperador.Mult:
			case TipoOperador.Exp:
				CalcExpressaoBinaria (no, acumulador);
				break;
			case TipoOperador.Cientifico:
				CalcExpressaoUnaria (no, acumulador);
				break;
			default:
				throw new Exception ("Compile error, next to  " + no.Token.Lexema);
			}
		}

		private void CalcExpressaoBinaria (INoArvore no, int acumulador)
		{
			NoArvoreComposite noBinario = no as NoArvoreComposite;
			Expressao (noBinario [1], acumulador);
			Expressao (noBinario [2], acumulador + 1);
			GeraCalculoExpressao (noBinario [0].Token, acumulador, acumulador + 1);
		}

		private void CalcExpressaoUnaria (INoArvore no, int acumulador)
		{
			NoArvoreComposite noOperadorUnario = no as NoArvoreComposite;
			Expressao (noOperadorUnario [1], acumulador);
			GeraCalculoExpressao (noOperadorUnario [0].Token, acumulador);
		}

		private void ExpressaoLogica (NoArvoreComposite no, int acumulador)
		{
			double valor1, valor2;
			
			switch (no [0].Token.TipoToken) {
			case Tokens.OP_LOGIC_E:
				ExpressaoLogica (no [1] as NoArvoreComposite, acumulador);
				ExpressaoLogica (no [2] as NoArvoreComposite, acumulador + 1);
				
				if (registro [acumulador] == 1 && registro [acumulador + 1] == 1)
					registro [acumulador] = 1;
				else
					registro [acumulador] = 0; 
				break;
			case Tokens.OP_LOGIC_OU:
				ExpressaoLogica (no [1] as NoArvoreComposite, acumulador);
				ExpressaoLogica (no [2] as NoArvoreComposite, acumulador + 1);
				
				if (registro [acumulador] == 1 || registro [acumulador + 1] == 1)
					registro [acumulador] = 1;
				else
					registro [acumulador] = 0; 
				break;
			case Tokens.OP_REL_IGUAL:
			case Tokens.OP_REL_MAIOR:
			case Tokens.OP_REL_MAIOR_QUE:
			case Tokens.OP_REL_MENOR:
			case Tokens.OP_REL_MENOR_QUE:
				Expressao (no [1], acumulador);
				valor1 = registro [acumulador];
				Expressao (no [2], acumulador + 1);
				valor2 = registro [acumulador + 1];
				Swap (ref valor1, ref registro [8]); 
				Swap (ref valor2, ref registro [9]);
				ComparaExpressaoLogica (no [0].Token, 8, 9);
				registro [acumulador] = registro [8];
				Swap (ref registro [8], ref valor1);
				Swap (ref registro [9], ref valor2);
				break;
			default:
				throw new Exception ("Compile error " + no [0].Token.Lexema +
				" line: " + no [0].Token.Linha.ToString () +
				" column: " + no [0].Token.Coluna.ToString () +
				no [0].Token.Lexema +
				" is a logical expression");
			}
		}

		private void ComparaExpressaoLogica (Token token, int acm1, int acm2)
		{
			switch (token.TipoToken) {
			case Tokens.OP_REL_IGUAL:
				if (registro [acm1] == registro [acm2])
					registro [acm1] = 1;
				else
					registro [acm1] = 0; 
				break;
			case Tokens.OP_REL_MAIOR:
				if (registro [acm1] > registro [acm2])
					registro [acm1] = 1;
				else
					registro [acm1] = 0; 
				break;
			case Tokens.OP_REL_MAIOR_QUE:
				if (registro [acm1] >= registro [acm2])
					registro [acm1] = 1;
				else
					registro [acm1] = 0; 
				break;
			case Tokens.OP_REL_MENOR:
				if (registro [acm1] < registro [acm2])
					registro [acm1] = 1;
				else
					registro [acm1] = 0; 
				break;
			case Tokens.OP_REL_MENOR_QUE:
				if (registro [acm1] <= registro [acm2])
					registro [acm1] = 1;
				else
					registro [acm1] = 0; 
				break;
			}
		}

		private void Swap (ref double origem, ref double destino)
		{
			double tmp = destino;
			destino = origem;
			origem = tmp;
		}

		private double Fatorial (double valor)
		{
			double novoValor = valor;
			if (valor > 1)
				novoValor = novoValor * Fatorial (valor - 1);
			return novoValor;
		}

		private double Expoencial (double valor, double expoente)
		{
			var n = 0;
			double resultado = 0;
			if (expoente > 0) {
				do {
					resultado = valor * valor; 
					n++;
				} while (n < expoente);
			} else
				resultado = 1;
				
			return resultado;
		}
	}
}