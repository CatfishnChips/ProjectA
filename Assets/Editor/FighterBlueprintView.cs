using System.Collections.Generic;
using UnityEditor;
using UnityEngine.UIElements;
using UnityEditor.Experimental.GraphView;
using System.Linq;
using System;
using UnityEngine;
using System.Reflection;

namespace EditableFighterActions{
    public class FighterBlueprintView : GraphView
    {
        public new class UxmlFactory : UxmlFactory<FighterBlueprintView, GraphView.UxmlTraits>{};
        FighterBlueprint blueprint;

        public Action<BPNode> OnNodeSelected;

        public FighterBlueprintView(){
            Insert(0, new GridBackground());

            this.AddManipulator(new ContentZoomer());
            this.AddManipulator(new ContentDragger());
            this.AddManipulator(new SelectionDragger());
            this.AddManipulator(new RectangleSelector());

            var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>("Assets/Editor/FighterBlueprintEditor.uss");
            styleSheets.Add(styleSheet);
        }

        NodeView FindNodeView(BPNode node){
            return GetNodeByGuid(node.Guid) as NodeView;
        }

        internal void PopulateView(FighterBlueprint blueprint)
        {
            this.blueprint = blueprint;

            graphViewChanged -= OnGraphViewChanged;
            DeleteElements(graphElements);
            graphViewChanged += OnGraphViewChanged;

            // To populate nodes
            blueprint.nodes.ForEach(n => CreateNodeView(n));

            // To populate edges
            blueprint.nodes.ForEach(n => {

                var children = blueprint.GetChildren(n);
                NodeView parentView = FindNodeView(n);

                children.ForEach(c => {

                    NodeView childView = FindNodeView(c);

                    Edge edge = parentView.output.ConnectTo(childView.input);
                    AddElement(edge);
                });
            });
        }

        public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
        {
            return ports.ToList().Where(endPort => 
            endPort.direction != startPort.direction &&
            endPort.node != startPort.node).ToList();
        }

        private GraphViewChange OnGraphViewChanged(GraphViewChange graphViewChange)
        {
            if(graphViewChange.elementsToRemove != null){
                graphViewChange.elementsToRemove.ForEach(elem => {
                    NodeView nodeView = elem as NodeView;
                    if(nodeView != null){
                        blueprint.DeleteNode(nodeView.node);
                    }

                    Edge edge = elem as Edge;
                    if(edge != null){
                        NodeView parentView = edge.output.node as NodeView;
                        NodeView childView = edge.input.node as NodeView;
                        blueprint.RemoveChild(parentView.node, childView.node);
                    }

                });
            }

            if(graphViewChange.edgesToCreate != null){
                graphViewChange.edgesToCreate.ForEach(edge => {

                    NodeView parentView = edge.output.node as NodeView;
                    NodeView childView = edge.input.node as NodeView;

                    if(parentView.node.GetType() == typeof(RootNode)){
                        if(childView.node.GetType() == typeof(NeutralActionNode)){
                            blueprint.AddChild(parentView.node, childView.node);
                        }
                        else{
                            parentView.output.Disconnect(edge);
                            childView.input.Disconnect(edge);
                            RemoveElement(edge);
                        }
                    }
                    else if(parentView.node.GetType() == typeof(ChainActionNode)){
                        if(childView.node.GetType() == typeof(ChainActionNode)){
                            blueprint.AddChild(parentView.node, childView.node);
                        }
                        else{
                            parentView.output.Disconnect(edge);
                            childView.input.Disconnect(edge);
                            RemoveElement(edge);
                        }
                    }
                    else{
                        blueprint.AddChild(parentView.node, childView.node);
                    }
                });
            }

            return graphViewChange;
        }

        public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
        {
            //base.BuildContextualMenu(evt);
            {
                var types = TypeCache.GetTypesDerivedFrom<BPNode>();
                foreach(var type in types){
                    if(!type.GetTypeInfo().IsAbstract){
                        evt.menu.AppendAction($"[{type.BaseType.Name}] {type.Name}", (a) => CreateNode(type));
                    }
                }
            }
        }

        void CreateNode(System.Type type){
            BPNode node = blueprint.CreateNode(type);
            CreateNodeView(node);
        }

        void CreateNodeView(BPNode node){
            NodeView nodeView = new NodeView(node);
            nodeView.OnNodeSelected = OnNodeSelected;
            AddElement(nodeView);
        }
    }
}