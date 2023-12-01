
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using Scene = UnityEngine.SceneManagement.Scene;

public class BuildWindow : EditorWindow {
    const string windowLabel = "Map Builder";

    [MenuItem(Constants.menuLabel + "/" + windowLabel, priority = Constants.defaultMenuPriority)]
    private static void ShowMyEditor() {
        EditorWindow wnd = GetWindow<BuildWindow>();
        wnd.titleContent = new GUIContent(windowLabel);
    }

    private Vector2 scrollbarPosition = Vector2.zero;

    private BRCMap _brcMap;
    private BRCMap brcMap => _brcMap = _brcMap != null ? _brcMap : FindObjectOfType<BRCMap>();

    private Editor brcMapEditor = null;

    private void OnDestroy() {
        DestroyImmediate(brcMapEditor);
    }

    private void OnEnable() {
        EditorSceneManager.activeSceneChangedInEditMode -= onSceneChange;
        EditorSceneManager.activeSceneChangedInEditMode += onSceneChange;
    }
    private void OnDisable() {
        EditorSceneManager.activeSceneChangedInEditMode -= onSceneChange;
    }
    private void onSceneChange(Scene s, Scene s2) {
        _brcMap = null;
    }

    private void OnInspectorUpdate() {
        Repaint();
    }

    private void OnGUI() {

        scrollbarPosition = EditorGUILayout.BeginScrollView(scrollbarPosition, false, false);

        if(brcMap) {
            Editor.CreateCachedEditor(brcMap, null, ref brcMapEditor);
            brcMapEditor.OnInspectorGUI();
        }

        EditorGUILayout.EndScrollView();
    }
}