using System;
using System.Reflection;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

namespace Winterland.MapStation.Common.VanillaAssets {
    public class VanillaAssetReferenceEditorDebugTools {
        const BindingFlags AllBindingFlags = BindingFlags.Default
            | BindingFlags.Instance
            | BindingFlags.Public
            | BindingFlags.NonPublic
            | BindingFlags.FlattenHierarchy
            ;

        [MenuItem("BRC Mapping Toolkit/Experiments/AnalyzeMeshRendererReflection")]
        private static void AnalyzeMeshRendererReflection() {

            var type = typeof(MeshRenderer);
            var type2 = typeof(Renderer);
            var type3 = typeof(Component);
            var type4 = typeof(UnityEngine.Object);
            var type5 = typeof(MonoBehaviour);
            var type6 = typeof(Behaviour);
            var meshRenderer = GameObject.FindAnyObjectByType<MeshRenderer>();
            var serialized = new SerializedObject(meshRenderer);
            serialized.FindProperty("m_StaticShadowCaster");

            // var filter = "ayTracing";
            var filter = "ateri";
            Debug.Log("SerializedObject fields:");
            var iterator = serialized.GetIterator();
            do {
                if(!iterator.name.Contains(filter)) continue;
                Debug.Log(String.Format("\"{0}\" {1} {2} hasChildren={3} {4} {5} {6}",// FieldTypename={4} FullTypename={5}",
                iterator.displayName, iterator.name, iterator.depth, iterator.hasChildren, iterator.propertyPath, iterator.propertyType, iterator.FixedPropertyPath()
                // iterator.managedReferenceFieldTypename, iterator.managedReferenceFullTypename
                ));
            } while(iterator.NextVisible(true));

            Debug.Log("Fields");
            foreach(var field in type.GetFields(VanillaAssetReference.UseTheseBindingFlags)) {
                Debug.Log(field.Name);
            }

            Debug.Log("Members");
            var members = type.GetMembers(AllBindingFlags);
            foreach(var member in members) {
                if(!member.Name.Contains(filter)) continue;
                Debug.Log(member.Name + " " + member.ReflectedType.Name);
            }
            Debug.Log("Properties");
            var properties = type.GetProperties(AllBindingFlags);
            foreach(var property in properties) {
                if(!property.Name.Contains(filter)) continue;
                Debug.Log(property.Name + " " + property.ReflectedType.Name);
            }
        }
    }
}
