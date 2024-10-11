using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Umeshu.Uf
{
    public static class UfMenuItem
    {
#if UNITY_EDITOR

        #region Implementations

        private const string MENU_ITEM_PATH = "GameObject/Custom Methods/";
        public const string ADDED_MENU_ITEM_PATH = MENU_ITEM_PATH + "Added/";

        #region Sorting

        #region Position
        private const string SORTING_MENU_ITEM_PATH = MENU_ITEM_PATH + "Sort/Position/";

        [MenuItem(SORTING_MENU_ITEM_PATH + nameof(SortChildOrderPerXPosition), false, 0)]
        private static void SortChildOrderPerXPosition() => SortChildOrder((_a, _b) => -_a.transform.position.x.CompareTo(_b.transform.position.x));

        [MenuItem(SORTING_MENU_ITEM_PATH + nameof(SortChildOrderPerYPosition), false, 0)]
        private static void SortChildOrderPerYPosition() => SortChildOrder((_a, _b) => -_a.transform.position.y.CompareTo(_b.transform.position.y));

        [MenuItem(SORTING_MENU_ITEM_PATH + nameof(SortChildOrderPerZPosition), false, 0)]
        private static void SortChildOrderPerZPosition() => SortChildOrder((_a, _b) => -_a.transform.position.z.CompareTo(_b.transform.position.z));
        #endregion

        private static void SortChildOrder(Comparison<GameObject> _comparison) => GameObject_ItemAction(_gameObjects =>
        {
            if (_gameObjects.Count < 2) return;
            Transform _parentTransform = _gameObjects[0].transform.parent;
            if (!_gameObjects.ChecksConditionWithWindowLog(_gameObject => _gameObject.transform.parent == _parentTransform, "All GameObjects must have the same parent.")) return;
            _gameObjects.Sort(_comparison);

            int _siblingIndex = _gameObjects[0].transform.GetSiblingIndex();
            foreach (GameObject _gameObject in _gameObjects) _siblingIndex = Mathf.Min(_siblingIndex, _gameObject.transform.GetSiblingIndex());

            _gameObjects.ForEach(_gameObject => _gameObject.transform.SetSiblingIndex(_siblingIndex));
        });

        #endregion

        #region Random

        private const string RANDOM_MENU_ITEM_PATH = MENU_ITEM_PATH + "Random/";

        [MenuItem(RANDOM_MENU_ITEM_PATH + nameof(AddRandom2DPosition), false, 0)]
        private static void AddRandom2DPosition() => GameObject_InputMenuItemAction_Float("Radius", 1, (List<GameObject> _gameObjects, float _randomRadius) =>
        {
            foreach (GameObject _gameObject in _gameObjects)
            {
                Vector2 _randomPosition = UnityEngine.Random.insideUnitCircle * _randomRadius;
                _gameObject.transform.AddToPos(_x: _randomPosition.x, _y: _randomPosition.y);
            }
        });

        [MenuItem(RANDOM_MENU_ITEM_PATH + nameof(AddRandomZRotation), false, 0)]
        private static void AddRandomZRotation() => GameObject_InputMenuItemAction_Float("Rotation Magnitude", 360, (List<GameObject> _gameObjects, float _maxRotation) =>
        {
            foreach (GameObject _gameObject in _gameObjects)
                _gameObject.transform.Rotate(Vector3.forward * UnityEngine.Random.Range(-_maxRotation, _maxRotation));
        });



        [MenuItem(RANDOM_MENU_ITEM_PATH + nameof(AddRandomScale), false, 0)]
        private static void AddRandomScale() => GameObject_InputMenuItemAction_Float("Scale Random Magnitude (Between 0 & 1)", 0.2f, (List<GameObject> _gameObjects, float _scaleModification) =>
        {
            foreach (GameObject _gameObject in _gameObjects)
            {
                float _randomScale = UnityEngine.Random.Range(-_scaleModification, _scaleModification);
                _gameObject.transform.localScale *= 1 + _randomScale;
            }
        });

        #endregion

        #region Spacing

        private const string SPACING_MENU_ITEM_PATH = MENU_ITEM_PATH + "Spacing/";

        [MenuItem(SPACING_MENU_ITEM_PATH + nameof(SetXSpacing), false, 0)]
        private static void SetXSpacing() => GameObject_InputMenuItemAction_Int("X Spacing", 1, (List<GameObject> _gameObjects, int _spacing) =>
        {
            _gameObjects.Sort((_a, _b) => _a.transform.position.x.CompareTo(_b.transform.position.x));
            for (int _i = 1; _i < _gameObjects.Count; _i++) _gameObjects[_i].transform.SetPosWith(_x: _gameObjects[_i - 1].transform.position.x + _spacing);
        });

        [MenuItem(SPACING_MENU_ITEM_PATH + nameof(SetYSpacing), false, 0)]
        private static void SetYSpacing() => GameObject_InputMenuItemAction_Int("Y Spacing", 1, (List<GameObject> _gameObjects, int _spacing) =>
        {
            _gameObjects.Sort((_a, _b) => _a.transform.position.y.CompareTo(_b.transform.position.y));
            for (int _i = 1; _i < _gameObjects.Count; _i++) _gameObjects[_i].transform.SetPosWith(_y: _gameObjects[_i - 1].transform.position.y + _spacing);
        });

        [MenuItem(SPACING_MENU_ITEM_PATH + nameof(SetZSpacing), false, 0)]
        private static void SetZSpacing() => GameObject_InputMenuItemAction_Int("Z Spacing", 1, (List<GameObject> _gameObjects, int _spacing) =>
        {
            _gameObjects.Sort((_a, _b) => _a.transform.position.z.CompareTo(_b.transform.position.z));
            for (int _i = 1; _i < _gameObjects.Count; _i++) _gameObjects[_i].transform.SetPosWith(_z: _gameObjects[_i - 1].transform.position.z + _spacing);
        });

        #endregion

        [MenuItem(MENU_ITEM_PATH + nameof(RenameObjects) + " &%R", false, 0)]
        private static void RenameObjects() => GameObject_InputMenuItemAction_String("Prefix ", "Object_", (List<GameObject> _gameObjects, string _prefix) =>
        {
            int _intLength = (_gameObjects.Count - 1).IntLength();
            for (int _i = 0; _i < _gameObjects.Count; _i++)
            {
                int _zeroCount = _intLength - _i.IntLength();
                _gameObjects[_i].name = _prefix + new string('0', _zeroCount) + _i;
            }
        });

        #endregion

        #region Permissions
        private static int lastCommandFrame = -1;
        private static bool CanExecuteMultiCommand(out List<GameObject> _gameObjects)
        {
            bool _canExecute = Time.frameCount != lastCommandFrame;
            lastCommandFrame = Time.frameCount;
            _gameObjects = Selection.gameObjects.ToList();
            return _canExecute;
        }

        private static bool CanExecuteMultiCommand<T>(out List<T> _components) where T : Component
        {
            bool _canExecute = Time.frameCount != lastCommandFrame;
            lastCommandFrame = Time.frameCount;
            List<GameObject> _gameObjects = Selection.gameObjects.ToList();
            _components = new();
            foreach (GameObject _gameObject in _gameObjects)
            {
                if (_gameObject.TryGetComponent(out T _component)) _components.Add(_component);
                else
                {
                    "Not all object have the required component.".LogErrorWindow();
                    return false;
                }
            }
            return _canExecute;
        }
        #endregion

        #region Tool Utility

        private static void SetDirty<T>(this IEnumerable<T> _collection) where T : UnityEngine.Object
        {
            foreach (T _item in _collection)
                EditorUtility.SetDirty(_item);
        }

        public static void CreateInputWindow<TInput, TObj>(string _windowTitle, Func<TInput, TInput> _editorFieldFunc, Action<List<TObj>, TInput> _endFunc, List<TObj> _items, TInput _defaultValue) =>
            ScriptableObject.CreateInstance<MenuItemUtility_InputWindow>().SetVariables(new InputWindowFonctionality.GenericWindow<TInput, TObj>(_windowTitle, _editorFieldFunc, _endFunc, _items, _defaultValue)).ShowUtility();

        #endregion

        #region Shortcuts

        public static void GameObject_ItemAction(Action<List<GameObject>> _action)
        {
            if (!CanExecuteMultiCommand(out List<GameObject> _gameObjects)) return;
            _action(_gameObjects);
            _gameObjects.SetDirty();
        }

        public static void Component_ItemAction<TComp>(Action<List<TComp>> _action) where TComp : Component
        {
            if (!CanExecuteMultiCommand(out List<TComp> _components)) return;
            _action(_components);
            _components.SetDirty();
        }

        public static void GameObject_InputMenuItemAction<TInput>(string _inputName, Action<List<GameObject>, TInput> _action, Func<TInput, TInput> _editorFieldFunc, TInput _defaultValue)
        {
            if (!CanExecuteMultiCommand(out List<GameObject> _gameObjects)) return;
            CreateInputWindow(_inputName, _editorFieldFunc, _action, _gameObjects, _defaultValue);
            _gameObjects.SetDirty();
        }

        public static void Component_InputMenuItemAction<TComp, TInput>(string _inputName, Action<List<TComp>, TInput> _action, Func<TInput, TInput> _editorFieldFunc, TInput _defaultValue) where TComp : Component
        {
            if (!CanExecuteMultiCommand(out List<TComp> _components)) return;
            CreateInputWindow(_inputName, _editorFieldFunc, _action, _components, _defaultValue);
            _components.SetDirty();
        }

        internal static int IntField(int _defaultValue) => EditorGUILayout.IntField(_defaultValue);
        internal static float FloatField(float _defaultValue) => EditorGUILayout.FloatField(_defaultValue);
        internal static string StringField(string _defaultValue) => EditorGUILayout.TextField(_defaultValue);
        internal static bool BoolField(bool _defaultValue) => EditorGUILayout.Toggle(_defaultValue);

        public static void GameObject_InputMenuItemAction_Int(string _inputName, int _defaultValue, Action<List<GameObject>, int> _action) => GameObject_InputMenuItemAction(_inputName, _action, IntField, _defaultValue);
        public static void GameObject_InputMenuItemAction_Float(string _inputName, float _defaultValue, Action<List<GameObject>, float> _action) => GameObject_InputMenuItemAction(_inputName, _action, FloatField, _defaultValue);
        public static void GameObject_InputMenuItemAction_String(string _inputName, string _defaultValue, Action<List<GameObject>, string> _action) => GameObject_InputMenuItemAction(_inputName, _action, StringField, _defaultValue);
        public static void GameObject_InputMenuItemAction_Bool(string _inputName, bool _defaultValue, Action<List<GameObject>, bool> _action) => GameObject_InputMenuItemAction(_inputName, _action, BoolField, _defaultValue);

        public static void Component_InputMenuItemAction_Int<TComp>(string _inputName, int _defaultValue, Action<List<TComp>, int> _action) where TComp : Component => Component_InputMenuItemAction(_inputName, _action, IntField, _defaultValue);
        public static void Component_InputMenuItemAction_Float<TComp>(string _inputName, float _defaultValue, Action<List<TComp>, float> _action) where TComp : Component => Component_InputMenuItemAction(_inputName, _action, FloatField, _defaultValue);
        public static void Component_InputMenuItemAction_String<TComp>(string _inputName, string _defaultValue, Action<List<TComp>, string> _action) where TComp : Component => Component_InputMenuItemAction(_inputName, _action, StringField, _defaultValue);
        public static void Component_InputMenuItemAction_Bool<TComp>(string _inputName, bool _defaultValue, Action<List<TComp>, bool> _action) where TComp : Component => Component_InputMenuItemAction(_inputName, _action, BoolField, _defaultValue);

        public static void InputMenuItemAction_Int(string _inputName, int _defaultValue, Action<int> _action) => GameObject_InputMenuItemAction(_inputName, (_, _input) => _action(_input), IntField, _defaultValue);
        public static void InputMenuItemAction_Float(string _inputName, float _defaultValue, Action<float> _action) => GameObject_InputMenuItemAction(_inputName, (_, _input) => _action(_input), FloatField, _defaultValue);
        public static void InputMenuItemAction_String(string _inputName, string _defaultValue, Action<string> _action) => GameObject_InputMenuItemAction(_inputName, (_, _input) => _action(_input), StringField, _defaultValue);
        public static void InputMenuItemAction_Bool(string _inputName, bool _defaultValue, Action<bool> _action) => GameObject_InputMenuItemAction(_inputName, (_, _input) => _action(_input), BoolField, _defaultValue);

        #endregion

        #region InputWindow

        public abstract class InputWindowFonctionality
        {
            public abstract bool DoOnGUI();
            public abstract string GetWindowTitle();
            public class GenericWindow<TInput, TObj> : InputWindowFonctionality
            {
                public string windowTitle;
                public Func<TInput, TInput> editorFieldFunc;
                public Action<List<TObj>, TInput> endFunc;
                private List<TObj> items;
                private TInput inputVar;

                public GenericWindow(string _windowTitle, Func<TInput, TInput> _editorFieldFunc, Action<List<TObj>, TInput> _endFunc, List<TObj> _items, TInput _defaultValue)
                {
                    windowTitle = _windowTitle;
                    editorFieldFunc = _editorFieldFunc;
                    endFunc = _endFunc;
                    items = _items;
                    inputVar = _defaultValue;
                }

                public override bool DoOnGUI()
                {
                    inputVar = editorFieldFunc(inputVar);
                    GUILayout.BeginHorizontal();
                    if (GUILayout.Button("Validate"))
                    {
                        try
                        {
                            endFunc(items, inputVar);
                            return true;
                        }
                        catch (Exception _e)
                        {
                            _e.Message.LogErrorWindow();
                            return true;
                        }
                    }
                    return GUILayout.Button("Abort");
                }

                public override string GetWindowTitle() => windowTitle + " - " + typeof(TInput).Name;
            }
        }

        public class MenuItemUtility_InputWindow : EditorWindow
        {
            InputWindowFonctionality inputWindowFonctionality;
            public MenuItemUtility_InputWindow SetVariables(InputWindowFonctionality _inputWindowFonctionality)
            {
                maxSize = new Vector2(200, 50);
                minSize = maxSize;
                position = new Rect(Screen.width / 2, Screen.height / 2, maxSize.x, maxSize.y);
                inputWindowFonctionality = _inputWindowFonctionality;
                titleContent = new GUIContent(inputWindowFonctionality.GetWindowTitle());
                return this;
            }
            void OnGUI() { if (inputWindowFonctionality.DoOnGUI()) Close(); }
        }
        #endregion
#endif
    }
}
