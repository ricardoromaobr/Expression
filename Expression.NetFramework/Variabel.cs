using System;
using System.ComponentModel;

namespace Expressao
{
	public class Variabel 
	{
		private string nome = null;
		private double? valor = null;
				
		public Variabel (string nome)
		{
			this.nome = nome;
		}
		
		public string Name {
			get {
				return nome;
			}
			set {
				nome = value;
			}
		}
		
		public double? Value {
			get { return valor; }
			set { valor = value; }
		}

	}
}