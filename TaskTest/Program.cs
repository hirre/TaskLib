using System;
using System.Threading;
using System.Threading.Tasks;
using TaskLibrary;
using TaskExtensions = TaskLibrary.TaskExtensions;

namespace TaskTest
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            // Main task
            await Task.Run(() =>
            {
                Console.WriteLine("Main task running...");
                
                Thread.Sleep(10000); // Sleep for 10 seconds
                
            })
            .TimeoutAfter(TimeSpan.FromSeconds(5), () => TimeoutTask(1)); // Timeout after 5 seconds (no cancellation token)

            // Or a general timeout not bound to an external task (no cancellation token)
            TaskExtensions.CreateTimeout(TimeSpan.FromSeconds(5), () => TimeoutTask(2));

            Console.ReadKey();
        }

        /// <summary>
        ///     Timeout task.
        /// </summary>
        private static void TimeoutTask(int i)
        {
            Console.WriteLine($"TIMEOUT {i} OCCURED!");
        }
    }
}
