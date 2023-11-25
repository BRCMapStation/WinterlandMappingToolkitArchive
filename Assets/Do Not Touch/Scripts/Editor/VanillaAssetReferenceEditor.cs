using UnityEngine;
using UnityEditor;
using Winterland.MapStation.Common.VanillaAssets;
using System.Reflection;
using Unity.VisualScripting;
using Microsoft.SqlServer.Server;
using System;
using System.Drawing;
using Unity.VisualScripting.FullSerializer;
using System.Collections.Generic;

[CustomEditor(typeof(VanillaAssetReference))]
public class VanillaAssetReferenceEditor : Editor {
    /// <summary>
    /// Prefixes to be stripped from asset paths.
    /// Allows you to nest Reptile's vanilla assets within a subdirectory of
    /// your project.
    /// </summary>
    public static List<string> StripPathPrefixes = new List<string> {
        "MappingToolkit/Assets/Do Not Touch/ReptileAssets/"
    };

    VanillaAssetReference t { get => target as VanillaAssetReference; }
    public override void OnInspectorGUI() {
        EditorGUILayout.HelpBox(
            "Repairs references to base game assets by re-assigning them at runtime.\n" +
            "References from our asset bundles to vanilla game asset bundles have the wrong IDs and are null at runtime unless we fix them.\n" +
            "This component stores the assetbundle name and asset path so it can retrieve the assets at runtime.",
            MessageType.Info);
        base.OnInspectorGUI();
        if(GUILayout.Button("Add field")) {
            var menu = new GenericMenu();
            var iterator = new SerializedObject(t.component).GetIterator();
            while (iterator.NextVisible(true)) {
                menu.AddItem(new GUIContent(iterator.propertyPath), false, onFieldSelected, iterator.propertyPath);
            }
            menu.ShowAsContext();
        }
    }
    private void onFieldSelected(object selected) {
        var field = selected as string;
        
        var f = t.component.GetType().GetField(field, 
            BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic
        );
        var value = f.GetValue(t.component);
        var path = AssetDatabase.GetAssetPath(value as UnityEngine.Object);
        if(path == null || path == "") {
            Debug.LogError(string.Format("Field {0} does not refer to an asset.", field));
            return;
        }
        var bundle = AssetImporter.GetAtPath(path).assetBundleName;
        if (bundle == null || bundle == "") {
            Debug.LogError("Referenced asset is not assigned to an assetbundle.");
            return;
        }
        const string assetsDir = "Assets/";
        foreach(var prefix in StripPathPrefixes) {
            if(path.IndexOf(assetsDir + prefix) == 0) {
                path = assetsDir + path.Substring(assetsDir.Length + prefix.Length);
                break;
            }
        }
        t.fields.Add(String.Format("{0}={1}:{2}", field, bundle, path));
    }
}