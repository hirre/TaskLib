using System;
using System.Threading;
using System.Threading.Tasks;

namespace TaskLibrary
{
    /// <summary>
    ///     Extension class for tasks.
    /// </summary>
    public static class TaskExtensions
    {
        /// <summary>
        ///     This method creates a task that will asynchronously wait until any of these tasks have completed:
        ///     - The delayed task (represents the timeout with cancellation option)
        ///     - The main task
        ///     If the task that was completed happens to be the main task, we cancel any cancellation tokens wait for it to
        ///     complete and return its result.
        ///     If the task that was completed happens to be the delayed task (a timeout has occured) we call any existing timeout
        ///     handler
        ///     and return default TResult.
        /// </summary>
        /// <typeparam name="TResult">Return type of the task</typeparam>
        /// <param name="task">The main task</param>
        /// <param name="timeout">Timeout value</param>
        /// <param name="timeoutHandler">Timeout handler</param>
        /// <param name="cancellationTokenSourceAction">Cancellation token source</param>
        /// <returns>TResult (default value for TResult if timeout has occured)</returns>
        public static async Task<TResult> TimeoutAfter<TResult>(this Task<TResult> task,
            TimeSpan timeout,
            Action timeoutHandler = null,
            CancellationTokenSource cancellationTokenSourceAction = null)
        {
            var timeoutCancellationTokenSource = cancellationTokenSourceAction ?? new CancellationTokenSource();

            using (timeoutCancellationTokenSource)
            {
                var delayedTask = Task.Delay(timeout, timeoutCancellationTokenSource.Token);

                if (await Task.WhenAny(task, delayedTask) == task)
                {
                    timeoutCancellationTokenSource.Cancel();
                    return await task; // Very important in order to propagate exceptions
                }

                // Timeout occured, invoke handler if there is any
                if (timeoutHandler != null && !timeoutCancellationTokenSource.IsCancellationRequested)
                    await Task.Run(() => timeoutHandler.Invoke(), timeoutCancellationTokenSource.Token);

                return default;
            }
        }

        /// <summary>
        ///     This method creates a task that will asynchronously wait until any of these tasks have completed:
        ///     - The delayed task (represents the timeout with cancellation option)
        ///     - The main task
        ///     If the task that was completed happens to be the main task, we cancel any cancellation tokens wait for it to
        ///     complete and return its result.
        ///     If the task that was completed happens to be the delayed task (a timeout has occured) we call any existing timeout
        ///     handler.
        /// </summary>
        /// <param name="task">The main task</param>
        /// <param name="timeout">Timeout value</param>
        /// <param name="timeoutHandler">Timeout handler</param>
        /// <param name="cancellationTokenSourceAction">Cancellation token source</param>
        public static async Task TimeoutAfter(this Task task,
            TimeSpan timeout,
            Action timeoutHandler = null,
            CancellationTokenSource cancellationTokenSourceAction = null)
        {
            var timeoutCancellationTokenSource = cancellationTokenSourceAction ?? new CancellationTokenSource();

            using (timeoutCancellationTokenSource)
            {
                var delayedTask = Task.Delay(timeout, timeoutCancellationTokenSource.Token);

                if (await Task.WhenAny(task, delayedTask) == task)
                {
                    timeoutCancellationTokenSource.Cancel();
                    await task; // Very important in order to propagate exceptions
                }
                else
                {
                    // Timeout occured, invoke handler if there is any
                    if (timeoutHandler != null && !timeoutCancellationTokenSource.IsCancellationRequested)
                        await Task.Run(() => timeoutHandler.Invoke(), timeoutCancellationTokenSource.Token);
                }
            }
        }
    }
}