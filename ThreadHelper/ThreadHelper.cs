using Microsoft.VisualStudio.Threading;
using System;
using System.Globalization;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Yinyue200.Threading.UI
{
    public class ThreadHelper
    {
        public static void Init(ThreadHelper threadHelper,JoinableTaskContext joinableTaskContext,JoinableTaskFactory joinableTaskFactory)
        {
            Generic = threadHelper;
            JoinableTaskFactory = joinableTaskFactory;
            JoinableTaskContext = joinableTaskContext;
        }
        public static void Init(JoinableTaskContext joinableTaskContext)
        {
            var context = new JoinableTaskContext();
            Init(new ThreadHelper(), context, context.Factory);
        }
        public static ThreadHelper Generic { get; private set; }
        /// <summary>Gets the singleton <see cref="Microsoft.VisualStudio.Threading.JoinableTaskContext" /> instance for App.</summary>
        public static JoinableTaskContext JoinableTaskContext { get; private set; }

        public static JoinableTaskFactory JoinableTaskFactory { get; private set; }

        /// <summary>Determines whether the call is being made on the UI thread.</summary>
        /// <returns>Returns <see langword="true" /> if the call is on the UI thread, otherwise returns <see langword="false" />.</returns>
        public static bool CheckAccess() => JoinableTaskContext.IsOnMainThread;
        /// <summary>Determines whether the call is being made on the UI thread ,and throws COMException(RPC_E_WRONG_THREAD) if it is.</summary>
        /// <param name="callerMemberName">The optional name of caller if a Debug Assert is desired if on the UI thread.</param>
        /// <exception cref="COMException">Thrown with RPC_E_WRONG_THREAD when called on any thread other than the main UI thread.</exception>
        public static void ThrowIfOnUIThread([CallerMemberName] string callerMemberName = "")
        {
            if (!CheckAccess())
            {
                return;
            }
            string message = string.Format(CultureInfo.CurrentCulture, "{0} must be called on a background thread.", new object[]
            {
                callerMemberName
            });
            throw new COMException(message, -2147417842);
        }
        /// <summary>Determines whether the call is being made on the UI thread, and throws COMException(RPC_E_WRONG_THREAD) if it is not.</summary>
        /// <param name="callerMemberName">The optional name of caller if a Debug Assert is desired if not on the UI thread.</param>
        /// <exception cref="COMException">Thrown with RPC_E_WRONG_THREAD when called on any thread other than the main UI thread.</exception>
        public static void ThrowIfNotOnUIThread([CallerMemberName] string callerMemberName = "")
        {
            if (CheckAccess())
            {
                return;
            }
            string message = string.Format(CultureInfo.CurrentCulture, "{0} must be called on the UI thread.", new object[]
            {
                callerMemberName
            });
            throw new COMException(message, -2147417842);
        }
        /// <summary>Schedules an action for execution on the UI thread asynchronously.</summary>
        /// <param name="action">The action to run.</param>
        public void BeginInvoke(Action action)
        {
            JoinableTaskFactory.RunAsync(async()=>{
                await JoinableTaskFactory.SwitchToMainThreadAsync();
                action();
            }, JoinableTaskCreationOptions.LongRunning);
        }

        ///// <summary>Schedules an action for execution on the UI thread asynchronously.</summary>
        ///// <param name="priority">The priority at which to run the action.</param>
        ///// <param name="action">The action to run.</param>
        //public void BeginInvoke(DispatcherPriority priority, Action action)
        //{
        //    Dispatcher dispatcherForUIThread = ThreadHelper.DispatcherForUIThread;
        //    if (dispatcherForUIThread == null)
        //    {
        //        throw new InvalidOperationException(Resources.ThreadHelper_UIThreadDispatcherUnavailable);
        //    }
        //    dispatcherForUIThread.BeginInvoke(priority, action);
        //}
    }
}
