using System;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace EditableFighterActions{

    public class NodeView : UnityEditor.Experimental.GraphView.Node
    {
        public Action<BPNode> OnNodeSelected;
        public BPNode node;
        public Port input;
        public Port output;

        private string initialTitle;

        public NodeView(BPNode node){
            this.node = node;
            initialTitle = node.name;
            this.title = initialTitle;
            this.viewDataKey = node.Guid;
            node.nodeView = this;

            style.left = node.positionOnGraph.x;
            style.top = node.positionOnGraph.y;

            Draw();
        }

        private void Draw()
        {

            if(node is ActionNode){
                ActionNode actionNode = node as ActionNode;

                if(actionNode.fighterAction != null){
                    this.title = initialTitle + " - " + actionNode.fighterAction.name;
                }

                /* Input Gesture Enum Field */

                var enumField = new EnumField("Input Gesture", InputGestures.None);

                enumField.Init(actionNode.inputGesture);

                enumField.RegisterValueChangedCallback(evt => { 
                    actionNode.inputGesture = (InputGestures)evt.newValue;
                });

                mainContainer.Add(enumField);

                /* Scriptable Object Field */

                var scriptableObjectField = new ObjectField("Action"){
                    objectType = typeof(FighterBaseState),
                    value = actionNode.fighterAction
                };

                scriptableObjectField.RegisterValueChangedCallback((EventCallback<ChangeEvent<UnityEngine.Object>>)(evt => {
                    actionNode.fighterAction = evt.newValue as FighterBaseState;

                    if(actionNode.fighterAction != null){
                        this.title = initialTitle + " - " + actionNode.fighterAction.name;
                    }
                    else{
                        this.title = initialTitle;
                    }
                    
                }));

                mainContainer.Add(scriptableObjectField);
            }

            CreateInputPorts();
            CreateOutputPorts();

        }

        private void CreateInputPorts(){
            //GetType could be more healthier in these if statements
            if(node is RootNode){

            }
            else if(node is ChainActionNode){
                input = InstantiatePort(Orientation.Horizontal, Direction.Input, Port.Capacity.Single, typeof(bool));
            }
            else if(node is NeutralActionNode){
                input = InstantiatePort(Orientation.Horizontal, Direction.Input, Port.Capacity.Single, typeof(bool));
            }

            if(input != null){
                input.portName = "";
                inputContainer.Add(input);
            }
        }

        private void CreateOutputPorts(){
            if(node is RootNode){
                output = InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Multi, typeof(bool));
            }
            else if(node is ChainActionNode){
                output = InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Multi, typeof(bool));
            }
            else if(node is NeutralActionNode){
                output = InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Multi, typeof(bool));
            }

            if(output != null){
                output.portName = "";
                outputContainer.Add(output);
            }
        }        

        public override void SetPosition(Rect newPos)
        {
            base.SetPosition(newPos);
            node.positionOnGraph.x = newPos.xMin;
            node.positionOnGraph.y = newPos.yMin;
        }

        public override void OnSelected()
        {
            base.OnSelected();
            OnNodeSelected?.Invoke(node);
        }

    }
}