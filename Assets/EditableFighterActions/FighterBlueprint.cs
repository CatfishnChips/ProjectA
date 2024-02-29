using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Animations;

namespace EditableFighterActions{
    [CreateAssetMenu(fileName = "FighterBlueprint", menuName = "ScriptableObject/FighterBlueprint")]
    public class FighterBlueprint : ScriptableObject
    {
        public RootNodeAttribution[] rootNodes;
        public List<BPNode> nodes = new List<BPNode>();

        public BPNode CreateNode(System.Type type){
            BPNode node = ScriptableObject.CreateInstance(type) as BPNode;
            node.name = type.Name;
            node.Guid = GUID.Generate().ToString();
            nodes.Add(node);

            AssetDatabase.AddObjectToAsset(node, this);
            AssetDatabase.SaveAssets();

            return node;
        }

        public void DeleteNode(BPNode node){
            nodes.Remove(node);
            AssetDatabase.RemoveObjectFromAsset(node);
            AssetDatabase.SaveAssets();
        }

        public void AddChild(BPNode parent, BPNode child){
            parent.Children.Add(child);
        }

        public void RemoveChild(BPNode parent, BPNode child){
            parent.Children.Remove(child);
        }

        public List<BPNode> GetChildren(BPNode parent){
            return parent.Children;
        }

        public Dictionary<ActionTypes, RootNode> GetRootDict(){
            Dictionary<ActionTypes, RootNode> dict = new Dictionary<ActionTypes, RootNode>();
            for(int i = 0; i < rootNodes.Length; i++){
                dict.Add(rootNodes[i].type, rootNodes[i].rootNode);
                Debug.Log(rootNodes[i].type);
            }
            return dict;
        }
    }

    [Serializable]
    public struct RootNodeAttribution{
        public ActionTypes type;
        public RootNode rootNode;
    }

}