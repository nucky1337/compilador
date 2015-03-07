using System;
using System.IO;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace Buscador
{
	class MainClass
	{
		public class States
		{
			private Dictionary<char,int> map = new Dictionary<char,int> ();
			private int idEstado;
			public int getIdEstado()
			{
				return idEstado;
			}
			public void addNode(char T)
			{
				if (map.ContainsKey(T)) 
					return;
				map.Add (T, S);
			}
			public void deleteNode(char T, int S)
			{
				if (map.ContainsKey (T))
					map.Remove (T);
			}
			public int getOutState(char T)
			{
				if (map.ContainsKey (T))
					return map [T];
				return -1;
			}

		}
		pubclic class Machine
		{
			private List<States> states;

			private int cState;
			private States cNode;
			private int getCState()
			{
				return cState;
			}
			private States getCNode()
			{
				return cNode;
			}
			public Machine()
			{
				states = new List<States>();
			}
			public void AddNode(States S)
			{
				if (states.Count == 0) {
					states.Add (S);
					cState = S;
					cNode = S.getIdEstado ();
					return;
				}
				foreach (States state in states) {
					if (state.getIdEstado () == S.getIdEstado ())
						return;
				}
				states.Add (S);
			}
			public void AddLink(char T)
			{
				int id = cNode.getOutState (T);
				if (id == -1)
					return;
				foreach (States state in states) {
					if (state.getIdEstado == cState) {
						cNode = state;
						break;
					}
				}
			}
		}
		public static void TableKMP(string P, ref int []F)
		{
			int pos = 2;
			int cnd = 0;
			F[0] = -1;
			F[1] = 0;
			int iteracion = 1;
			while (pos < P.Length)
			{
				//Console.WriteLine("iteracion " + iteracion +
				//                  " pos: " + pos +
				//                  " cnd: " + cnd +
				//                  " P["+pos+"]: " + P[pos]);
				iteracion++;
				if (P[pos - 1] == P[cnd])
				{
					cnd++;
					F[pos] = cnd;
					pos++;
				}
				else
				{
					if (cnd > 0)
						cnd = F[cnd];
					else {
						F[pos] = 0;
						pos++;
					}
				}
			}
		}
		public Machine CreateMachine(string expr)
		{
			Machine m = new Machine ();

		}
		// T: texto donde buscar
		// P: Palabra a buscar
		public static int findKMP(string T, string P)
		{
	    	int k = 0; //puntero de examen en T
	    	int i = 0; //la posicion del caracter actual en P, y avance relativo respecto de k, para T
	    	int []F = new int[P.Length]; //Tabla de fallo con longitud de P
			int exp = -1;
	    	if (T.Length >= P.Length)
	    	{
	        	TableKMP(P,ref F);
	        	while (k+i < T.Length)
	        	{
					
					if (i+1 < P.Length)
					{
						if (P[i+1] == '?')
						{
							exp = i+1;
						}
					}
	            	else if (P[i] == T[k + i])
	            	{
	                	if (i == P.Length-1)
	                    	return k;
	                	i++;
	            	}
	            	else
	            	{
	                	k = k + i - F[i];
	                	if (i > 0)
	                    	i = F[i];
	            	}
	        	}
	    	}
	    	return -1;
		}
		
		public static void Main (string[] args)
		{
			string usage = "busca - busca en archivos de texto\n" +
		                             "SINTAXIS\n" +
		                             "\tbusca - OPCION ARCHIVO...\n" +
		                             "OPCIONES\n" +
		                             "\t-v PALABRA\t Busca la palabra y muestra la linea que la contiene.\n" +
		                             "\t-r SRC DEST\t Busca la palabra SRC y la reemplaza por DEST\n";
			List<string> files = new List<string>();
			StreamReader sr;
			string expr;
			if (args.Length < 3)
			{
				Console.WriteLine(usage);
			}
			else {
				if (args[0].ToString().ToCharArray()[0] == '-') //calcular tama;o!!!
				{
					switch (args[0].ToString().ToCharArray()[1])
					{
					case 'v':
						expr = args[1];
						Console.WriteLine("Encontrando ocurrencias de " +
						                  expr + " en: \n");
						for (int i = 2; i<args.Length; i++) 
							files.Add(args[i]);
						foreach (string s in files)
						{
							Console.WriteLine(s + ": ");
							if (File.Exists(s))
							{
								sr = new StreamReader(s);
								int pos = 1;
								while (sr.Peek() > -1)
								{
									string line = sr.ReadLine();
									int f = findKMP(line, expr);
									if (f != -1)
										Console.WriteLine("Linea: " + pos + "Columna: " + f + "-> " +
										                  line);
									pos++;
								}
								sr.Close();
							}
							else
								Console.WriteLine("Archivo no encontrado");
						}
						break;
					default:
						Console.WriteLine(usage);
						break;
					}
				}
			}
		}
	}
}
