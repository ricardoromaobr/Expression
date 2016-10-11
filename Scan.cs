using System;
using System.Text;
using System.Collections;

namespace Expressao
{
	public class Scan
	{
		private Buffer bf;
		private MaquinaEstado maquinaEstado;
		private Token token;
		private Token tokenAnterior;
		private char caractere;
		private char[] lexema;
		private int posicao = 0;
		private ulong linha = 0;
		private int coluna = 0;
		private TabelaSimbolo tabelaSimbolo;
		private string stringToken;
		private Simbolo simbolo;

		public Scan (Buffer bf, TabelaSimbolo tb)
		{
			this.bf = bf;
			token = new Token (Tokens.NENHUM, "nenhum", linha, coluna);
			tokenAnterior = token;
			linha = 1;
			this.tabelaSimbolo = tb;
		}

		private void IncrementaLexema ()
		{
			posicao++;
			lexema [posicao] = caractere;
		}

		public ulong Linha {
			get { return linha; }
		}

		public int Coluna {
			get { return coluna; }
		}

		public bool PalavraReservada (string lexema)
		{	
			bool result = false;
			
			return result;
		}
		//		public Token TokenAnterior {
		//			get { return tokenAnterior; }
		//		}
		public Token ObterToken ()
		{
			maquinaEstado = MaquinaEstado.INICIO;
			posicao = 0;
			lexema = new char [32];
						
			do {
				switch (maquinaEstado) {
				case MaquinaEstado.INICIO:
					caractere = bf.LerUmCaracter ();
					//TODO: Validar os caracteres somente quando o mesmo for reconhecido
					if (caractere != '\n' && caractere != '\r' && caractere != 0)
						coluna++;
					
					//TODO: representar corretamente o fim da fonte em analise
					if (caractere == '\0') {
						maquinaEstado = MaquinaEstado.FEITO;
						token = new Token (Tokens.NENHUM, "", linha, coluna);
						break;
					}
					
					lexema [posicao] = caractere;
					
					if (Char.IsDigit (caractere)) {
						maquinaEstado = MaquinaEstado.EM_NUM_INTEIRO;
						break;
					} else if (char.IsLetter (caractere)) {
						maquinaEstado = MaquinaEstado.EM_ID;
						break;
					} else if (caractere == '_') {
						maquinaEstado = MaquinaEstado.EM_ID;
						break;
					}  
					
					switch (caractere) {
					case '*':
						token = new Token (Tokens.OP_MULT, new string (lexema, 0, posicao + 1), linha, Coluna);
						maquinaEstado = MaquinaEstado.FEITO;
						break;
					case '+':
						if (tokenAnterior.TipoToken == Tokens.PARENTESE_ESQUERDO || tokenAnterior.TipoToken == Tokens.NENHUM)
							maquinaEstado = MaquinaEstado.EM_NUM_INTEIRO;
						else {
							token = new Token (Tokens.OP_SOMA, new string (lexema, 0, posicao + 1), linha, coluna);
							maquinaEstado = MaquinaEstado.FEITO;
						}
						break;
					case '-':
						if (tokenAnterior.TipoToken == Tokens.PARENTESE_ESQUERDO || tokenAnterior.TipoToken == Tokens.NENHUM)
							maquinaEstado = MaquinaEstado.EM_NUM_INTEIRO;
						else {
							token = new Token (Tokens.OP_SUB, new string (lexema, 0, posicao + 1), linha, coluna);
							maquinaEstado = MaquinaEstado.FEITO;
						}
						break;
					case '%':
						token = new Token (Tokens.OP_MOD, new string (lexema, 0, posicao + 1), linha, coluna);
						maquinaEstado = MaquinaEstado.FEITO;
						break;
					case '!':
						token = new Token (Tokens.OP_FATORIAL, new string (lexema, 0, posicao + 1), linha, coluna);
						maquinaEstado = MaquinaEstado.FEITO;
						break;
					case '^':
						token = new Token (Tokens.OP_EXPOENTE, new string (lexema, 0, posicao + 1), linha, coluna);
						maquinaEstado = MaquinaEstado.FEITO;
						break;
					case '/':
						maquinaEstado = MaquinaEstado.EM_OP_MAT_DIV;
						break;
					case '(':
						token = new Token (Tokens.PARENTESE_ESQUERDO, new string (lexema, 0, posicao + 1), linha, coluna);
						maquinaEstado = MaquinaEstado.FEITO;
						break;
					case ')':
						token = new Token (Tokens.PARENTESE_DIREITO, new string (lexema, 0, posicao + 1), linha, coluna);
						maquinaEstado = MaquinaEstado.FEITO;
						break;
					case '{':
						token = new Token (Tokens.ABRE_CHAVE, new string (lexema, 0, posicao + 1), linha, coluna);
						maquinaEstado = MaquinaEstado.FEITO;
						break;
					case '}':
						token = new Token (Tokens.FECHA_CHAVE, new string (lexema, 0, posicao + 1), linha, coluna);
						maquinaEstado = MaquinaEstado.FEITO;
						break;
					case '[':
						token = new Token (Tokens.ABRE_COCHETE, new string (lexema, 0, posicao + 1), linha, coluna);
						maquinaEstado = MaquinaEstado.FEITO;
						break;
					case ']':
						token = new Token (Tokens.FECHA_COCHETE, new string (lexema, 0, posicao + 1), linha, coluna);
						maquinaEstado = MaquinaEstado.FEITO;
						break;
					case ';':
						token = new Token (Tokens.PONTO_VIRGULA, new string (lexema, 0, posicao + 1), linha, coluna);
						maquinaEstado = MaquinaEstado.FEITO;
						break;
					case '=':
						token = new Token (Tokens.OP_REL_IGUAL, new string (lexema, 0, posicao + 1), linha, coluna);
						maquinaEstado = MaquinaEstado.FEITO;
						break;
					case '>':
						maquinaEstado = MaquinaEstado.EM_OP_REL_MAIOR;
						break;
					case '<':
						maquinaEstado = MaquinaEstado.EM_OP_REL_MENOR;
						break;
					case ' ':
						coluna++;
						break;
					case '\t':
						break;
					case '\n':
						coluna = 0;
						linha++;
						break;
					case '\r':
						coluna = 0;
						maquinaEstado = MaquinaEstado.INICIO;
						break;
					default :
						maquinaEstado = MaquinaEstado.ERRO;
						break;
					}
					
					break;
				
				case MaquinaEstado.EM_ID:
					caractere = bf.LerUmCaracter ();
					coluna++;

					while (char.IsLetter (caractere) || caractere == '_' || char.IsDigit (caractere)) {
						posicao++;
						lexema [posicao] = caractere;
						caractere = bf.LerUmCaracter ();
						coluna++;
					}
					
					if (caractere != 0)
						bf.Retroceder (1);
					
					stringToken = new string (lexema, 0, posicao + 1);
					simbolo = tabelaSimbolo.ObterSimbolo (stringToken);

					if (simbolo != null) {
						token = tabelaSimbolo.ObterSimbolo (stringToken).token;
					} else {
						token = new Token (Tokens.ID, stringToken, linha, coluna - posicao);
						tabelaSimbolo.AdicionarSimbolo (token.Lexema, new Simbolo (token, CategoriaToken.Variavel));
					}
					maquinaEstado = MaquinaEstado.FEITO;
					coluna--;
					break;
				case MaquinaEstado.EM_NUM_INTEIRO:
					
					caractere = bf.LerUmCaracter ();
					if (caractere != 0) {
					
						while (char.IsDigit (caractere)) {
							posicao++;
							coluna++;
							lexema [posicao] = caractere;
							caractere = bf.LerUmCaracter ();
						}
						if (caractere == '.') {
							posicao++;
							coluna++;
							lexema [posicao] = caractere;
							maquinaEstado = MaquinaEstado.EM_NUM_DECIMAL;
						} else {
							maquinaEstado = MaquinaEstado.FEITO;
							if (caractere != 0)
								bf.Retroceder (1);						
							token = new Token (Tokens.NUMERO, new string (lexema, 0, posicao + 1), linha, coluna - posicao);
							coluna--;
						} 
						
					} else {
						token = new Token (Tokens.NUMERO, new string (lexema, 0, posicao + 1), linha, coluna - posicao);
						maquinaEstado = MaquinaEstado.FEITO;
					}
					break;
				case MaquinaEstado.EM_NUM_DECIMAL:
					caractere = bf.LerUmCaracter ();
					
					while (char.IsDigit(caractere)) {
						IncrementaLexema ();
						coluna++;
						caractere = bf.LerUmCaracter ();
					}
					
					token = new Token (Tokens.NUMERO, new string (lexema, 0, posicao + 1), linha, coluna - posicao);
					bf.Retroceder (1);
					maquinaEstado = MaquinaEstado.FEITO;
					coluna--;
					break;
				case MaquinaEstado.EM_OP_MAT_DIV:
					lexema [posicao] = caractere;
					caractere = bf.LerUmCaracter (); 
					if (caractere == '*') {
						maquinaEstado = MaquinaEstado.EM_COMENTARIO;
						lexema [posicao] = '\0';
					} else {
						token = new Token (Tokens.OP_DIV, new string (lexema, 0, posicao + 1), linha, coluna - posicao);
						maquinaEstado = MaquinaEstado.FEITO;
						if (caractere != '\0')
							bf.Retroceder (1);
					}
					break;
				case MaquinaEstado.EM_OP_REL_MAIOR:
					lexema [posicao] = caractere;
					caractere = bf.LerUmCaracter ();
					if (caractere == '=') {
						IncrementaLexema ();
						token = new Token (Tokens.OP_REL_MAIOR_QUE, new string (lexema, 0, posicao + 1), linha, coluna);
						maquinaEstado = MaquinaEstado.FEITO;
					} else {
						bf.Retroceder (1);
						token = new Token (Tokens.OP_REL_MAIOR, new string (lexema, 0, posicao + 1), linha, coluna);
						maquinaEstado = MaquinaEstado.FEITO;
						coluna--;
					}
					break;
				case MaquinaEstado.EM_OP_REL_MENOR:
					lexema [posicao] = caractere;
					caractere = bf.LerUmCaracter ();
					if (caractere == '=') {
						IncrementaLexema ();
						token = new Token (Tokens.OP_REL_MENOR_QUE, new string (lexema, 0, posicao + 1), linha, coluna);
						maquinaEstado = MaquinaEstado.FEITO;
					} else {
						bf.Retroceder (1);
						token = new Token (Tokens.OP_REL_MENOR, new string (lexema, 0, posicao + 1), linha, coluna);
						maquinaEstado = MaquinaEstado.FEITO;
						coluna--;
					}
					break;
				case MaquinaEstado.EM_COMENTARIO:
					caractere = bf.LerUmCaracter ();
					
					while (caractere != '*' && caractere != '\0') {
						
						
						if (caractere == '\n') {
							linha++;
							coluna = 0;
						}
						caractere = bf.LerUmCaracter ();
					}
					
					while (caractere == '*')
						caractere = bf.LerUmCaracter ();
					
					if (caractere == '\0' || caractere == '/')
						maquinaEstado = MaquinaEstado.INICIO;
					else {
						if (caractere == '\n') {
							linha++;
							coluna = 0;
						} else
							coluna++;
							
						maquinaEstado = MaquinaEstado.EM_COMENTARIO;
					}
					break;
				case MaquinaEstado.ERRO:
					token = new Token (Tokens.ERRO, "Invalid simbol " + new string (lexema, 0, posicao + 1), linha, coluna);
					maquinaEstado = MaquinaEstado.FEITO;
					break;		
				case MaquinaEstado.FEITO:
					break;
				default:
					maquinaEstado = MaquinaEstado.ERRO;
					break;
				}
			} while (maquinaEstado != MaquinaEstado.FEITO);

			tokenAnterior = token;
			return token;
		}
	}
}