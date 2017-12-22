using Microsoft.VisualStudio.Threading;

namespace Yinyue200.Threading.UI
{
    public abstract class ThreadHelper
    {
        public static JoinableTaskContext JoinableTaskContext { get; set; }

        public static JoinableTaskFactory JoinableTaskFactory => JoinableTaskContext.Factory;
    }
}
