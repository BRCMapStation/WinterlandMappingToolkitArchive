using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using Winterland.Common;
public class AddPrefabsToContextMenu {
    private const int Priority = -30;
    private const string PrefabPathPrefix = "Assets/MappingToolkit/Assets/Map Template/";

    [MenuItem("GameObject/" + Constants.menuLabel + "/Create Door Teleport", priority = Priority)]
    private static void CreateTeleport(MenuCommand menuCommand) {
        CreatePrefabUnderContext(menuCommand.context, "DoorTeleporterPrefab");
    }
    [MenuItem("GameObject/" + Constants.menuLabel + "/Create Out-of-Bounds Teleport", priority = Priority)]
    private static void CreateOobTeleport(MenuCommand menuCommand) {
        CreatePrefabUnderContext(menuCommand.context, "TeleporterPrefab");
    }

    [MenuItem("GameObject/" + Constants.menuLabel + "/Create Small Graffiti Spot", priority = Priority)]
    private static void CreateGraffitiSpotSmall(MenuCommand menuCommand) {
        CreatePrefabUnderContext(menuCommand.context, "GraffitiSpotSmallPrefab", false);
    }
    [MenuItem("GameObject/" + Constants.menuLabel + "/Create Medium Graffiti Spot", priority = Priority)]
    private static void CreateGraffitiSpotMedium(MenuCommand menuCommand) {
        CreatePrefabUnderContext(menuCommand.context, "GraffitiSpotMediumPrefab", false);
    }
    [MenuItem("GameObject/" + Constants.menuLabel + "/Create Large Graffiti Spot", priority = Priority)]
    private static void CreateGraffitiSpotLarge(MenuCommand menuCommand) {
        CreatePrefabUnderContext(menuCommand.context, "GraffitiSpotLargePrefab", false);
    }
    [MenuItem("GameObject/" + Constants.menuLabel + "/Create Extra Large Graffiti Spot", priority = Priority)]
    private static void CreateGraffitiSpotExtraLarge(MenuCommand menuCommand) {
        CreatePrefabUnderContext(menuCommand.context, "GraffitiSpotExtraLargePrefab", false);
    }

    [MenuItem("GameObject/" + Constants.menuLabel + "/Create Grind", priority = Priority)]
    private static void CreateGrind(MenuCommand menuCommand) {
        CreatePrefabUnderContext(menuCommand.context, "GrindPrefab", false);
    }

    [MenuItem("GameObject/" + Constants.menuLabel + "/Create Vert Ramp", priority = Priority)]
    private static void CreateVertRamp(MenuCommand menuCommand) {
        CreatePrefabUnderContext(menuCommand.context, "VertRampPrefab");
    }
    [MenuItem("GameObject/" + Constants.menuLabel + "/Create Wallrun", priority = Priority)]
    private static void CreateWallrun(MenuCommand menuCommand) {
        CreatePrefabUnderContext(menuCommand.context, "WallRunPrefab");
    }
    [MenuItem("GameObject/" + Constants.menuLabel + "/Create Vending Machine", priority = Priority)]
    private static void CreateVendingMachine(MenuCommand menuCommand) {
        CreatePrefabUnderContext(menuCommand.context, "VendingMachinePrefab");
    }

    private static void CreatePrefabUnderContext(Object context, string PrefabName, bool supportUndo = true) {
        var assetPath = PrefabPathPrefix + PrefabName + ".prefab";
        var prefabAsset = AssetDatabase.LoadAssetAtPath<GameObject>(assetPath);
        if(prefabAsset == null) {
            Debug.LogError(string.Format("Prefab not found at path {0}", assetPath));
            return;
        }
        var prefabInstance = PrefabUtility.InstantiatePrefab(prefabAsset) as GameObject;
        StageUtility.PlaceGameObjectInCurrentStage(prefabInstance);
        GameObjectUtility.SetParentAndAlign(prefabInstance, context as GameObject);
        prefabInstance.transform.position = SceneView.lastActiveSceneView.pivot;
        // Undo for some prefabs is *crashing* Unity Editor.
        // Broken/misbehaving Undo is better than losing your work.
        if(supportUndo) {
            Undo.RegisterCreatedObjectUndo(prefabInstance, $"Create {prefabInstance.name}");
        }
        Selection.activeObject = prefabInstance;
    }
}
