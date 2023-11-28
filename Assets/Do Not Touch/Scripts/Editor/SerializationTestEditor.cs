using System;
using UnityEditor;
using UnityEngine;

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