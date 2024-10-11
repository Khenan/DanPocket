using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

namespace Umeshu.Uf
{
    public static class UfLoad
    {
        private static IEnumerator TryGetAssets<T>(string _path, RunTimeAssetRequest<T> _runTimeAssetRequest) where T : UnityEngine.Object
        {
            string _folder = Path.GetDirectoryName(Application.dataPath) + _path;
            if (Directory.Exists(_folder)) yield return HandleDirectory(_folder, _runTimeAssetRequest);
            else if (!Application.isEditor) UnityEngine.Debug.Log("No Folder at " + _folder);
            _runTimeAssetRequest.MethodToDoAtEnd?.Invoke(_runTimeAssetRequest.Assets);
        }

        private static IEnumerator HandleDirectory<T>(string _path, RunTimeAssetRequest<T> _runTimeAssetRequest) where T : UnityEngine.Object
        {
            string _filter = _runTimeAssetRequest.GetFilter();
            string[] _filePaths = Directory.GetFiles(_path, _filter);

            foreach (string _filePath in _filePaths)
            {
                UnityWebRequest _webRequest = _runTimeAssetRequest.GetWebRequest(_filePath);
                if (_webRequest != null)
                {
                    yield return _webRequest.SendWebRequest();
                    if (_webRequest.result != UnityWebRequest.Result.Success) UnityEngine.Debug.Log(_webRequest.error);
                    else _runTimeAssetRequest.AddAssetToList(_path, _filePath, _runTimeAssetRequest.GetContentMethod().Invoke(_webRequest));
                }
                else _runTimeAssetRequest.AddAssetToList(_path, _filePath, _runTimeAssetRequest.CustomGet(_filePath));
            }

            string[] _directories = Directory.GetDirectories(_path);
            foreach (string _directory in _directories) { yield return HandleDirectory(_directory, _runTimeAssetRequest); }
        }


        public static IEnumerator LoadInFolder_AudioClips(string _path, AudioType _audioType, Action<List<AudioClip>> _methodAtEnd) => TryGetAssets(_path, new RunTimeAssetRequest_AudioClip(_methodAtEnd, _audioType));
        public static IEnumerator LoadInFolder_Texture2D(string _path, Action<List<Texture2D>> _methodAtEnd) => TryGetAssets(_path, new RunTimeAssetRequest_Texture2D(_methodAtEnd));
        public static IEnumerator LoadInFolder_TSV(string _path, Action<List<TextAsset>> _methodAtEnd) => TryGetAssets(_path, new RunTimeAssetRequest_TSV(_methodAtEnd));
        public abstract class RunTimeAssetRequest<T> where T : UnityEngine.Object
        {
            public RunTimeAssetRequest(Action<List<T>> _methodAtEnd) => MethodToDoAtEnd = _methodAtEnd;
            public List<T> Assets { get; private set; } = new();
            public Action<List<T>> MethodToDoAtEnd { get; private set; } = null;
            public abstract Func<UnityWebRequest, T> GetContentMethod();
            public abstract string GetFilter();
            public abstract UnityWebRequest GetWebRequest(string _path);
            public virtual T CustomGet(string _path) => default;
            public void AddAssetToList(string _folderPath, string _path, T _asset)
            {
                _asset.name = _path.Replace(_folderPath + @"\", "").Replace(GetFilter().Replace("*", ""), "");
                Assets.Add(_asset);
            }
        }

        public class RunTimeAssetRequest_AudioClip : RunTimeAssetRequest<AudioClip>
        {
            private readonly AudioType audioType;
            public RunTimeAssetRequest_AudioClip(Action<List<AudioClip>> _methodAtEnd, AudioType _audioType) : base(_methodAtEnd) => this.audioType = _audioType;
            public override Func<UnityWebRequest, AudioClip> GetContentMethod() => DownloadHandlerAudioClip.GetContent;
            public override string GetFilter() => "*." + audioType.ToString().ToLower();
            public override UnityWebRequest GetWebRequest(string _path) => UnityWebRequestMultimedia.GetAudioClip(_path, audioType);
        }

        public class RunTimeAssetRequest_Texture2D : RunTimeAssetRequest<Texture2D>
        {
            public RunTimeAssetRequest_Texture2D(Action<List<Texture2D>> _methodAtEnd) : base(_methodAtEnd) { }
            public override Func<UnityWebRequest, Texture2D> GetContentMethod() => DownloadHandlerTexture.GetContent;
            public override string GetFilter() => "*.png";
            public override UnityWebRequest GetWebRequest(string _path) => UnityWebRequestTexture.GetTexture(_path);
        }

        public class RunTimeAssetRequest_TSV : RunTimeAssetRequest<TextAsset>
        {
            public RunTimeAssetRequest_TSV(Action<List<TextAsset>> _methodAtEnd) : base(_methodAtEnd) { }
            public override Func<UnityWebRequest, TextAsset> GetContentMethod() => null;
            public override string GetFilter() => "*.tsv";
            public override UnityWebRequest GetWebRequest(string _path) => null;
            public override TextAsset CustomGet(string _path) => new(File.ReadAllText(_path));
        }
    }
}
