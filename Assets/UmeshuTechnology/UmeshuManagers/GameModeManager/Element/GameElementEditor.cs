//#if UNITY_EDITOR

//using Umeshu.USystem;
//using UnityEditor;

//// Declare a custom editor of game element
//[CustomEditor(typeof(GameElement), true)]
//public class GameElementEditor : Editor
//{
//    public override void OnInspectorGUI()
//    {
//        GameElement _gameElement = target as GameElement;
//        if (GameElement.HeritsDirectlyFromGameElement(_gameElement, out string _errorMessage)) EditorGUILayout.HelpBox(_errorMessage, MessageType.Error);
//        else base.OnInspectorGUI();
//    }
//}

//#endif