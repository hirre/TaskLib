using System;
using System.Threading;
using System.Threading.Tasks;
using TaskLibrary;

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
            .TimeoutAfter(TimeSpan.FromSeconds(5), TimeoutTask); // Timeout after 5 seconds (no cancellation token)


            Console.ReadKey();
        }

        /// <summary>
        ///     Timeout task.
        /// </summary>
        private static void TimeoutTask()
        {
            Console.WriteLine("TIMEOUT OCCURED!");
        }
    }
}
