using System.Diagnostics;

namespace MarriageAlg
{
	internal class Program
	{
		public static void Main(string[] args)
		{
			// Участники и их предпочтения
			var men = new List<string> { "M1", "M2", "M3" };
			var women = new List<string> { "W1", "W2", "W3" };

			var menPreferences = new Dictionary<string, List<string>>
		{
			{ "M1", new List<string> { "W1", "W2", "W3" } },
			{ "M2", new List<string> { "W2", "W1", "W3" } },
			{ "M3", new List<string> { "W3", "W2", "W1" } }
		};

			var womenPreferences = new Dictionary<string, List<string>>
		{
			{ "W1", new List<string> { "M2", "M1", "M3" } },
			{ "W2", new List<string> { "M1", "M3", "M2" } },
			{ "W3", new List<string> { "M3", "M2", "M1" } }
		};

			var stopwatch = Stopwatch.StartNew(); // Начинаем измерение времени

			var stableMatches = FindStableMatches(men, women, menPreferences, womenPreferences);

			stopwatch.Stop(); // Останавливаем измерение времени

			Console.WriteLine("Stable Matches:");
			foreach (var match in stableMatches)
			{
				Console.WriteLine($"{match.Key} is paired with {match.Value}");
			}

			Console.WriteLine($"Execution Time: {stopwatch.ElapsedMilliseconds} ms");
		}

		public static Dictionary<string, string> FindStableMatches(
			List<string> men,
			List<string> women,
			Dictionary<string, List<string>> menPreferences,
			Dictionary<string, List<string>> womenPreferences)
		{
			// Инициализация
			var freeMen = new Queue<string>(men); // Все мужчины изначально свободны
			var womenPartners = new Dictionary<string, string>(); // Текущие партнеры женщин
			var menNextProposal = new Dictionary<string, int>(); // Индекс следующего предложения для каждого мужчины

			foreach (var man in men)
			{
				menNextProposal[man] = 0; // Все мужчины начинают с первого предпочтения
			}

			// Алгоритм предложений
			while (freeMen.Count > 0)
			{
				var man = freeMen.Dequeue();
				var womanIndex = menNextProposal[man];
				var woman = menPreferences[man][womanIndex];
				menNextProposal[man]++;

				if (!womenPartners.ContainsKey(woman))
				{
					// Женщина свободна, формируем пару
					womenPartners[woman] = man;
				}
				else
				{
					// Женщина уже имеет партнера, проверяем предпочтения
					var currentPartner = womenPartners[woman];
					if (PrefersNewPartner(woman, man, currentPartner, womenPreferences))
					{
						// Женщина выбирает нового партнера, освобождаем старого
						freeMen.Enqueue(currentPartner);
						womenPartners[woman] = man;
					}
					else
					{
						// Женщина остается с текущим партнером
						freeMen.Enqueue(man);
					}
				}
			}

			// Формируем результат
			var matches = new Dictionary<string, string>();
			foreach (var pair in womenPartners)
			{
				matches[pair.Value] = pair.Key;
			}

			return matches;
		}

		private static bool PrefersNewPartner(string woman, string newMan, string currentMan, Dictionary<string, List<string>> womenPreferences)
		{
			var preferences = womenPreferences[woman];
			return preferences.IndexOf(newMan) < preferences.IndexOf(currentMan);
		}
	}
}
