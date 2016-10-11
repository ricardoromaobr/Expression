using System;
using Expressao;

namespace Projetasoft.Expression
{
	/// <summary>
	/// Resolver. 
	/// this resolver an expression 
	/// </summary>
	public class Resolver
	{
		Parse parse;
		Calculadora calc;

		public Resolver (Parse parse)
		{
			this.parse = parse;
			parse.DoParse ();
			calc = new Calculadora (parse.ArvoreSintatica, parse.TabelaSimboloInterna);
		}

		public Resolver (string expression) : this (new Parse (expression))
		{
		}

		/// <summary>
		/// Solvers the expression.
		/// </summary>
		/// <returns>The double value of the result expression.</returns>
		public double SolverExpression ()
		{
			double result; 
			if (parse.TemOpLogic)
				result = calc.ResolverFormulaLogica ();
			else
				result = calc.ResolverFormula ();
			return result;
		}

		/// <summary>
		/// Gets the  <see cref="Expression.Android.Resolver"/> with the specified variableName.
		/// </summary>
		/// <param name="variableName">Variable name.</param>
		public double this [string variableName] {
			get { 
				return calc.ObterVariavel (variableName).Value.Value; 
			}
			set {
				calc.DefinirValorVariavel (variableName, value);
			}
		}

		/// <summary>
		/// Gets the name of the variables.
		/// </summary>
		/// <value>The name of the variables.</value>
		public string[] VariablesName {
			get {
				return calc.VariablesName;
			}
		}


	}
}

