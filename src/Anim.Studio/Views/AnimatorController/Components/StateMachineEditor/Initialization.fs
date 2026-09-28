namespace Anim.Studio.Views.AnimatorController.Components.StateMachineEditor

[<AutoOpen>]
module internal Initialization =

    open System
    open Anim.Studio.Views.AnimatorController.Data
    open Avalonia
    open Avalonia.Controls
    open Avalonia.Controls.Templates
    open Avalonia.Data
    open Avalonia.Markup.Xaml.Templates
    open Nodify    
    
    let createItemTemplate () =
        let itemTemple = DataTemplate()

        itemTemple.DataType <- typeof<NodeViewModel>

        itemTemple.Content <-
            Func<IServiceProvider, obj>(fun _ ->
                let node = Node()

                // Bit ugly but it will do for now.
                node.BindClass("clip", Binding("NodeType.IsClip"), null) |> ignore
                node.BindClass("blend-1d", Binding("NodeType.IsBlend1D"), null) |> ignore
                node.BindClass("blend-2d", Binding("NodeType.IsBlend2D"), null) |> ignore
                node.BindClass("junction", Binding("NodeType.IsJunction"), null) |> ignore
                node.BindClass("portal", Binding("NodeType.IsPortal"), null) |> ignore
                node.BindClass("entry", Binding("NodeType.IsEntry"), null) |> ignore

                //node.Classes.Add("clip")

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

        itemTemple

    let createConnectionTemplate () =
        let connDT = DataTemplate()

        connDT.Content <-
            Func<IServiceProvider, obj>(fun _ ->
                let conn = LineConnection()
                conn.Bind(LineConnection.SourceProperty, Binding("Source.Anchor")) |> ignore
                conn.Bind(LineConnection.TargetProperty, Binding("Target.Anchor")) |> ignore

                TemplateResult<Control>(conn, null) :> obj)

        connDT.DataType <- typeof<ConnectionViewModel>

        connDT

    let createPendingConnectionTemplate () =
        let pendingConnectionTemplate = DataTemplate()

        pendingConnectionTemplate.DataType <- typeof<PendingConnectionViewModel>

        pendingConnectionTemplate.Content <-
            Func<IServiceProvider, obj>(fun _ ->
                let pendingConnection = PendingConnection()

                pendingConnection.Bind(PendingConnection.StartedCommandProperty, Binding("StartedCommand"))
                |> ignore

                pendingConnection.Bind(PendingConnection.CompletedCommandProperty, Binding("CompletedCommand"))
                |> ignore

                pendingConnection.AllowOnlyConnectors <- true

                TemplateResult<Control>(pendingConnection, null) :> obj)

        pendingConnectionTemplate
    

