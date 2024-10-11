using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace Umeshu.Uf
{
    /// <summary>
    /// Provides utility methods for asynchronous operations.
    /// </summary>
    public static class UfAsync
    {

        public static void ExecuteNextFrame(System.Action _action) => ExecuteAfterMillisecondsDelay(1, _action);

        public static void ExecuteAfterMillisecondsDelay(int _millisecondsDelay, System.Action _action)
        {
            async void WaitAndExecute()
            {
                await Task.Delay(_millisecondsDelay);
                _action?.Invoke();
            }
            WaitAndExecute();
        }

        /// <summary>
        /// Launches multiple coroutines.
        /// </summary>
        /// <param name="_handler">The MonoBehaviour that will handle the coroutines.</param>
        /// <param name="_coroutines">The coroutines to launch.</param>
        public static IEnumerator LaunchMultipleCoroutines(MonoBehaviour _handler, params IEnumerator[] _coroutines)
        {
            List<CoroutinePlayer> _players = new();
            foreach (IEnumerator _coroutine in _coroutines)
            {
                _players.Add(new CoroutinePlayer(_handler, _coroutine));
            }
            while (true)
            {
                bool _aCoroutineIsInProcess = _players.Find(_x => !_x.IsFinished) != null;
                if (!_aCoroutineIsInProcess) break;
                else yield return null;
            }
        }

        /// <summary>
        /// Waits for multiple asynchronous operations to complete.
        /// </summary>
        /// <param name="_asyncOps">The asynchronous operations to wait for.</param>
        public static IEnumerator WaitMultipleAsyncOp(params AsyncOperation[] _asyncOps)
        {
            while (true)
            {
                bool _asyncAllFinished = _asyncOps.ToList().Find(_x => !_x.isDone) == null;
                if (_asyncAllFinished) break;
                else yield return null;
            }
        }

        /// <summary>
        /// Handles the execution of a coroutine.
        /// </summary>
        private class CoroutinePlayer
        {
            /// <summary>
            /// Gets a value indicating whether the coroutine has finished execution.
            /// </summary>
            public bool IsFinished { get; private set; } = false;

            /// <summary>
            /// Initializes a new instance of the CoroutinePlayer class.
            /// </summary>
            /// <param name="_handler">The MonoBehaviour that will handle the coroutine.</param>
            /// <param name="_coroutine">The coroutine to handle.</param>
            public CoroutinePlayer(MonoBehaviour _handler, IEnumerator _coroutine)
            {
                _handler.StartCoroutine(CoroutineHandling(_coroutine));
            }

            /// <summary>
            /// Handles the execution of the coroutine.
            /// </summary>
            /// <param name="_coroutine">The coroutine to handle.</param>
            private IEnumerator CoroutineHandling(IEnumerator _coroutine)
            {
                yield return _coroutine;
                IsFinished = true;
            }
        }
    }
}
