using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;

namespace HopGame
{
    internal class Program
    {
        private static int k;
        private static int n;
        private static Status kChanged = Status.Initial;

        public static async Task Main()
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("Please enter number of players: ");
            var input = Console.ReadLine();
            while (!(Int32.TryParse(input, out n) && n >= 2))
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("Only numbers greater than 2 is allowed, try again.");
                input = Console.ReadLine();
            }

            List<Player> players = new List<Player>();
            Random random = new Random();
            for (int i = 1; i <= n; i++)
            {
                players.Add(new Player()
                {
                    Name = $"Player{i}",
                    Iq = random.NextDouble()
                });
            }

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("Now the Game is going to start!");
            Console.WriteLine();

            var tasks = new List<Task>();
            var source = new CancellationTokenSource();
            var token = source.Token;

            var timer = new System.Timers.Timer();

            timer.Interval = random.Next(500, 750);
            timer.Elapsed += Timer_Elapsed;
            timer.Start();
            tasks.Add(Task.Run(() => Timer_Elapsed(timer, null)));
            tasks.Add(Task.Run(() =>
            {
                Thread.Sleep(100);
                int counter = 0;
                while (players.Count > 1)
                {
                    if (k > 0)
                        foreach (var player in players.ToList())
                        {
                            if (kChanged == Status.Changed)
                            {
                                kChanged = Status.Unchanged;
                                break;
                            }
                            counter++;
                            var correctAnswer = counter % k == 0 ? "Hop" : counter.ToString();
                            string playerSelectedValue = counter % k != 0 ? counter.ToString() : GetCorrectAnswerProbability(player.Iq) ? "Hop" : counter.ToString();
                            Console.ForegroundColor = ConsoleColor.White;
                            Console.WriteLine($"{player.Name}: {playerSelectedValue}");
                            if (correctAnswer != playerSelectedValue)
                            {
                                Console.ForegroundColor = ConsoleColor.Red;
                                Console.WriteLine($"{player.Name} is losed!");
                                players.Remove(player);
                                n = players.Count;
                                if (n == 1)
                                    break;
                                continue;
                            }
                        }
                }
                timer.Stop();
                Console.WriteLine();
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"End of the Game. {players[0].Name} Won!!!");
                Console.ReadLine();
            }, token));
            Task.WaitAll(tasks.ToArray());
        }
        private static async void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            k = await GetHopNumber();
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine($"*** Hop Number is: {k} ***");
        }
        static async Task<int> GetHopNumber()
        {
            int oldK = k;
            Random random = new Random();
            k = random.Next(3, 10);
            while (k == n || k == oldK)
            {
                k = random.Next(3, 10);
            }
            kChanged = Status.Changed;
            return k;
        }
        private static bool GetCorrectAnswerProbability(double iq)
        {
            Random random = new Random();
            bool result = random.NextDouble() + iq > 1;
            return result;
        }
    }
    internal enum Status
    {
        Initial = 1,
        Unchanged = 2,
        Changed = 3
    }
}