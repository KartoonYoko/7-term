﻿using System;
using System.Text;
using System.Collections.Generic;

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
			/// <returns>true - если правило зацикливает перевод, иначе - false</returns>
			private bool CheckLoop(string input, Rule rule, int count = 5) {
				for (int i = 0; i < count; i++) {
					string key = rule.Key;
					string value = rule.Value;

					int pos = input.IndexOf(key);

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

					// проверка для третьего типа TODO подправить работает неверно
					if (isEachTermPosBigger || isEachTermPosSmaller)
					{
						List<int> terminlPositions = new();
						List<int> nonTerminlPositions = new();
						foreach (string vn in Nonterminal)
						{
							nonTerminlPositions.Add(r.Value.IndexOf(vn));
						}
						foreach (string vt in Terminal)
						{
							terminlPositions.Add(r.Value.IndexOf(vt));
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
				string res = "0";
				if (isTypeOne) res += " 1";
				if (isTypeTwo) res += " 2";
				if (isTypeThree) res += " 3";
				return res;
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
			Console.WriteLine(fl.Translate("S"));

			Console.WriteLine("Подпункт б)");

			dict = new()
			{
				new Rule("S", "A\u27C2"),   // \u27C2 - значок перпендикуляра
				new Rule("S", "B\u27C2"),
				new Rule("A", "a"),
				new Rule("A", "Ba"),
				new Rule("b", "b"),
				new Rule("b", "Bb"),
				new Rule("b", "Ab"),
			};
			fl = new(dict);
			Console.WriteLine(fl.Translate("S"));

			Console.WriteLine("");
			Console.WriteLine("Задание 3.");
			Console.WriteLine("Подпункт a)");
			Console.WriteLine("Грамматика: G: ({a, b, c}, {A, B, C}, P, S)");
			dict = new()
			{
				new Rule("S", "aaB"),
				new Rule("B", "bCCCC"),
				new Rule("B", "b"),
				new Rule("C", "Cc"),
				new Rule("C", "c"),
			};
			fl = new(dict);
			Console.WriteLine(fl.Translate("S"));

			Console.WriteLine("Подпункт б)");
			Console.WriteLine("Грамматика: G: ({0, 10}, {A, B}, P, S)");
			dict = new()
			{
				new Rule("S", "0AB"),
				new Rule("A", "000"),
				new Rule("B", "1010"),
			};
			fl = new(dict);
			Console.WriteLine(fl.Translate("S"));

			Console.WriteLine("Подпункт в)");
			Console.WriteLine("Грамматика: G: ({0, 1}, {A, B}, P, S)");
			dict = new()
			{
				new Rule("S", "AB"),
				new Rule("A", "1001010"),
				new Rule("B", "0101001"),
			};
			fl = new(dict);
			Console.WriteLine(fl.Translate("S"));

			Console.WriteLine("");

			Console.WriteLine("Задание 3.");
			Console.WriteLine("Подпункт a)");
			dict = new()
			{
				new Rule("S", "0A1"),
				new Rule("S", "01"),
				new Rule("0A", "00A1"),
				new Rule("A", "01"),
			};
			Grammar gr = new(
				new List<string> { "S", "A" },
				new List<string> { "0", "1" },
				dict);
			Console.WriteLine(gr.GetTypeGrammar());
			Console.WriteLine("Подпункт б)");
			dict = new()
			{
				new Rule("S", "Ab"),
				new Rule("A", "Aa"),
				new Rule("A", "ba"),
			};
			gr = new(
				new List<string> { "S", "A" },
				new List<string> { "a", "b" },
				dict);
			Console.WriteLine(gr.GetTypeGrammar());

		}
	}
}
