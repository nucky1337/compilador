using System;
using System.IO;
using System.Collections.Generic;

namespace Buscador
{
	class MainClass
	{
		public enum Transition
		{
			NullTransition = 0,
			PlusTransition,
			StarTransition,
			OrTransition,
			OtherTransition
		}
		public enum StateID
		{
			NullStateID = 0,
			PlusState,
			OtherState
		}
		public abstract class FSMState
		{
			protected Dictionary<Transition,StateID> map = new Dictionary<Transition, StateID> ();
			protected StateID stateID;
			public StateID ID { get { return stateID; } }
			public void AddTransition(Transition T, StateID id)
			{
				if (T == Transition.NullTransition)
					return;
				if (id == StateID.NullStateID)
					return;
				if (map.ContainsKey (T))
					return;
				map.Add (T, id);
			}
			public StateID GetOutputState(Transition T)
			{
				if (map.ContainsKey (T))
					return map [T];
				return StateID.NullStateID;
			}
		}

		public class FSMSystem
		{
			private List<FSMState> states;
			private StateID currentStateID;
			public StateID CurrentStateID { get { return currentStateID; } }
			private FSMState currentState;
			public FSMState CurrentState { get { return currentState; } }
			public FSMSystem()
			{
				states = new List<FSMState>();
			}
			public void AddState(FSMState s)
			{
				if (s == null)
					return;
				if (states.Count == 0) {
					states.Add (s);
					currentState = s;
					currentStateID = s.ID;
					return;
				}
				foreach (FSMState state in states) {
					if (state.ID == s.ID) {
						return;
					}
				}
				states.Add (s);
			}
			public void PerformTransition(Transition T)
			{
				if (T == Transition.NullTransition)
					return;
				StateID id = currentState.GetOutputState (T);
				if (id == StateID.NullStateID)
					return;
				currentStateID = id;
				foreach (FSMState state in states) {
					if (state.ID == currentStateID) {
						currentState = state;
						break;
					}
				}
			} 
		}

		public class PlusTransition : FSMState
		{
			private char c;
			public PlusTransition(char val)
			{
				c = val;
				stateID = StateID.PlusState;
			}
		}
	/// <summary>
	/// version personal
	/// </summary>
		public class State
		{
			public Dictionary<char,int> map = new Dictionary<char,int> ();
			private int idEstado = 0;
			public State(char K, int V)
			{
				map.Add(K,V);
				idEstado = V;
			}
			public State(int V)
			{
				idEstado = V;
			}
			public int getIdEstado()
			{
				return idEstado;
			}
			public void addNode(char T, int S)
			{
				if (map.ContainsKey(T)) 
					return;
				map.Add (T, S);
			}
			public void deleteNode(char T)
			{
				if (map.ContainsKey (T))
					map.Remove (T);
			}
			public int getOutState(char T)
			{
				if (map.ContainsKey (T))
					return map[T];
				return -1;
			}

		}
		public class Machine
		{
			public List<State> states;

			private int idState = 0;
			private int finalState = 0;
			private State cState = null;
			private State firstState = null;
			private State prevState = null;
			public State getPrevState { get { return prevState; } }
			public int getFinalState { get { return finalState; } }
			public void ResetMachine()
			{
				cState = firstState;
			}
			public void setFinalState(int st)
			{
				finalState = st;
			}
			public void setCIDState(int st)
			{
				idState = st;
			}
			public int getCIDState()
			{
				return idState;
			}
			public State getCState()
			{
				return cState;
			}
			public Machine()
			{
				states = new List<State>();
			}
			public void AddState(State S, bool b)
			{
				if (states.Count == 0) {
					states.Add (S);
					firstState = S;
					idState = S.getIdEstado();
					//idState = 0;
					cState = S;
					return;
				}
				foreach (State state in states) {
					if (state.getIdEstado () == S.getIdEstado ())
						return;
				}

				states.Add (S);
				if (b) {
					prevState = cState;
					idState = S.getIdEstado();
					cState = S;
				}
			}

			public void Transition(char T)
			{
				int id = cState.getOutState (T);
				if (id == -1) {
					setCIDState (0);
					ResetMachine ();
					return;
				}
				idState = id;
				foreach (State state in states) {
					if (state.getIdEstado() == idState) {
						cState = state;
						break;
					}
				}
			}
		}

		public static Machine BuildMachine(string expr)
		{
			Machine m = new Machine ();
			char temp = ' ';
			int counter = 1;
			bool pending = false;
			m.AddState (new State (0),true);
			m.setFinalState (0);
			foreach (char c in expr) {
				/*if (m.getCState == 0 && m.getCNode == null) {
					m.AddNode()
				}*/
				if (pending) {
					m.getPrevState.addNode (c, counter - 1);
					pending = false;
				} else {
					switch (c) {
					case '*':
						//m.AddState (new State(temp,m.getCIDState));
						m.getCState ().addNode (temp, m.getCIDState ());
						break;
					case '|':
						//m.getPrevState.addNode (temp, counter - 1);
						pending = true;
						break;
					case '+':
						//m.AddState (new State (counter++), true);
						//m.getPrevState.addNode (temp, counter - 1);
						//m.setFinalState (counter - 1);
						m.getCState ().addNode (temp, m.getCIDState());
						break;
					default:
						m.AddState (new State (counter++), true);
						m.getPrevState.addNode (c, counter - 1);
						m.setFinalState (counter - 1);
						break;
					}
				}
				temp = c;
			}
			m.setCIDState(0);
			m.ResetMachine ();
			return m;
		}
		public static void RecorreMaquina(string S, Machine m)
		{
			foreach (State state in m.states) {
				Console.WriteLine ("Estado: " + state.getIdEstado());
				foreach (KeyValuePair<char,int> map in state.map) {
					Console.WriteLine (string.Format ("Key-{0}:Value-{1}", map.Key, map.Value));
				}
			}
			int i = 0;
			Console.WriteLine ("Estado final: " + m.getFinalState);
			foreach (char c in S) {
				m.Transition (c);
				String valid = " ";
				if (m.getCIDState () == m.getFinalState)
					valid = " cadena valida hasta " + i;
				else
					valid = "";
				i++;
				Console.WriteLine ("Transicion para " + c + ": idState-> " + m.getCIDState () + valid);


			}
		}

		public FSMSystem BuildFSMSystem(string expr)
		{
			return null;
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
			return null;
		}
		/// <summary>
		/// Encuentra el String dentro de otro.
		/// </summary>
		/// <returns>Posicion de la coincidencia</returns>
		/// <param name="T">T. Texto donde buscar</param>
		/// <param name="P">P. Palabra a buscar</param>
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
			Machine m = BuildMachine ("busca+");
			RecorreMaquina ("buscccaaaabuscaaaabusco", m);
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
