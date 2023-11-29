using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UEditor = UnityEditor.Editor;
using Winterland.MapStation.Editor.ExposedUnityInternals;

namespace Winterland.MapStation.Common {
    [CustomEditor(typeof(SerializationTest))]
    internal class SerializationTestEditor : SupportsBepInExSerializationWorkaroundEditor<SerializationTest, SerializationTest.Data, SerializationTestView> {}
    internal class SerializationTestView : DataView<SerializationTest.Data> {}
}

namespace Winterland.MapStation.Common {
    public abstract class DataView<D> : ScriptableObject {
        public D SerializedData;
    }

    public abstract class SupportsBepInExSerializationWorkaroundEditor<C, D, V> : UEditor
        where C : MonoBehaviour, ISupportsBepInExSerializationWorkaround<D>
        where D : class
        where V : DataView<D>, new()
    {
        private C component { get => target as C; }
        private V view;
        private UEditor editor;
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
                editor = UEditor.CreateEditor(view);
            }
            editor.OnInspectorGUI();
            // Propagate view's dirty state to component
            if(EditorUtility.IsDirty(view)) {
                EditorUtility.ClearDirty(view);
                EditorUtility.SetDirty(component);
            }
        }
    }

    [CustomEditor(typeof(SerializationTest4))]
    class SerializationTest4Editor : UEditor {
        private SerializationTest4 t => target as SerializationTest4;

        private class Foo : ScriptableObject {
            public List<SerializationTest4.Item> list;
        }

        private static Type propertyHandlerType;
        private static object propertyHandler;
        private static MethodInfo propertyHandlerOnGUILayoutMethod;
        private static PropertyHandlerONGUILayoutDelegate propertyHandlerOnGUILayoutCallable;
        private delegate bool PropertyHandlerONGUILayoutDelegate(SerializedProperty property, GUIContent label, bool includeChildren, params GUILayoutOption[] options);

        private static void ensureListHandler() {
            if(propertyHandler != null) return;

            // Type scriptAttributeUtilityType = null;
            // foreach(var asm in AppDomain.CurrentDomain.GetAssemblies()) {
            //     foreach(var type in asm.GetTypes()) {
            //         if(type.Name == "ScriptAttributeUtility") {
            //             Debug.Log("found it"); 
            //             scriptAttributeUtilityType = type;
            //             goto Found;
            //         }
            //     }
            // }
            // Found:
            var asm = Array.Find(AppDomain.CurrentDomain.GetAssemblies(), 
                (a) => a.GetName().Name == "UnityEditor.CoreModule");
            var types = asm.GetTypes();
            // var scriptAttributeUtilityType = asm.GetType("ScriptAttributeUtility");
            var scriptAttributeUtilityType = Array.Find(types, (t) => t.Name == "ScriptAttributeUtility");
            var GetHandlerMethod = scriptAttributeUtilityType.GetMethod(
                            "GetHandler",
                            BindingFlags.Static | BindingFlags.NonPublic, null, 
                            new[] { typeof(SerializedProperty) }, null);
            var foo = ScriptableObject.CreateInstance<Foo>();
            var so = new SerializedObject(foo); 
            var property = so.FindProperty("list");
            var ph = propertyHandler = GetHandlerMethod.Invoke(null, new [] {property});
            var pht = propertyHandler.GetType();
            var phm = propertyHandlerOnGUILayoutMethod = pht.GetMethod("OnGUILayout");
            var phd = propertyHandlerOnGUILayoutCallable = (PropertyHandlerONGUILayoutDelegate)Delegate.CreateDelegate(typeof(PropertyHandlerONGUILayoutDelegate), ph, phm);
            Debug.Log(propertyHandlerOnGUILayoutMethod); 
        }
        void Awake() {
            ensureListHandler();
            // Use reflection to call UnityEditor.ScriptAttributeUtility.GetHandler(property)
            // for a List<SerializationTest4> property
            // Cache it so we can call its OnGUILayout(property, label, includeChildren, options);
 
        }
        public override void OnInspectorGUI() {
            base.OnInspectorGUI();
            var realProp = serializedObject.FindProperty("items");
            var so = ScriptableObject.CreateInstance<Foo>();
            so.list = t.items;
            var so2 = new SerializedObject(so);
            var prop = so2.FindProperty("list");
            // EditorGUILayout.GetControlRect(EditorGUI.LabelHasContent(label), GetHeight(property, label, includeChildren), options)
            var rect = EditorGUILayout.GetControlRect(true, 100);
            var reorderableList = new ReorderableListWrapper(prop, new GUIContent(realProp.displayName), true);
			// Vector2 vector = GUIUtility.GUIToScreenPoint(position.position);
			// vector.y = Mathf.Clamp(vector.y, GUIView.current?.screenPosition.yMin ?? 0f, GUIView.current?.screenPosition.yMax ?? ((float)Screen.height));
			// Rect screenRect = new Rect(vector.x, vector.y, GUIView.current?.screenPosition.width ?? ((float)Screen.width), GUIView.current?.screenPosition.height ?? ((float)Screen.height));
			// screenRect = GUIUtility.ScreenToGUIRect(screenRect);
			// value.Property = property;
            reorderableList.Draw(new GUIContent(realProp.displayName), rect, rect, "", true);
            if(so2.hasModifiedProperties) {
                Debug.Log("Changes");
                so2.ApplyModifiedProperties();
                EditorUtility.SetDirty(t);
                Debug.Log(t.items);
            }
            // EditorGUILayout.PropertyField(prop);
            

            // string propertyIdentifier = ReorderableListWrapper.GetPropertyIdentifier(property);
			// if (!s_reorderableLists.TryGetValue(propertyIdentifier, out var value))
			// {
			// 	value = new ReorderableListWrapper(property, label);
			// 	s_reorderableLists[propertyIdentifier] = value;
			// }
			// Vector2 vector = GUIUtility.GUIToScreenPoint(position.position);
			// vector.y = Mathf.Clamp(vector.y, GUIView.current?.screenPosition.yMin ?? 0f, GUIView.current?.screenPosition.yMax ?? ((float)Screen.height));
			// Rect screenRect = new Rect(vector.x, vector.y, GUIView.current?.screenPosition.width ?? ((float)Screen.width), GUIView.current?.screenPosition.height ?? ((float)Screen.height));
			// screenRect = GUIUtility.ScreenToGUIRect(screenRect);
			// value.Property = property;
			// value.Draw(label, position, screenRect, tooltip, includeChildren);
			// return !includeChildren && property.isExpanded;

            // propertyHandlerOnGUILayoutCallable(prop, new GUIContent("label here"), true);
            // EditorGUILayout.PropertyField
            // ScriptAttributeUtility
            GUILayout.Label("hello");
        }
    }
}