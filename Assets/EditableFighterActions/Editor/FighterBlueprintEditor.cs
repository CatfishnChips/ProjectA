using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using TheKiwiCoder;

namespace EditableFighterActions{
    public class FighterBlueprintEditor : EditorWindow
    {
        FighterBlueprintView blueprintView;
        InspectorView inspectorView;

        [MenuItem("Window/UI Toolkit/FighterBlueprintEditor")]
        public static void OpenWindow()
        {
            FighterBlueprintEditor wnd = GetWindow<FighterBlueprintEditor>();
            wnd.titleContent = new GUIContent("FighterBlueprintEditor");
        }

        public void CreateGUI()
        {
            // Each editor window contains a root VisualElement object
            VisualElement root = rootVisualElement;

            // Import UXML
            var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>("Assets/EditableFighterActions/Editor/FighterBlueprintEditor.uxml");
            visualTree.CloneTree(root);

            // A stylesheet can be added to a VisualElement.
            // The style will be applied to the VisualElement and all of its children.
            var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/EditableFighterActions/Editor/FighterBlueprintEditor.uss");
            root.styleSheets.Add(styleSheet);

            blueprintView = root.Q<FighterBlueprintView>();
            inspectorView = root.Q<InspectorView>();
            blueprintView.OnNodeSelected = OnNodeSelectionChange;

            OnSelectionChange();
        }

        private void OnSelectionChange(){
            FighterBlueprint blueprint = Selection.activeObject as FighterBlueprint;
            if(blueprint){
                blueprintView.PopulateView(blueprint);
            }
        }

        private void OnNodeSelectionChange(BPNode node){
            inspectorView.UpdateSelection(node);
        }
    }
}