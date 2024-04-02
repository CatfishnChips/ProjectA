using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace EditableFighterActions{
    [CustomEditor(typeof(FighterBlueprint))]
    public class FighterBlueprintInspector : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();


            FighterBlueprint fighterBlueprint = (FighterBlueprint)target;
            if(GUILayout.Button("Initialize Dictionaries")){
                fighterBlueprint.InitializeDictionaries();
            }
        }
    }
}
