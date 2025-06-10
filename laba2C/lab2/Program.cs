using System;
using System.Diagnostics;
using System.Linq;

namespace Lab
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Write("Введіть кількість потоків (натисніть Enter для значення за замовчуванням = 8): ");
            string input = Console.ReadLine();
            int threadCount = 8;
            if (!string.IsNullOrWhiteSpace(input))
            {
                if (!int.TryParse(input, out threadCount) || threadCount <= 0)
                {
                    Console.WriteLine("Некоректне значення! Використовується значення за замовчуванням: 8");
                    threadCount = 8;
                }
            }

            double[] array = GetRandomArray(12_000_000);

            Console.WriteLine("\nПошук мінімального значення (багатопотоковий режим):");
            Stopwatch stopwatch = Stopwatch.StartNew();
            ThreadController threadController = new ThreadController(threadCount, array);
            var minResult = threadController.GetMinWithIndex();
            stopwatch.Stop();

            Console.WriteLine($"Мінімальне значення: {minResult.Value}");
            Console.WriteLine($"Індекс мінімального значення: {minResult.Index}");
            Console.WriteLine($"Час виконання: {stopwatch.ElapsedMilliseconds} мс");

            Console.WriteLine("\nПеревірка результату (стандартний метод):");
            stopwatch.Restart();
            double min = array.Min();
            stopwatch.Stop();

            Console.WriteLine($"Мінімальне значення: {min}");
            Console.WriteLine($"Час виконання: {stopwatch.ElapsedMilliseconds} мс");

            Console.WriteLine($"\nРезультати збігаються: {(Math.Abs(minResult.Value - min) < 1e-10 ? "Так" : "Ні")}");

            Console.WriteLine("\nНатисніть будь-яку клавішу для завершення...");
            Console.ReadKey();
        }

        private static double[] GetRandomArray(int count)
        {
            Random random = new Random();
            double[] array = new double[count];

            for (int i = 0; i < count; i++)
            {
                array[i] = random.NextDouble() * 5000;
            }

            int negativeIndex = random.Next(count);
            array[negativeIndex] = -(random.NextDouble() * 5000);

            return array;
        }
    }
}
