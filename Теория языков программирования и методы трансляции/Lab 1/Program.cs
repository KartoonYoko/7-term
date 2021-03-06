using System;
using System.Text;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Lab_1
{
	class Program
	{
		/// <summary>
		/// Правило языка
		/// </summary>
		public class Rule {
			/// <summary>
			/// Порождающая цепочка языка
			/// </summary>
			public string Key { get; set; }
			/// <summary>
			/// Порождаемая цепочка языка
			/// </summary>
			public string Value { get; set; }
			/// <summary>
			/// Введет ли правило зацикливанию.
			/// True - введет, false - правило не зацикливает грамматику
			/// </summary>
			public bool IsLooped { get; set; }

			public Rule(string k, string v, bool l = false) {
				Key = k;
				Value = v;
				IsLooped = l;
			}
		}

		/// <summary>
		/// Класс формального языка с леволинейной грамматикой и проверкой на зацикливание
		/// </summary>
		public class FormalLanguage {

			/// <summary>
			/// Правила языка
			/// </summary>
			private List<Rule> _rules { get; set; }
			/// <summary>
			/// Максимально количество повторений
			/// </summary>
			public uint MaxRepetitionsCount { get; set; }

			public FormalLanguage(List<Rule> rules, uint count = 10000) {
				_rules = rules;
				MaxRepetitionsCount = count;
			}

			/// <summary>
			/// Проверяет правило на зацикливание
			/// </summary>
			/// <param name="input">Строка, к которой применяется правило</param>
			/// <param name="rule">Правило языка</param>
			/// <param name="count">Количество допустимых повторений</param>
			/// <param name="isReverse">Проверять ли с конца строки.</param>
			/// <returns>true - если правило зацикливает перевод, иначе - false</returns>
			private bool CheckLoop(string input, Rule rule, int count = 5, bool isReverse = false) {
				for (int i = 0; i < count; i++) {
					string key = rule.Key;
					string value = rule.Value;

					int pos;
					if (isReverse) pos = input.LastIndexOf(key);
					else pos = input.IndexOf(key);
					if (pos != -1)
					{
						input = input.Remove(pos, key.Length);
						input = input.Insert(pos, value);
					}
					else return false;				
				}

				return true;
			}
			/// <summary>
			/// Переводит строку на формальный язык
			/// </summary>
			/// <param name="text">Строка для перевода</param>
			/// <returns>Строка на формальном языке</returns>
			public string Translate(string text)
			{
				int count = 0;
				bool isEnd = false;	// true - если ни одно из правил непреминимо
				while (count < MaxRepetitionsCount)
				{
					if (isEnd) break;

					count++;
					isEnd = true;
					// применяем по очереди каждое правило языка к строке
					foreach (Rule rule in _rules)
					{
						if (!rule.IsLooped)		// если правило зацикливает
						{
							string key = rule.Key;
							string value = rule.Value;

							int pos = text.IndexOf(key);

							if (pos != -1)	// если ключ найден
							{
								// если правило зацикливает перевод - запоминаем это
								if (CheckLoop(text, rule)) rule.IsLooped = true;
								else
								{
									text = text.Remove(pos, key.Length);
									text = text.Insert(pos, value);
									isEnd = false;

									break;
								}
							}
						}
						else rule.IsLooped = !rule.IsLooped;
					}
				}

				RefreshRules();
				return text;
			}
			/// <summary>
			/// Переводит строку на формальный язык. Анализирует строку справа
			/// </summary>
			/// <param name="text">Строка для перевода</param>
			/// <returns>Строка на формальном языке</returns>
			public string TranslateRight(string text){
				int count = 0;
				bool isEnd = false;	// true - если ни одно из правил непреминимо
				while (count < MaxRepetitionsCount)
				{
					if (isEnd) break;

					count++;
					isEnd = true;
					// применяем по очереди каждое правило языка к строке
					foreach (Rule rule in _rules)
					{
						if (!rule.IsLooped)		// если правило зацикливает
						{
							string key = rule.Key;
							string value = rule.Value;

							int pos = text.LastIndexOf(key);

							if (pos != -1)	// если ключ найден
							{
								// если правило зацикливает перевод - запоминаем это
								if (CheckLoop(text, rule, isReverse: true)) rule.IsLooped = true;
								else
								{
									text = text.Remove(pos, key.Length);
									text = text.Insert(pos, value);
									isEnd = false;

									break;
								}
							}
						}
						else rule.IsLooped = !rule.IsLooped;
					}
				}

				RefreshRules();
				return text;
			}
			/// <summary>
			/// Переводит строку на формальный язык. 
			/// Если встречается несколько выводов из одного нетерминального символа - берет случайный. 
			/// </summary>
			/// <param name="text">Строка для перевода.</param>
			/// <returns>Строка на формальном языке.</returns>	
			public string TranslateRandom(string text){
				int count = 0;
				bool isEnd = false;	// true - если ни одно из правил непреминимо
				while (count < MaxRepetitionsCount)
				{
					if (isEnd) break;

					count++;
					isEnd = true;
					// правила, которые применимы к текущему состоянию строки
					List<Rule> checkedRules = new();
					// применяем по очереди каждое правило языка к строке
					foreach (Rule rule in _rules)
					{
						string key = rule.Key;
						string value = rule.Value;

						int pos = text.LastIndexOf(key);

						if (pos != -1)	// если ключ найден
						{					
							checkedRules.Add(rule);
							isEnd = false;														
						}						
					}
					Random random = new();
					int index = random.Next(checkedRules.Count);
					Rule ruleChecked = null;
					if (checkedRules.Count != 0){
						ruleChecked = checkedRules[index];
					}					
					
					if (ruleChecked != null){
						string k = ruleChecked.Key;
						string v = ruleChecked.Value;
						int p = text.LastIndexOf(k);
						text = text.Remove(p, k.Length);
						text = text.Insert(p, v);	
					}				
				}

				// RefreshRules();
				return text;
			}

			/// <summary>
			/// Лвеосторонний вывод.
			/// </summary>
			/// <returns>Строка, порожденная на основе правил языка.</returns>
			public string OutputLeft() {

				string result = "S";
				int count = 0;
				while (count < MaxRepetitionsCount) {
					int pos = -1;

					// найдем крайний левый нетерминальный символ в цепочке
					foreach (Rule rule in _rules)
					{
						string key = rule.Key;
						int findPos = result.IndexOf(key);
						if ((pos > findPos || pos == -1) && findPos != -1) {
							pos = findPos;
						}

					}

					// если не найдено ниодного подходящего правила - выходим
					if (pos == -1)
					{
						break;
					}

					// найдем все правил подходящие для крайнего левого нетерминального символа
					List<Rule> rules = new(); 
					foreach (Rule rule in _rules) {
						string key = rule.Key;
						if (pos == result.IndexOf(key)) {
							rules.Add(rule);
						}
					}

					// случайно выберем правило
					Random random = new();
					int index = random.Next(rules.Count);
					Rule r = rules[index];

					int p = result.IndexOf(r.Key);
					result = result.Remove(p, r.Key.Length);
					result = result.Insert(p, r.Value);

					count++;
				}

				return result;
			}

			/// <summary>
			/// Правосторонний вывод.
			/// </summary>
			/// <returns>Строка, порожденная на основе правил языка.</returns>
			public string OutputRight()
			{

				string result = "S";
				int count = 0;
				while (count < MaxRepetitionsCount)
				{
					int pos = -1;

					// найдем крайний правый нетерминальный символ в цепочке
					foreach (Rule rule in _rules)
					{
						string key = rule.Key;
						int findPos = result.IndexOf(key);
						if ((pos < findPos || pos == -1) && findPos != -1)
						{
							pos = findPos;
						}

					}

					// если не найдено ниодного подходящего правила - выходим
					if (pos == -1)
					{
						break;
					}

					// найдем все правил подходящие для крайнего правого нетерминального символа
					List<Rule> rules = new();
					foreach (Rule rule in _rules)
					{
						string key = rule.Key;
						if (pos == result.LastIndexOf(key))
						{
							rules.Add(rule);
						}
					}

					// случайно выберем правило
					Random random = new();
					int index = random.Next(rules.Count);
					Rule r = rules[index];

					int p = result.LastIndexOf(r.Key);
					result = result.Remove(p, r.Key.Length);
					result = result.Insert(p, r.Value);

					count++;
				}

				return result;
			}

			private void RefreshRules() {
				foreach (Rule rule in _rules) {
					rule.IsLooped = false;
				}
			}
		}
		
		/// <summary>
		/// Грамматика формального языка
		/// </summary>
		public class Grammar {
			/// <summary>
			/// Множество терминальных символов
			/// </summary>
			public List<string> Nonterminal { get; set; }
			/// <summary>
			/// Множество терминальных символов
			/// </summary>
			public List<string> Terminal { get; set; }
			/// <summary>
			/// Множество правил (продукций) грамматики
			/// </summary>
			public List<Rule> P { get; set; }
			/// <summary>
			/// Целевой (начальный) символ грамматики
			/// </summary>
			public string S { get; set; }
			/// <summary>
			/// 
			/// </summary>
			/// <param name="vn">Нетерминальные символы</param>
			/// <param name="vt">Nthvbyfkmyst cbvdjks</param>
			/// <param name="rules">Правила</param>
			/// <param name="s">Начальный символ</param>
			public Grammar(List<string> vn, List<string> vt, List<Rule> rules, string s = "S") {
				Nonterminal = vn;
				Terminal = vt;
				P = rules;
				S = s;
			}
			/// <summary>
			/// Возвращает тип грамматики
			/// </summary>
			/// <returns></returns>
			public string GetTypeGrammar() {
				bool isTypeOne = true;
				bool isTypeTwo = true;
				bool isTypeThree = true;

				bool isEachTermPosBigger = true;
				bool isEachTermPosSmaller = true;
				foreach (Rule r in P) {
					// проверка проинадлежности первому типу грамматики
					isTypeOne &= r.Key.Length <= r.Value.Length;

					// проверка принадлежности второму типу
					foreach (string vt in Terminal) {
						isTypeTwo &= !r.Key.Contains(vt);
					}

					// 
					if (isEachTermPosBigger || isEachTermPosSmaller)
					{
						List<int> terminlPositions = new();
						List<int> nonTerminlPositions = new();
						foreach (string vn in Nonterminal)
						{
							int pos = r.Value.IndexOf(vn);
							if (pos != -1) nonTerminlPositions.Add(pos);
						}
						foreach (string vt in Terminal)
						{
							int pos = r.Value.IndexOf(vt);
							if (pos != -1) terminlPositions.Add(pos);
						}
						foreach (int pos in terminlPositions)
						{
							foreach (int posNonTerm in nonTerminlPositions)
							{
								isEachTermPosBigger &= pos > posNonTerm;
								isEachTermPosSmaller &= pos < posNonTerm;
							}
						}
					}
				}

				if ((isEachTermPosBigger && isEachTermPosSmaller) || (!isEachTermPosBigger && !isEachTermPosSmaller))
				{
					isTypeThree = false;
				}
				string res = "0";
				if (isTypeOne) res += " 1";
				if (isTypeTwo) res += " 2";
				if (isTypeThree) res += " 3";
				return res;
			}
			/// <summary>
			/// Создает дерево вывода из цепочки символов
			/// </summary>
			/// <param name="text">Строка (цепочка символов), для которой нужно построить дерево</param>
			/// <returns></returns>
			public string MakeTree(string text){				
				int maxCount = 10000;
				int count = 0;
				List<string> tree = new();
				tree.Add(text);
				while (count < maxCount){
					foreach(Rule rule in P){
						string key = rule.Key;
						string value = rule.Value;

						int pos = text.LastIndexOf(value);
						if (pos != -1) {
							text = text.Remove(pos, value.Length);
							text = text.Insert(pos, key);
							
							string separator = "|"; 
							for (int i = 0; i < pos; i++){
								separator = " " + separator;
							}
							tree.Add(separator);
							tree.Add(text);
						}
					}
					count++;
				}
				tree.Reverse();

				foreach(string branch in tree){
					Console.WriteLine(branch);
				}
				return text;
			}

		}

		static void Main(string[] args)
		{
			Console.OutputEncoding = Encoding.Unicode;

			Console.WriteLine("Первая лабораторная работа.");
			Console.WriteLine("");
			Console.WriteLine("Задание 2.");

			Console.WriteLine("Подпункт a)");

			List<Rule> dict = new()
			{
				new Rule("S", "aaCFD"),
				new Rule("AD", "D"),
				new Rule("F", "AFB"),
				new Rule("F", "AB"),
				new Rule("Cb", "bC"),
				new Rule("AB", "bBA"),
				new Rule("CB", "C"),
				new Rule("Ab", "bA"),
				new Rule("bCD", "\u03B5"),  // epsilon
			};
			FormalLanguage fl = new(dict);
			Console.WriteLine(fl.OutputLeft());

			Console.WriteLine("Подпункт б)");

			dict = new()
			{
				new Rule("S", "A\u27C2"),   // \u27C2 - значок перпендикуляра
				new Rule("S", "B\u27C2"),
				new Rule("A", "a"),
				new Rule("A", "Ba"),
				new Rule("B", "b"),
				new Rule("B", "Bb"),
				new Rule("B", "Ab"),
			};
			fl = new(dict);
			Console.WriteLine(fl.OutputLeft());

			Grammar gr;
			Console.WriteLine("Задание 9.");
			//new Rule("bCD", "\u03B5"),  // epsilon
			//a\u03B5b\u03B5a\u03B5b\u03B5
			dict = new(){
				new Rule("S", "aSbS"),
				new Rule("S", "bSaS"),
				new Rule("S", "\u03B5"),
			};
			gr = new(
				new List<string> { "S" },
				new List<string> { "a", "b", "\u03B5" },
				dict);
				// aEbaEbE
			Console.WriteLine(gr.MakeTree("a\u03B5ba\u03B5b\u03B5"));

			Console.WriteLine("Задание 11.");
			Console.WriteLine("Подпункт а)");
			Console.WriteLine("Грамматика описывает язык 0^n 1^n 'Символ перепендикуляра' \u27C2");
			dict = new()
			{
				new Rule("S", "0S"),
				new Rule("S", "0B"),
				new Rule("B", "1B"),
				new Rule("B", "1C"),
				new Rule("C", "1C"),
				new Rule("C", "\u27C2"),

			};
			fl = new(dict);
			Console.WriteLine(fl.OutputLeft());

			dict = new()
			{
				new Rule("S", "A\u27C2"),
				new Rule("A", "A1"),
				new Rule("A", "CB1"),
				new Rule("B", "B1"),
				new Rule("B", "C1"),
				new Rule("B", "CB1"),
				new Rule("C", "0"),

			};
			fl = new(dict);
			Console.WriteLine(fl.OutputLeft());
			Console.WriteLine("Подпункт б)");
			Console.WriteLine("Грамматика описывает язык {a^n b^n} 'Символ перепендикуляра' \u27C2");
			dict = new()
			{
				new Rule("S", "aA"),
				new Rule("S", "aB"),
				new Rule("S", "bA"),
				new Rule("A", "bS"),
				new Rule("B", "aS"),
				new Rule("B", "bB"),
				new Rule("B", "\u27C2"),
				
			};
			fl = new(dict);
			Console.WriteLine(fl.OutputLeft());

			dict = new()
			{
				new Rule("S", "A\u27C2"),
				new Rule("A", "Ba"),
				new Rule("A", "Bb"),
				new Rule("A", "Ab"),
				new Rule("A", "ABa"),
				new Rule("A", "ABb"),
				new Rule("B", "a"),
				new Rule("B", "b"),

			};
			fl = new(dict);
			Console.WriteLine(fl.OutputLeft());
			Console.WriteLine("Задание 12.");
			dict = new()
			{
				new Rule("S", "S1"),
				new Rule("S", "A0"),
				new Rule("A", "A1"),
				new Rule("A", "0"),
			};
			fl = new(dict);
			Console.WriteLine(fl.OutputLeft());
			dict = new()
			{
				new Rule("S", "A1"),
				new Rule("S", "B0"),
				new Rule("S", "E1"),
				new Rule("A", "S1"),
				new Rule("B", "C1"),
				new Rule("B", "D1"),
				new Rule("C", "0"),
				new Rule("D", "B1"),
				new Rule("E", "E0"),
				new Rule("E", "1"),
			};
			fl = new(dict);
			Console.WriteLine(fl.OutputLeft());
			dict = new()
			{
				new Rule("S", "S1"),
				new Rule("S", "A0"),
				new Rule("S", "A1"),
				new Rule("A", "0"),
			};
			fl = new(dict);
			Console.WriteLine(fl.OutputLeft());

			Console.WriteLine();
			Console.WriteLine();
			Console.WriteLine("Лабораторная №2");
			Console.WriteLine("a) Определить тип грамматики.");
			dict = new() { 
				new Rule("S", "0S"),
				new Rule("S", "S0"),
				new Rule("S", "D"),
				new Rule("D", "DD"),
				new Rule("D", "1A"),
				new Rule("D", "\u03B5"),
				new Rule("A", "0B"),
				new Rule("A", "\u03B5"),
				new Rule("B", "0A"),
				new Rule("B", "0"),
			};
			gr = new(
				new List<string> { "S", "D", "A", "B" },
				new List<string> { "0", "1", "\u03B5" },
				dict);

			Console.WriteLine(gr.GetTypeGrammar());
			Console.WriteLine("б) Определить язык.");
			fl = new(dict);
			for (int i = 0; i < 10; i++)
			{
				Console.WriteLine(fl.OutputLeft());
			}
			Console.WriteLine("в) Написать р-грамматику почти эквивалентную данной.");
			dict = new()
			{
				new Rule("S", "S0"),
				new Rule("S", "D0"),
				new Rule("S", "D1"),
				new Rule("D", "D0"),
				new Rule("D", "A1"),
				new Rule("A", "B0"),
				new Rule("B", "A0"),
				new Rule("B", "0"),
			};
			fl = new(dict);
			Console.WriteLine(fl.OutputLeft());
			Console.WriteLine("г) Построить ДС анализатор.");
			AnalizeToConsole("\u03B5101010\u03B51101010");
		}

		public enum State { H, A, D, B, S, ER }
		public static void AnalizeToConsole(string text)
		{
			State st = State.H;
			int count = 0;

			do
			{
				switch (st)
				{
					case State.H:
						{
							if (text[count] == '\u03B5')
							{
								var rand = new Random();
								var i = rand.Next(0, 2);
								if (i == 0) st = State.A;
								else if (i == 1) st = State.D;
								else st = State.ER;
							}
							break;
						}
					case State.A:
						{
							if (text[count] == '0') st = State.B;
							else st = State.ER;
							break;
						}
					case State.D:
						{
							if (text[count] == '1') st = State.A;
							else if (text[count] == ' ') st = State.D;  // как отобразить переход из D в DD?
							else st = State.ER;
							break;
						}
					case State.B:
						{
							if (text[count] == '0') st = State.A;
							else st = State.ER;
							break;
						}
					case State.S:
						{
							if (text[count] == '0') st = State.S;
							if (text[count] == ' ') st = State.D;   // как отобразиь переход в D без терминального символа?
							else st = State.ER;
							break;
						}
					default:
						break;
				}

				Console.WriteLine(st);
				count++;

			} while (st != State.S && st != State.ER);
		}
	}
}
