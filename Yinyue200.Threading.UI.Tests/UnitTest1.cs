using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Xunit;

namespace Yinyue200.Threading.UI.Tests
{
    public class UnitTest1
    {
        public UnitTest1()
        {
            ThreadHelper.Init(new Microsoft.VisualStudio.Threading.JoinableTaskContext());
        }
        [Fact]
        public async Task ThrowIfNotOnUIThreadTest()
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            await Task.Run(() =>
            {
                Assert.Throws<COMException>(() =>
                {
                    ThreadHelper.ThrowIfNotOnUIThread();
                });
            });
        }
        [Fact]
        public async Task ThrowIfOnUIThreadTest()
        {
            Assert.Throws<COMException>(() =>
            {
                ThreadHelper.ThrowIfOnUIThread();
            });
            await Task.Run(() =>
            {
                ThreadHelper.ThrowIfOnUIThread();
            });
        }
        [Fact]
        public async Task CheckAccessTest()
        {
            Assert.True(ThreadHelper.CheckAccess());
            await Task.Run(() =>
            {
                Assert.False(ThreadHelper.CheckAccess());
            });
        }
        [Fact]
        public async Task BeginInvokeTest()
        {
            await Task.Run(() =>
            {
                ThreadHelper.Generic.BeginInvoke(() =>
                {
                    Assert.True(ThreadHelper.CheckAccess());
                });
            });
        }
    }
}
