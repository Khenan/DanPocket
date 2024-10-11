#if UNITY_EDITOR

using System;
using System.Collections.Generic;
using Umeshu.Utility;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(GenericField))]
public class GenericFieldDrawer : PropertyDrawer
{
    public override float GetPropertyHeight(SerializedProperty _property, GUIContent _label)
    {
        return EditorGUI.GetPropertyHeight(_property, _label);
    }
    public override void OnGUI(Rect _position, SerializedProperty _property, GUIContent _label)
    {
        if (_property.isArray)
        {
            EditorGUILayout.PropertyField(_property, _label, true);
            return;
        }
        else
        {
            OnElementGUI(_position, _property, _label);
        }
    }

    private void OnElementGUI(Rect _position, SerializedProperty _property, GUIContent _label)
    {
        object _value = _property.GetUnderlyingValue();
        Rect _rect = _position.GetFieldRectOf();
        string _buttonText;
        if (_value is not null)
        {
            _rect.width /= 2;
            _rect.x += _rect.width;
            _buttonText = "Change Type";
            _label = new($"{_label} - {_value.GetType().Name}");
            _label.tooltip = $"The type of this object is {_value.GetType().Name}";
        }
        else
        {
            _buttonText = "Set Type";
            _label = new($"{_label} (Null)");
            _label.tooltip = "This object is null and has no type !";
        }
        bool _pressed = GUI.Button(_rect, _buttonText);
        if (_pressed)
        {
            Event.current.Use();
            GenericSwapper.Open(_property, (attribute as GenericField)?.showAsList ?? false);
            return;
        }
        EditorGUI.PropertyField(_position, _property, _label, true);
        GUI.enabled = true;
    }
}

public class GenericSwapper : EditorWindow
{
    public static bool Editing { get; private set; }
    [System.Serializable]
    public delegate void SwapAction(object _newObject);
    public static void Open(SerializedProperty _property, bool _showAsList) => GetWindow<GenericSwapper>().Create(_property, _showAsList);
    private void Create(SerializedProperty _property, bool _showAsList)
    {
        Type _type = _property.GetUnderlyingType();
        CreateOptionsAndTypes(ClassTree.Create(_type), _showAsList, out options, out types);
        copyEditor = new(_property);
        for (int _i = 0; _i < types.Length; _i++)
        {
            if (types[_i] == _type)
            {
                index = _i;
            }
        }
        copyEditor.onInvalidate += Close;
        RecreateObject();
        UpdateSize();
    }
    private void UpdateSize()
    {
        minSize = new(EditorGUIUtility.currentViewWidth, 3 * EditorGUIUtility.singleLineHeight + EditorGUI.GetPropertyHeight(copyEditor.Property));
        maxSize = minSize + Vector2.right * 500;
    }
    private int index;
    private Type[] types;
    private string[] options;
    SerializedPropertyUtil.PropertyCopy copyEditor;
    private Type Type => types[index];

    private void OnGUI()
    {
        if (copyEditor == null || !copyEditor.IsValid)
        {
            Close();
            return;
        }
        DisplayTypeChanger();
        DisplayObject();
        GUILayout.FlexibleSpace();
        DisplayButtons();
    }
    private void DisplayTypeChanger()
    {
        EditorGUI.BeginChangeCheck();
        index = EditorGUILayout.Popup(index, options);
        if (EditorGUI.EndChangeCheck())
        {
            RecreateObject();
            UpdateSize();
        }
    }
    private void DisplayObject()
    {
        EditorGUI.BeginChangeCheck();
        GUI.enabled = false;
        EditorGUILayout.PropertyField(copyEditor.Property, true);
        GUI.enabled = true;
        if (EditorGUI.EndChangeCheck())
        {
            UpdateSize();
        }
    }
    private void RecreateObject()
    {
        copyEditor.OriginalProperty.serializedObject.Update();
        object _newValue = Type.Instantiate();
        System.Reflection.FieldInfo[] _typeFields = Type.GetFields();
        foreach (SerializedProperty _property in copyEditor.OriginalProperty.Copy())
        {
            foreach (System.Reflection.FieldInfo _typeInfo in _typeFields)
            {
                if (_typeInfo.Name == _property.name)
                {
                    if (_typeInfo.FieldType == _property.GetValue()?.GetType())
                    {
                        _typeInfo.SetValue(_newValue, _property.GetValue());
                    }
                }
            }
        }
        copyEditor.Property.SetValue(_newValue);
    }
    private void DisplayButtons()
    {
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Close"))
        {
            Close();
        }
        if (GUILayout.Button("Confirm"))
        {
            copyEditor.Apply();
            Close();
        }
        GUILayout.EndHorizontal();
    }
    private void OnDestroy()
    {
        if (copyEditor != null)
        {
            copyEditor.onInvalidate -= Close;
            copyEditor.Close();
            copyEditor = null;
        }
    }
    ~GenericSwapper()
    {
        OnDestroy();
    }

    private static void CreateOptionsAndTypes(ClassTree _tree, bool _showAsList, out string[] _options, out Type[] _types)
    {
        List<string> _optionsList = new();
        List<Type> _typesList = new();
        AddOptionsFromTree(_tree.Nodes, "");
        _options = _optionsList.ToArray();
        _types = _typesList.ToArray();

        void AddOptionsFromTree(IReadonlyNode<Type> _node, string _path)
        {
            _path += _node.Value.Name;
            if (!_node.Value.IsAbstract && !_node.Value.IsInterface)
            {
                _optionsList.Add(_path);
                _typesList.Add(_node.Value);
            }
            if (_showAsList)
            {
                _path = "";
            }
            else
            {
                _path += '/';
            }
            foreach (INode<Type> _child in _node)
            {
                AddOptionsFromTree(_child, _path);
            }
        }
    }
}

#endif