﻿using System.Collections.Generic;
using System.Threading.Tasks;

namespace Challenge.Threading
{
    public class AsyncAutoResetEvent
    {
        private static readonly Task SCompleted = Task.FromResult(true);
        private readonly Queue<TaskCompletionSource<bool>> _mWaits = new Queue<TaskCompletionSource<bool>>();
        private bool _mSignaled;

        public Task WaitAsync()
        {
            lock (_mWaits)
            {
                if (_mSignaled)
                {
                    _mSignaled = false;
                    return SCompleted;
                }

                var tcs = new TaskCompletionSource<bool>();
                _mWaits.Enqueue(tcs);
                return tcs.Task;
            }
        }

        public void Set()
        {
            TaskCompletionSource<bool> toRelease = null;

            lock (_mWaits)
            {
                if (_mWaits.Count > 0)
                    toRelease = _mWaits.Dequeue();
                else if (!_mSignaled)
                    _mSignaled = true;
            }

            toRelease?.SetResult(true);
        }
    }
}