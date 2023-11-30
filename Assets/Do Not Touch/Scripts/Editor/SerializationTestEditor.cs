using System;
using UnityEditor;
using UnityEditor.Compilation;
using UnityEngine;
using Winterland.MapStation.Common.Serialization;

namespace Winterland.MapStation.Common {
    [CustomEditor(typeof(SerializationTest))]
    internal class SerializationTestEditor : SupportsBepInExSerializationWorkaroundEditor<SerializationTest, SerializationTest.Data, SerializationTestView> {}
    internal class SerializationTestView : DataView<SerializationTest.Data> {}
}

namespace Winterland.MapStation.Common {
    public abstract class DataView<D> : ScriptableObject {
        public D SerializedData;
    }

    public abstract class SupportsBepInExSerializationWorkaroundEditor<C, D, V> : Editor
        where C : MonoBehaviour, ISupportsBepInExSerializationWorkaround<D>
        where D : class
        where V : DataView<D>, new()
    {
        private C component { get => target as C; }
        private V view;
        private Editor editor;
        public override void OnInspectorGUI() {
            // Re-create editor when data object is replaced
            if(
                editor == null ||
                editor.target == null ||
                editor.target is not V ||
                ((V)editor.target).SerializedData != component.SerializedData
            ) {
                view = ScriptableObject.CreateInstance<V>();
                view.SerializedData = component.SerializedData;
                editor = Editor.CreateEditor(view);
            }
            editor.OnInspectorGUI();
            // Propagate view's dirty state to component
            if(EditorUtility.IsDirty(view)) {
                EditorUtility.ClearDirty(view);
                EditorUtility.SetDirty(component);
            }
        }
    }
}

namespace Winterland.MapStation.Common.Serialization {
    // [CustomPropertyDrawer(typeof(SList<object>), true)]
    // [CustomPropertyDrawer(typeof(InspectSListAttribute))]
    // public class SListDrawer : PropertyDrawer {
    //     // Draw the property inside the given rect
    //     public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    //     {
    //         EditorGUI.LabelField(position, "hello");
    //         EditorGUI.PropertyField(position, property.FindPropertyRelative("list"));
    //     }
    // }
    // [CustomPropertyDrawer(typeof(SerializationTest3.SList_Item), true)]
    // public class SList_Item_Drawer : SListDrawer {}

    class ExposeBuildStatus {
        [InitializeOnLoadMethod]
        static void OnLoad() {
            // BuildStatus.IsBuildingPlayerCallback = IsBuildingPlayer;
        }
        private static bool IsBuildingPlayer() {
            return BuildPipeline.isBuildingPlayer;
        }
    }
}