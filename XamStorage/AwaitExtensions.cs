using System;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace XamStorage
{
    /// <summary>
    /// Extensions for use internally by MyPCLStorage for awaiting.
    /// </summary>
    internal static partial class AwaitExtensions
    {
        /// <summary>
        /// Causes the caller who awaits this method to
        /// switch off the Main thread. It has no effect if
        /// the caller is already off the main thread.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>An awaitable that does the thread switching magic.</returns>
        internal static TaskSchedulerAwaiter SwitchOffMainThreadAsync(CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            return new TaskSchedulerAwaiter(
                SynchronizationContext.Current != null ? TaskScheduler.Default : null,
                cancellationToken);
        }

        internal struct TaskSchedulerAwaiter : INotifyCompletion
        {
            private TaskScheduler taskScheduler;
            private CancellationToken cancellationToken;

            internal TaskSchedulerAwaiter(TaskScheduler taskScheduler, CancellationToken cancellationToken)
            {
                this.taskScheduler = taskScheduler;
                this.cancellationToken = cancellationToken;
            }

            internal TaskSchedulerAwaiter GetAwaiter()
            {
                return this;
            }

            public bool IsCompleted {
                get { return taskScheduler == null; }
            }

            public void OnCompleted(Action continuation)
            {
                if (taskScheduler == null)
                {
                    throw new InvalidOperationException("IsCompleted is true, so this is unexpected.");
                }

                Task.Factory.StartNew(
                    continuation,
                    CancellationToken.None,
                    TaskCreationOptions.None,
                    taskScheduler);
            }

            public void GetResult()
            {
                cancellationToken.ThrowIfCancellationRequested();
            }
        }
    }
}
