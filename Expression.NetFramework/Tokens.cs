using System;
namespace Expressao
{
	public enum Tokens
	{
		ID,
		NUMERO,
		CADEIA,
		// Operadores Matematico
		OP_SOMA, 
		OP_SUB,
		OP_MULT,
		OP_DIV,
		OP_EXPOENTE,
		OP_MOD,
		OP_FATORIAL,
		
		// Operadores Relacionais
		OP_REL_MAIOR,
		OP_REL_MENOR,
		OP_REL_MAIOR_QUE,
		OP_REL_MENOR_QUE,
		OP_REL_IGUAL,
		// Operadores Logico
		OP_LOGIC_E,
		OP_LOGIC_OU,
		// Palavras chaves
		PROGRAMA,
		VAR,
		EXPRESAO,
		LOG, 
		LOG10,
		LOG2,
		SEN,
		COS,
		TANG,
		ASEN,
		ACOS,
		ATANG,
		PI,
		NEP,
		RQD, // Raiz quadrada
		//SIMBOLOS
		PARENTESE_ESQUERDO,
		PARENTESE_DIREITO,
		ABRE_CHAVE,
		FECHA_CHAVE,
		ABRE_COCHETE,
		FECHA_COCHETE,
		PONTO_PONTO,
		PONTO_VIRGULA,
		ERRO,
		NENHUM,
		FIM
	}
}