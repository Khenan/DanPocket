using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;
using System.Linq;
using Object = UnityEngine.Object;
using Unity.VisualScripting;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditorInternal;
#endif

namespace Umeshu.Uf
{
    public static class UfEditor
    {

#if UNITY_EDITOR
        #region Gizmos

        public static void DrawText(Vector3 _position, string _text, Color _color, float _size = 12, TextAnchor _alignment = TextAnchor.MiddleCenter)
        {
            GUIStyle _style = new();
            _style.normal.textColor = _color;
            _style.fontSize = Mathf.RoundToInt(_size);
            _style.alignment = _alignment;

            Camera _sceneCamera = SceneView.currentDrawingSceneView.camera;

            if (_sceneCamera != null)
            {
                float _distance = Vector3.Distance(_sceneCamera.transform.position, _position);
                float _scale = _distance / 10f;
                _style.fontSize = Mathf.RoundToInt(_size / _scale);
            }

            Handles.Label(_position, _text, _style);
        }

        public static void DrawCone(Vector2 _startPosition, Vector2 _direction, float _radius, float _angle)
        {
            if (_angle == 360f)
            {
                Gizmos.DrawWireSphere(_startPosition, _radius);
                return;
            }
            List<Vector2> _points = UfMath.GetPointsOfCone(_startPosition, _direction, _radius, _angle);

            for (int _i = 0; _i < _points.Count; _i++)
            {
                Vector2 _point = _points[_i];
                Vector2 _nextPoint = _points[(_i + 1) % _points.Count];
                Gizmos.DrawLine(_point, _nextPoint);
            }
        }

        public static void DrawCircle(Vector2 _center, float _radius, Color _color, float _resolution = 10) => UfMath.GetPointsOfCircle(_center, _radius, _resolution).DrawLineOnVector2List(true, _color);

        public static void DrawLineOnVector2List(this List<Vector2> _points, bool _looping, Color _color)
        {
            Color _gizmoColor = Gizmos.color;
            Gizmos.color = _color;

            for (int _i = 0; _i < (_looping ? _points.Count : _points.Count - 1); _i++)
            {
                Vector2 _point = _points[_i];
                Vector2 _nextPoint = _points[(_i + 1) % _points.Count];
                Gizmos.DrawLine(_point, _nextPoint);
            }

            Gizmos.color = _gizmoColor;
        }

        public static void DrawArrow(Vector2 _startPosition, Vector2 _endPosition, Color _color, float _arrowBorderSize = 1)
        {
            Color _gizmoColor = Gizmos.color;
            Gizmos.color = _color;
            Vector2 _dir = _endPosition - _startPosition;
            Gizmos.DrawLine(_startPosition, _endPosition);

            Vector2 _topArrowDecal = Quaternion.AngleAxis(-45, Vector3.forward) * _dir.normalized * _arrowBorderSize;
            Vector2 _botArrowDecal = Quaternion.AngleAxis(45, Vector3.forward) * _dir.normalized * _arrowBorderSize;
            Gizmos.DrawLine(_endPosition, _endPosition - _topArrowDecal);
            Gizmos.DrawLine(_endPosition, _endPosition - _botArrowDecal);

            Gizmos.color = _gizmoColor;
        }

        public static void DrawSquareGizmos(Vector3 _pos, Vector2 _size, Color _col) => DrawSquareGizmos(_pos, _size.x, _size.y, _col, Vector2.up);
        public static void DrawSquareGizmos(Vector3 _pos, float _sizeX, float _sizeY, Color _col) => DrawSquareGizmos(_pos, _sizeX, _sizeY, _col, Vector2.up);

        public static void DrawCameraBounds(float _camFOV, float _camDist, Vector3 _position, Color _col)
        {
            float _verticalSize = UfCamera.GetVerticalCamSize(_camFOV, _camDist);
            DrawSquareGizmos(_position, _verticalSize * (16 / 9f), _verticalSize, _col);
        }

        #region Barred Square Gizmos

        public static void DrawBarredSquareGizmos(Vector3 _pos, Vector2 _size, Color _col, float _resolution, float _lineRotation) => DrawBarredSquareGizmos(_pos, _size.x, _size.y, _col, _resolution, _lineRotation);
        public static void DrawBarredSquareGizmos(Vector2 _pos, float _sizeX, float _sizeY, Color _col, float _resolution, float _lineRotation)
        {
            UfEditor.DrawSquareGizmos(_pos, _sizeX, _sizeY, _col);

            Gizmos.color = _col;

            Vector2 _topLeftCorner = _pos + new Vector2(-_sizeX / 2, _sizeY / 2);
            Vector2 _botRightCorner = _pos + new Vector2(_sizeX / 2, -_sizeY / 2);

            float _distance = Vector2.Distance(_topLeftCorner, _botRightCorner);
            float _nbLine = _distance * _resolution;

            _lineRotation = _lineRotation.RepeatBetween(0, -90);

            for (int _i = 0; _i <= _nbLine; _i++)
            {
                float _ratio = _i / (float)(_nbLine - 1);

                Vector2 _linePosition = Vector2.Lerp(_topLeftCorner, _botRightCorner, _ratio);

                Vector2 _lineVector = Quaternion.AngleAxis(_lineRotation, Vector3.forward) * Vector3.up;
                Vector2 _linePointA = _linePosition + _lineVector * _distance;
                Vector2 _linePointB = _linePosition - _lineVector * _distance;

                FitLineInsideRectangle(_linePointA, _linePointB, _pos, new Vector2(_sizeX, _sizeY), out Vector2 _clippedStart, out Vector2 _clippedEnd);
                Gizmos.DrawLine(_clippedStart, _clippedEnd);
            }

        }


        public static void FitLineInsideRectangle(Vector2 _pointA, Vector2 _pointB, Vector2 _rectPosition, Vector2 _rectDimension, out Vector2 _pointAOut, out Vector2 _pointBOut)
        {
            float _xMin = _rectPosition.x - _rectDimension.x / 2;
            float _yMin = _rectPosition.y - _rectDimension.y / 2;
            float _xMax = _rectPosition.x + _rectDimension.x / 2;
            float _yMax = _rectPosition.y + _rectDimension.y / 2;

            float _p1x = _pointA.x;
            float _p1y = _pointA.y;
            float _p2x = _pointB.x;
            float _p2y = _pointB.y;

            float _t0 = 0.0f, _t1 = 1.0f;
            float _dx = _p2x - _p1x;
            float _dy = _p2y - _p1y;

            if (ClipTest(-_dx, _p1x - _xMin, ref _t0, ref _t1) &&
                ClipTest(_dx, _xMax - _p1x, ref _t0, ref _t1) &&
                ClipTest(-_dy, _p1y - _yMin, ref _t0, ref _t1) &&
                ClipTest(_dy, _yMax - _p1y, ref _t0, ref _t1))
            {
                if (_t1 < 1.0f)
                {
                    _p2x = _p1x + _t1 * _dx;
                    _p2y = _p1y + _t1 * _dy;
                }
                if (_t0 > 0.0f)
                {
                    _p1x += _t0 * _dx;
                    _p1y += _t0 * _dy;
                }
                _pointAOut = new Vector2(_p1x, _p1y);
                _pointBOut = new Vector2(_p2x, _p2y);
            }
            else
            {
                // The line segment is completely outside the rectangle
                _pointAOut = _pointBOut = Vector2.zero;
            }
        }

        private static bool ClipTest(float _p, float _q, ref float _t0, ref float _t1)
        {
            float _r;
            if (_p < 0.0f)
            {
                _r = _q / _p;
                if (_r > _t1) return false;
                else if (_r > _t0) _t0 = _r;
            }
            else if (_p > 0.0f)
            {
                _r = _q / _p;
                if (_r < _t0) return false;
                else if (_r < _t1) _t1 = _r;
            }
            else if (_q < 0.0f)
            {
                return false;
            }
            return true;
        }

        #endregion

        public static void DrawOrthographicCameraBounds(float _verticalSize, Vector3 _position, Color _col) => DrawSquareGizmos(_position, _verticalSize * (16 / 9f), _verticalSize, _col);

        public static void DrawSquareGizmos(Vector3 _pos, float _sizeX, float _sizeY, Color _col, Vector2 _up)
        {
            Gizmos.color = _col;
            Vector3 _right = Quaternion.AngleAxis(-90, Vector3.forward) * _up;
            Vector3 _decalUp = _sizeY / 2 * _up;
            Vector3 _decalRight = _sizeX / 2 * _right;
            Vector3 _decalBot = _sizeY / 2 * -_up;
            Vector3 _decalLeft = _sizeX / 2 * -_right;
            Gizmos.DrawLine(_pos + _decalLeft + _decalUp, _pos + _decalRight + _decalUp);
            Gizmos.DrawLine(_pos + _decalLeft + _decalBot, _pos + _decalRight + _decalBot);
            Gizmos.DrawLine(_pos + _decalLeft + _decalBot, _pos + _decalLeft + _decalUp);
            Gizmos.DrawLine(_pos + _decalRight + _decalBot, _pos + _decalRight + _decalUp);
        }

        public static void DrawMultiLine(Vector2 _posA, Vector2 _posB, uint _nbLine, float _thickness, Color _color)
        {
            if (_nbLine == 0) return;
            Color _gizmoColor = Gizmos.color;
            Gizmos.color = _color;
            if (_nbLine == 1)
            {
                Gizmos.DrawLine(_posA, _posB);
                Gizmos.color = _gizmoColor;
                return;
            }
            Vector2 _dir = _posB - _posA;
            Vector2 _decalDir = Quaternion.AngleAxis(-90, Vector3.forward) * _dir.normalized;
            float _step = _thickness / (float)(_nbLine - 1f);
            for (int _i = 0; _i < _nbLine; _i++)
            {
                Vector2 _decal = (_i * _step - _thickness / 2) * _decalDir;
                Gizmos.DrawLine(_posA + _decal, _posB + _decal);
            }
            Gizmos.color = _gizmoColor;
        }
        #endregion

#endif
        public static void QuitApplication()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }

#if UNITY_EDITOR

        #region Prefab Root
        public static bool ObjectIsPrefabRoot(this GameObject _gameObject)
        {
            PrefabStage _stage = PrefabStageUtility.GetCurrentPrefabStage();
            return _stage != null && (_stage.openedFromInstanceRoot != null || _gameObject.transform.parent == null) && !UnityEngine.Application.isPlaying;
        }

        public static bool ParentWithComponentIsPrefabRoot<T>(this GameObject _gameObject) where T : Component
        {
            Transform _currParent = _gameObject.transform.parent;
            T _component = null;
            while (_currParent != null && _component == null)
                if (!_currParent.TryGetComponent(out _component))
                    _currParent = _currParent.parent;
            return _currParent != null && _component != null && ObjectIsPrefabRoot(_component.gameObject);
        }
        #endregion

        #region Get Assets
        public static List<T> GetAllPrefabsOfType<T>() where T : Component
        {
            List<T> _allInstancesOfComponent = new();
            string[] _allPrefabs = AssetDatabase.FindAssets("t:Prefab");
            for (int _i = 0; _i < _allPrefabs.Length; _i++)
            {
                _allPrefabs[_i] = AssetDatabase.GUIDToAssetPath(_allPrefabs[_i]);
                GameObject _prefab = AssetDatabase.LoadAssetAtPath<GameObject>(_allPrefabs[_i]);
                if (_prefab.TryGetComponent(out T _component))
                {
                    _allInstancesOfComponent.Add(_component);
                }
            }
            return _allInstancesOfComponent;
        }
        public static T GetScriptableObjectInstanceInProject<T>() where T : ScriptableObject
        {
            return GetAllAssetsOfType<T>().FirstOrDefault();
        }
        public static string GetAssetPath(Object _instance)
        {
            return AssetDatabase.GetAssetPath(_instance);
        }
        public static string GetScriptableObjectAssetPath<T>() where T : ScriptableObject
        {
            return GetAssetPath(GetScriptableObjectInstanceInProject<T>());
        }


        public static void EnsureComponentDepecency<TKey, TComponent>() where TKey : Component where TComponent : Component => EnsureComponentDepecency(typeof(TKey), typeof(TComponent));

        public static void EnsureComponentDepecencies(params (Type, Type)[] _depedencies) => _depedencies.ForEach(_depedency => EnsureComponentDepecency(_depedency.Item1, _depedency.Item2));

        public static void EnsureComponentDepecency(Type _typeKey, Type _typeToAdd)
        {
            DoMethodOnAllComponentsInPrefabs(_typeKey, true, (_prefab, _gameObject, _instance) =>
            {
                if (!_instance.TryGetComponent(_typeToAdd, out _))
                {
                    _instance.gameObject.AddComponent(_typeToAdd);
                    EditorUtility.SetDirty(_instance);
                    Debug.Log($"Added component {_typeToAdd.Name.Color(Color.cyan)} to {_gameObject.GetHyperLink()} in prefab {_prefab.GetHyperLink()} - Hierarchy path is {_gameObject.GetHierarchyPath().Color(Color.yellow)}");
                }
            });

            AssetDatabase.SaveAssets();
        }

        public static void LogComponentsInPrefab(bool _excludeVariants, params Type[] _typeKeys)
        {
            foreach (Type _typeKey in _typeKeys)
            {
                List<string> _existingComponents = new();
                DoMethodOnAllComponentsInPrefabs(_typeKey, _excludeVariants, (_prefab, _gameObject, _instance) =>
                _existingComponents.Add($"{_gameObject.GetHyperLink()} in prefab {_prefab.GetHyperLink()} - Hierarchy path is {_gameObject.GetHierarchyPath().Color(Color.yellow)}"));
                $"{$"Existing {_typeKey.Name.Quote()} components are ".Color(Color.cyan)} {_existingComponents.ToCollectionString()}".Log();
            }
        }

        public static void DoMethodOnAllComponentsInPrefabs(Type _typeKey, bool _excludeVariants, Action<GameObject, GameObject, Component> _action)
        {
            List<Transform> _projectPrefabs = GetAllPrefabsOfType<Transform>();

            foreach (Transform _prefab in _projectPrefabs)
            {
                if (!AssetDatabase.GetAssetPath(_prefab.gameObject).Contains("Assets")) continue;
                if (_excludeVariants && PrefabUtility.IsPartOfVariantPrefab(_prefab.gameObject)) continue;

                AssetDatabase.OpenAsset(_prefab.gameObject);
                GameObject _currentPrefab = GetCurrentOpenedPrefab();
                string _prefabPath = AssetDatabase.GetAssetPath(_prefab.gameObject);

                foreach (Component _instance in _prefab.GetComponentsInChildren(_typeKey))
                {
                    if (_excludeVariants && PrefabUtility.IsPartOfVariantPrefab(_instance.gameObject)) continue;

                    _action?.Invoke(_prefab.gameObject, _instance.gameObject, _instance);
                }
            }

            StageUtility.GoBackToPreviousStage();
        }

        public static List<T> GetAllAssetsOfType<T>(params string[] _pathFilters) where T : UnityEngine.Object
        {
            List<T> _allInstancesType = new();
            string[] _assets = AssetDatabase.FindAssets("t:" + typeof(T).Name);
            for (int _i = 0; _i < _assets.Length; _i++)
            {
                _assets[_i] = AssetDatabase.GUIDToAssetPath(_assets[_i]);
                bool _valid = true;
                foreach (string _pathFilter in _pathFilters)
                {
                    if (!_assets[_i].Contains(_pathFilter))
                    {
                        _valid = false;
                        break;
                    }
                }
                if (_valid)
                {
                    _allInstancesType.Add(AssetDatabase.LoadAssetAtPath<T>(_assets[_i]));
                }
            }
            return _allInstancesType;
        }

        public static T GetAssetOfType<T>(string _assetName = "", string _pathToLookAt = "") where T : UnityEngine.Object => (T)GetAssetOfType(typeof(T), _assetName, _pathToLookAt);

        public static UnityEngine.Object GetAssetOfType(Type _type, string _assetName = "", string _pathToLookAt = null)
        {
            string[] _assets = !string.IsNullOrEmpty(_pathToLookAt)
                ? AssetDatabase.FindAssets("t:" + _type.Name, new string[] { _pathToLookAt })
                : AssetDatabase.FindAssets("t:" + _type.Name);

            for (int _i = 0; _i < _assets.Length; _i++)
            {
                _assets[_i] = AssetDatabase.GUIDToAssetPath(_assets[_i]);
                string _currAssetName = _assets[_i].Split('/')[^1];
                if (_assetName == "" || _assetName == _currAssetName) return AssetDatabase.LoadAssetAtPath<UnityEngine.Object>(_assets[_i]);
            }
            $"No asset of type {_type.Name} found in {_assets.ToCollectionString()}".LogError();
            return default;
        }


        public static void OpenSpecificPrefab<T>() where T : Component
        {
            AssetDatabase.OpenAsset(GetPrefabOfType<T>());
        }
        public static T GetPrefabOfType<T>() where T : Component => GetAllPrefabsOfType<T>()[0];

        public static List<string> GetAllAssetsInPath(string _path)
        {
            List<string> _allAssets = new();
            SearchFolderForAssets(ref _allAssets, _path);
            return _allAssets;
        }

        private static void SearchFolderForAssets(ref List<string> _assets, string _path)
        {
            string[] _files = Directory.GetFiles(_path);
            foreach (string _file in _files)
            {
                _assets.Add(_file);
            }
            string[] _directories = Directory.GetDirectories(_path);
            foreach (string _directory in _directories)
            {
                SearchFolderForAssets(ref _assets, _directory);
            }
        }

        public static void DoMethodOnComponentsInPrefab<T, U>(Action<U> _action, bool _setDirtyAndSave) where T : Component where U : Component
        {
            List<T> _prefabs = GetAllPrefabsOfType<T>();
            foreach (T _prefab in _prefabs)
            {
                bool _foundComponent = false;
                foreach (U _componentInstance in _prefab.GetComponentsInChildren<U>())
                {
                    _foundComponent = true;
                    _action?.Invoke(_componentInstance);
                    if (_setDirtyAndSave) EditorUtility.SetDirty(_componentInstance);
                }
                if (_foundComponent && _setDirtyAndSave) AddObjectToDirtyAndSaveIt(_prefab);
            }
        }
        #endregion

        public static GameObject GetCurrentOpenedPrefab()
        {
            PrefabStage _stage = PrefabStageUtility.GetCurrentPrefabStage();
            return _stage == null ? null : AssetDatabase.LoadAssetAtPath<GameObject>(_stage.assetPath);
        }

        public static bool authorizeSaveAsset = true;
        public static void AddObjectToDirtyAndSaveIt(UnityEngine.Object _obj)
        {
            EditorUtility.SetDirty(_obj);
            if (authorizeSaveAsset) AssetDatabase.SaveAssetIfDirty(_obj);
            else Debug.Log("Warning, save asset isn't authorized");
        }

        #region GUI Layout Method
        public static void MethodButton<T>(this T _editor, Action _method, params GUILayoutOption[] _options) where T : UnityEditor.Editor
        {
            string _methodName = _method.Method.Name;
            if (GUILayout.Button(_methodName, _options))
            {
                Undo.RecordObject(_editor.target, _methodName);
                _method.Invoke();
            }
        }

        public static void PropertyMethodLayout<T>(this T _editor, string _propertyName, Action _method, string _overrideLabel = null) where T : UnityEditor.Editor
        {
            GUILayout.BeginHorizontal();

            GUIContent _label = new(_overrideLabel ?? _propertyName);
            EditorGUILayout.PropertyField(_editor.serializedObject.FindProperty(_propertyName), _label);
            if (_overrideLabel == "") _editor.MethodButton(_method, GUILayout.Width(Mathf.RoundToInt(Screen.width / 2f)));
            else _editor.MethodButton(_method);

            GUILayout.EndHorizontal();

            _editor.serializedObject.ApplyModifiedProperties();
        }
        #endregion

        #region Prefab Physics Scene
        public static PhysicsScene2D GetPrefabPhysicsScene2D() => PrefabStageUtility.GetCurrentPrefabStage().scene.GetPhysicsScene2D();
        public static PhysicsScene GetPrefabPhysicsScene() => PrefabStageUtility.GetCurrentPrefabStage().scene.GetPhysicsScene();
        #endregion

        public static string GetHyperLink(this UnityEngine.Object _asset) => "<a href=\"" + AssetDatabase.GetAssetPath(_asset) + "\">" + _asset.name + "</a>";

        public static Type FindTypeOfObject(string _typeName) => Type.GetType(_typeName, false);

        public static Type GetGenericArgumentType(this SerializedProperty _property, int _index) => _property.GetTypeOfProperty().GetGenericArguments()[_index];

        public static string GetCurrentProjectWindowPath()
        {
            Type _projectWindowUtilType = typeof(ProjectWindowUtil);
            MethodInfo _getActiveFolderPath = _projectWindowUtilType.GetMethod("GetActiveFolderPath", BindingFlags.Static | BindingFlags.NonPublic);
            object _obj = _getActiveFolderPath.Invoke(null, new object[0]);
            return _obj.ToString();
        }

        #region Create Object


        #region Create Scriptable Object Shortcuts

        public static T GetOrCreateScriptableObject<T>(string _path = "", string _assetName = "") where T : ScriptableObject => (T)GetOrCreateScriptableObject(typeof(T), _path, _assetName);
        public static ScriptableObject GetOrCreateScriptableObject(Type _type, string _folderPath = "", string _assetName = "") => (ScriptableObject)GetAssetOfType(_type, _assetName) ?? CreateScritableObjectInProject(_type, _folderPath, _displayEditorWindow: true, _displaySaveFilePannel: true);
        public static T CreateScritableObjectInProject<T>(string _path = "") where T : ScriptableObject => CreateScritableObjectInProject(typeof(T), _path) as T;

        #endregion

        public static ScriptableObject CreateScritableObjectInProject(Type _type, string _folderPath = "", bool _displayEditorWindow = false, bool _displaySaveFilePannel = false) =>
            CreateObjectInProject(_objectDescription: $"scriptable object of type {_type.Name}", () => ScriptableObject.CreateInstance(_type), _assetExtension: "asset", _folderPath, _displayEditorWindow, _displaySaveFilePannel);

        public static Texture2D CreateTexture2DInProject(int _width, int _height, string _folderPath = "", bool _displayEditorWindow = false, bool _displaySaveFilePannel = false) =>
            CreateObjectInProject(_objectDescription: $"texture 2D", () => new Texture2D(_width, _height), _assetExtension: "asset", _folderPath, _displayEditorWindow, _displaySaveFilePannel);

        private static T CreateObjectInProject<T>(string _objectDescription, Func<T> _methodToCreateObject, string _assetExtension, string _folderPath, bool _displayEditorWindow, bool _displaySaveFilePannel = false) where T : Object
        {
            string _defaultName = typeof(T).Name;
            string _extensionWithDot = _assetExtension.StartsWith(".") ? _assetExtension : "." + _assetExtension;
            string _extensionWithoutDot = _assetExtension.StartsWith(".") ? _assetExtension[1..] : _assetExtension;
            string PathWithExtension(string _name) => _name + _extensionWithDot;
            string PathWithoutExtension(string _name) => _name.Replace(_extensionWithDot, "");
            bool AssetExistAt(string _path) => !string.IsNullOrEmpty(AssetDatabase.AssetPathToGUID(_path.Contains(_extensionWithDot) ? _path : PathWithExtension(_path)));

            try
            {
                bool _done = false;
                if (_displayEditorWindow) _done = EditorUtility.DisplayDialog($"Create {_defaultName}", $"Creating {_objectDescription}", "Ok");

                _done.Log();

                string _name = _defaultName;
                string _finalPath = _folderPath + _name;
                bool _canDeleteExistingAsset = false;

                // --- Save Window

                if (_displaySaveFilePannel)
                {
                    string _baseSavePath;
                    if (_folderPath.Length == 0)
                    {
                        _baseSavePath = GetCurrentProjectWindowPath();
                        string _pathToCurrentFolder = GetCurrentProjectWindowPath();
                    }
                    else
                    {
                        _baseSavePath = _folderPath;
                    }
                    _finalPath = EditorUtility.SaveFilePanel($"Save {_objectDescription} to folder", _baseSavePath, _name, _extensionWithoutDot);
                    if (_finalPath.StartsWith(Application.dataPath)) _finalPath = "Assets" + _finalPath[Application.dataPath.Length..];
                    _folderPath = _finalPath[.._finalPath.LastIndexOf('/')];
                    _canDeleteExistingAsset = true;
                }

                if (AssetExistAt(_finalPath))
                {
                    if (_canDeleteExistingAsset)
                    {
                        AssetDatabase.DeleteAsset(_finalPath);
                        AssetDatabase.Refresh();
                    }
                    else
                    {
                        int _index = 0;
                        int _security = 50;
                        while (_security > 0 && AssetExistAt(PathWithExtension(PathWithoutExtension(_finalPath) + '_' + _index)))
                        {
                            _index++;
                            _security--;
                        }
                        _finalPath = PathWithExtension(PathWithoutExtension(_finalPath) + '_' + _index);
                    }
                }

                T _objectCreated = _methodToCreateObject();
                AssetDatabase.CreateAsset(_objectCreated, _finalPath);
                EditorUtility.DisplayDialog($"{_defaultName} created", $"Created {_finalPath}", "Giga nice");
                EditorGUIUtility.PingObject(_objectCreated);
                AssetDatabase.Refresh();
                return _objectCreated;
            }
            catch (Exception _e)
            {
                _e.LogError();
            }
            return default;
        }

        #endregion





        [MenuItem("Assets/Prefab/Create Variant")]
        private static void CreateVariant()
        {
            UnityEngine.Object _selected = Selection.activeObject;
            if (_selected is GameObject _gameObject)
            {
                string _path = AssetDatabase.GetAssetPath(_selected);
                _path = _path.Replace(".prefab", "_Variant.prefab");
                GameObject _objSource = (GameObject)PrefabUtility.InstantiatePrefab(_gameObject);
                PrefabUtility.SaveAsPrefabAsset(_objSource, _path);
                MonoBehaviour.DestroyImmediate(_objSource);
            }
            else $"Can't create prefab : object is not a GameObject".LogError();
        }

        public static bool CreateConfirmWindow(string _title, string _message) => EditorUtility.DisplayDialog(_title, _message, "Confirm", "Cancel");
        public static void CreateErrorWindow(string _title, string _message) => EditorUtility.DisplayDialog(_title, _message, "Bummer");

        public static void ReimportAllAssetOfType<T>(params string[] _nameFilters) where T : UnityEngine.Object
        {
            string[] _reimportedAssetPaths = GetAllAssetsOfType<T>(_nameFilters).ExtractArray(_textAsset => AssetDatabase.GetAssetPath(_textAsset), _textAsset => true);
            foreach (string _reimportedAssetPath in _reimportedAssetPaths)
                AssetDatabase.ImportAsset(_reimportedAssetPath, ImportAssetOptions.ImportRecursive);
        }

        #region Sub Asset
        public static T AddSubScriptableObject<T>(this UnityEngine.Object _obj, string _name = "Element", HideFlags _hideFlags = HideFlags.None, bool _saveAssets = false) where T : ScriptableObject
        {
            T _element = ScriptableObject.CreateInstance<T>();

            _element.name = _name;
            _element.hideFlags = _hideFlags;

            string _scriptableObjectPath = AssetDatabase.GetAssetPath(_obj);

            AssetDatabase.AddObjectToAsset(_element, _scriptableObjectPath);
            if (_saveAssets) AssetDatabase.SaveAssets();

            Undo.RegisterCreatedObjectUndo(_element, "Add element to ScriptableObject");
            return _element;

        }

        public static void DeleteAllSubObjects(this UnityEngine.Object _obj)
        {
            string _assetPath = AssetDatabase.GetAssetPath(_obj);
            foreach (UnityEngine.Object _subObject in AssetDatabase.LoadAllAssetsAtPath(_assetPath).ToArray())
                if (_subObject != _obj)
                    UnityEngine.Object.DestroyImmediate(_subObject, true);

            EditorUtility.SetDirty(_obj);
            AssetDatabase.SaveAssets();
        }
        #endregion

        #region Path
        public static bool IsFromAssetPath(this Object _object) => AssetDatabase.GetAssetPath(_object).Split('/')[0] == "Assets";
        public static bool IsFromSpecificFolders(this Object _object, params string[] _folderNames) => _folderNames.Any(_folderName => AssetDatabase.GetAssetPath(_object).Contains(_folderName + '/'));
        #endregion

        #region Texture
        public static bool TryGetTrueTextureSize(this Texture2D _texture, out int _width, out int _height)
        {
            if (_texture.TryGetTextureImporter(out TextureImporter _textureImporter))
            {
                _textureImporter.GetTrueTextureSize(out _width, out _height);
                return true;
            }
            _width = _height = 0;
            return false;
        }
        public static void GetTrueTextureSize(this TextureImporter _textureImporter, out int _width, out int _height)
        {
            object[] _args = new object[2] { 0, 0 };
            MethodInfo _mi = typeof(TextureImporter).GetMethod("GetWidthAndHeight", BindingFlags.NonPublic | BindingFlags.Instance);
            _mi.Invoke(_textureImporter, _args);

            _width = (int)_args[0];
            _height = (int)_args[1];
        }

        public static bool TryGetTextureImporter(this Texture _texture, out TextureImporter _textureImporter)
        {
            string _path = AssetDatabase.GetAssetPath(_texture);
            try
            {
                AssetImporter _assetImporter = AssetImporter.GetAtPath(_path);
                _path.LogDesc((_assetImporter != null && _assetImporter is TextureImporter).ToString());
                _textureImporter = _assetImporter != null && _assetImporter is TextureImporter _textureImporterCast ? _textureImporterCast : null;
            }
            catch (Exception _e)
            {
                _path.Log();
                _e.LogError();
                _textureImporter = null;
            }
            return _textureImporter != null;
        }
        public static bool IsTextureUsedInMaterials(this Texture2D _texture)
        {
            List<Material> _materials = UfEditor.GetAllAssetsOfType<Material>();
            foreach (Material _material in _materials)
            {
                if (!_material.IsFromAssetPath()) continue;

                string[] _textureProperties = _material.GetTexturePropertyNames();
                foreach (string _textureProperty in _textureProperties)
                    if (_material.GetTexture(_textureProperty) == _texture)
                        return true;
            }
            return false;
        }

        public static bool HasValidDimensions(this Texture _texture) => Mathf.IsPowerOfTwo(_texture.height) && Mathf.IsPowerOfTwo(_texture.width);

        public static void EnableCompression(this Texture2D _texture)
        {
            if (!_texture.TryGetTextureImporter(out TextureImporter _textureImporter)) return;
            if (_textureImporter.textureCompression == TextureImporterCompression.CompressedHQ && _textureImporter.crunchedCompression) return;

            _textureImporter.textureCompression = TextureImporterCompression.CompressedHQ;
            _textureImporter.crunchedCompression = true;
            _textureImporter.compressionQuality = 0;
            if (AssetDatabase.WriteImportSettingsIfDirty(_textureImporter.assetPath))
                _textureImporter.SaveAndReimport();
        }

        public static void RemoveCompression(this Texture2D _texture)
        {
            if (!_texture.TryGetTextureImporter(out TextureImporter _textureImporter)) return;
            if (_textureImporter.textureCompression == TextureImporterCompression.Uncompressed && !_textureImporter.crunchedCompression) return;

            _textureImporter.textureCompression = TextureImporterCompression.Uncompressed;
            _texture.Log();
            if (AssetDatabase.WriteImportSettingsIfDirty(_textureImporter.assetPath))
                _textureImporter.SaveAndReimport();
        }

        #endregion

        public static void ReimportRandomAssembly()
        {
            // find any asset of format .asmdef and force update it because unity won't make it work otherwise, the things i do for you unity
            AssemblyDefinitionAsset _randomAssembly = UfEditor.GetAssetOfType<AssemblyDefinitionAsset>();
            string _assemblyPath = AssetDatabase.GetAssetPath(_randomAssembly);
            AssetDatabase.ImportAsset(_assemblyPath, ImportAssetOptions.ForceUpdate);
        }

#endif
    }
}
