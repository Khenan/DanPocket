using System.Collections.Generic;
using System.Linq;
using Umeshu.Uf;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Umeshu.USystem.Scene
{
    public static class LoaderExtension
    {
        public static float Progress(params ILoader[] _loaders)
        {
            List<ILoader> _validLoaders = _loaders.Where(_l => !float.IsNaN(_l.Progress)).ToList();
            if (_validLoaders.Count == 0) return 1;

            float _totalWeight = _validLoaders.Sum(_l => _l.VisualWeightOnLoading);
            float _progress = 0;
            foreach (ILoader _loader in _validLoaders)
            {
                float _weight = _loader.VisualWeightOnLoading / _totalWeight;
                _progress += _loader.Progress * _weight;
            }
            return _progress;
        }
        public static bool Finished(params ILoader[] _loaders) => _loaders.All(_l => _l.IsDone);
    }
    public interface ILoader
    {
        public float Progress { get; }
        public bool IsDone { get; }
        public float VisualWeightOnLoading { get; }
    }
    public abstract class SceneOperator : ILoader
    {
        private List<AsyncOperation> operations = new();
        public float Progress => Uf.UfMath.GetAverage(operations.Select(_op => _op.progress));
        public bool IsDone => operations.All(_op => _op.isDone);
        public float VisualWeightOnLoading => 1;

        public void StartOperation(IEnumerable<string> _scenes)
        {
            ClearLoader();
            foreach (string _scene in _scenes)
            {
                if (SceneManager.GetSceneByBuildIndex(0).name == _scene) continue;
                AsyncOperation _operation = StartSceneOperation(_scene);
                if (_operation == null) continue;
                operations.Add(_operation);
            }
        }
        public void ClearLoader() => operations.Clear();

        protected abstract AsyncOperation StartSceneOperation(string _sceneName);
    }
}