using UnityEngine.UIElements;
using UnityEditor;

namespace EditableFighterActions {
    public class InspectorView : VisualElement {
        public new class UxmlFactory : UxmlFactory<InspectorView, VisualElement.UxmlTraits> { }

        Editor editor;

        public InspectorView() {
            
        }

        internal void UpdateSelection(BPNode node) {
            Clear();

            UnityEngine.Object.DestroyImmediate(editor);
            
            editor = Editor.CreateEditor(node);
            IMGUIContainer container = new IMGUIContainer(() => {
                if (editor && editor.target) {
                    editor.OnInspectorGUI();
                }
            });
            Add(container);
        }
    }
}
