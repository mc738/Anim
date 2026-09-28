namespace Anim.Studio.Views.AnimatorController.StateMachineEditor

open System
open System.Collections.Generic
open System.Collections.ObjectModel
open System.ComponentModel
open Avalonia
open Avalonia.Controls
open Avalonia.Controls.Primitives
open Avalonia.Controls.Templates
open Avalonia.Data
open Avalonia.Layout
open Avalonia.Markup.Xaml.Templates
open Avalonia.Styling
open FsToolbox.Avalonia.Commands
open FsToolbox.Avalonia.Dsl
open Nodify

type AnimationState(id: Guid) =
    let mutable name = "[blank]"


    member _.Name = name

    member _.Id = id

type StateTransition(fromId: Guid, toId: Guid) =

    member _.From = fromId
    member _.To = toId

type StateMachineEditorComponent() as this =
    inherit Grid()

    let editor = EditorViewModel()
    
    // Nodes
    let states = Dictionary<Guid, AnimationState>()

    // Edges
    let transitions = ResizeArray<StateTransition>()

    let nodeEditor = NodifyEditor()

    //let nodes = ObservableCollection<NodeViewModel>()
    //let connections = ObservableCollection<ConnectionViewModel>()

    do
        this.InitializeEditor()
        
        let n1 =
            NodeViewModel(
                "Test",
                [ ConnectorPortViewModel("Input 1"); ConnectorPortViewModel("Input 2") ]
                |> ObservableCollection<ConnectorPortViewModel>,
                [

                  ConnectorPortViewModel("Output 1")
                  ConnectorPortViewModel("Output 2") ]
                |> ObservableCollection<ConnectorPortViewModel>
            )
            
        n1.Location <- Point(60.0, 60.0)

        let n2 =
            NodeViewModel(
                "Test 1",
                [ ConnectorPortViewModel("Input 1"); ConnectorPortViewModel("Input 2") ]
                |> ObservableCollection<ConnectorPortViewModel>,
                [

                  ConnectorPortViewModel("Output 1")
                  ConnectorPortViewModel("Output 2") ]
                |> ObservableCollection<ConnectorPortViewModel>
            )
        
        n2.Location <- Point(360.0, 120.0)

        editor.Nodes.Add(n1)
        editor.Nodes.Add(n2)

        editor.Connections.Add(new ConnectionViewModel(n1.Outputs[0], n2.Inputs[0]))

        this |> setChildren [ nodeEditor ]

    member private _.InitializeEditor() =
        let itemTemple = DataTemplate()

        itemTemple.DataType <- typeof<NodeViewModel>

        itemTemple.Content <-
            Func<IServiceProvider, obj>(fun _ ->
                let node = Node()

                let inputDT = DataTemplate()
                inputDT.DataType <- typeof<ConnectorPortViewModel>

                inputDT.Content <-
                    Func<IServiceProvider, obj>(fun _ ->
                        let input = NodeInput()
                        let b = Binding("Name")


                        input.Bind(NodeInput.HeaderProperty, b) |> ignore
                        input.IsConnected <- true
                        
                        
                        input.Bind(NodeInput.IsConnectedProperty, Binding("IsConnected")) |> ignore
                        //input.Bind(NodeInput.AnchorProperty, BindingBase())

                        let anchorBinding = Binding("Anchor")

                        anchorBinding.Mode <- BindingMode.OneWayToSource

                        input.Bind(NodeInput.AnchorProperty, anchorBinding) |> ignore
                        
                        TemplateResult<Control>(input, null) :> obj)

                let outputDT = DataTemplate()
                outputDT.DataType <- typeof<ConnectorPortViewModel>

                outputDT.Content <-
                    Func<IServiceProvider, obj>(fun _ ->
                        let output = NodeOutput()

                        let b = Binding("Name")

                        output.Bind(NodeOutput.HeaderProperty, b) |> ignore
                        
                        output.IsConnected <- true
                        
                        output.Bind(NodeOutput.IsConnectedProperty, Binding("IsConnected")) |> ignore

                        let anchorBinding = Binding("Anchor")

                        anchorBinding.Mode <- BindingMode.OneWayToSource

                        output.Bind(NodeOutput.AnchorProperty, anchorBinding) |> ignore

                        TemplateResult<Control>(output, null) :> obj)

                node.InputConnectorTemplate <- inputDT
                node.OutputConnectorTemplate <- outputDT

                node.Bind(Node.HeaderProperty, Binding("Name")) |> ignore
                node.Bind(Node.InputProperty, Binding("Inputs")) |> ignore
                node.Bind(Node.OutputProperty, Binding("Outputs")) |> ignore
                
                TemplateResult<Control>(node, null) :> obj)


        let sourceBinding = Binding()
        sourceBinding.Source <- editor.Nodes
        nodeEditor.Bind(NodifyEditor.ItemsSourceProperty, sourceBinding) |> ignore

        let connectionsSourceBinding = Binding()

        connectionsSourceBinding.Source <- editor.Connections

        nodeEditor.Bind(NodifyEditor.ConnectionsProperty, connectionsSourceBinding)
        |> ignore
        
        nodeEditor.ItemTemplate <- itemTemple

        let connDT = DataTemplate()

        connDT.Content <-
            Func<IServiceProvider, obj>(fun _ ->
                let conn = LineConnection()
                conn.Bind(LineConnection.SourceProperty, Binding("Source.Anchor")) |> ignore
                conn.Bind(LineConnection.TargetProperty, Binding("Target.Anchor")) |> ignore
                
                TemplateResult<Control>(conn, null) :> obj)
        connDT.DataType <- typeof<ConnectionViewModel>
        
        
        nodeEditor.ConnectionTemplate <- connDT
        
        let pendingConnectionTemplate = DataTemplate()
        
        pendingConnectionTemplate.DataType <- typeof<PendingConnectionViewModel>
        pendingConnectionTemplate.Content <-
            Func<IServiceProvider, obj>(fun _ ->
                let pendingConnection = PendingConnection()
                
                pendingConnection.StartedCommand <- editor.PendingConnection.StartedCommand
                pendingConnection.CompletedCommand <- editor.PendingConnection.CompletedCommand
                pendingConnection.AllowOnlyConnectors <- true
                
                TemplateResult<Control>(pendingConnection, null) :> obj)
            
        nodeEditor.PendingConnectionTemplate <- pendingConnectionTemplate
        
        let pendingSourceBinding = Binding()
        pendingSourceBinding.Source <- editor.PendingConnection
        nodeEditor.Bind(NodifyEditor.PendingConnectionProperty, pendingSourceBinding) |> ignore
        
        let itemContainerStyle = Style(fun x -> x.OfType<ItemContainer>())
        itemContainerStyle.Add(Setter(ItemContainer.LocationProperty, Binding("Location")))
        
        nodeEditor.Styles.Add(itemContainerStyle)
        
        ()
        
        
    member private _.ConnectInternal(source: ConnectorPortViewModel, target: ConnectorPortViewModel) = 
        editor.Connections.Add(ConnectionViewModel(source, target))
        