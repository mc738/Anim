namespace Anim.Studio.Views.AnimatorController.Data

open System
open Avalonia
open FsToolbox.Avalonia.Commands

[<AutoOpen>]
module rec ViewModels =

    open System.Collections.ObjectModel
    open System.ComponentModel

    type NodeType =
        | Entry
        | Clip
        | SubState
        | Blend1D
        | Blend2D
        | Junction
        | Portal
        | Exit

    [<RequireQualifiedAccess>]
    type ConnectorType =
        | Input
        | Output
        | Any

    type ConnectorPortViewModel(name: string, parent: NodeViewModel, connectorType: ConnectorType, multipleAllow: bool) as this
        =
        let mutable anchor: Point = Point(0, 0)
        let mutable showConnection = true

        let propertyChanged =
            new Event<PropertyChangedEventHandler, PropertyChangedEventArgs>()

        interface INotifyPropertyChanged with
            [<CLIEvent>]
            member this.PropertyChanged = propertyChanged.Publish

        member private _.OnPropertyChanged(propertyName: string) =
            propertyChanged.Trigger(this, PropertyChangedEventArgs(propertyName))

        member this.Anchor
            with get () = anchor
            and set (value) =
                anchor <- value
                this.OnPropertyChanged("Anchor")

        member _.Name = name


        member _.Parent = parent

        member _.ConnectorType = connectorType

        member _.MultipleAllowed = multipleAllow

        member this.ShowConnection
            with get () = showConnection
            and set (value) =
                showConnection <- value
                this.OnPropertyChanged("Anchor")

    type ConnectionViewModel(source: ConnectorPortViewModel, target: ConnectorPortViewModel) as this =
        member _.Source = source

        member _.Target = target

    type PendingConnectionViewModel(editor: EditorViewModel) as this =
        let mutable source: ConnectorPortViewModel option = None

        let startCommand =
            DelegateCommand<ConnectorPortViewModel>((fun v -> this.SetSource(v)), (fun _ -> true))

        let completeCommand =
            DelegateCommand<ConnectorPortViewModel>((fun v -> this.HandleConnection(v)), (fun _ -> true))

        member _.Source = source
        member _.StartedCommand = startCommand
        member _.CompletedCommand = completeCommand

        member _.SetSource(v: ConnectorPortViewModel) = source <- Some v

        member _.ClearSource() = source <- None

        member _.HandleConnection(target: ConnectorPortViewModel) =

            match source with
            | None -> ()
            | Some source -> editor.AddConnection(source, target)

            this.ClearSource()

    type NodeViewModel(id: Guid, name: string, nodeType: NodeType) as this =

        let inputs = ObservableCollection<ConnectorPortViewModel>()
        let outputs = ObservableCollection<ConnectorPortViewModel>()

        let propertyChanged =
            new Event<PropertyChangedEventHandler, PropertyChangedEventArgs>()

        let mutable location = Point(0, 0)

        interface INotifyPropertyChanged with
            [<CLIEvent>]
            member this.PropertyChanged = propertyChanged.Publish

        member private _.OnPropertyChanged(propertyName: string) =
            propertyChanged.Trigger(this, PropertyChangedEventArgs(propertyName))

        member _.Id = id

        member _.Name = name

        member _.NodeType = nodeType
        
        member _.Inputs = inputs
        member _.Outputs = outputs

        member this.Location
            with get () = location
            and set (value) =
                location <- value
                this.OnPropertyChanged("Location")

        member _.AddInputPort(name: string, multipleAllowed: bool) =
            inputs.Add(ConnectorPortViewModel(name, this, ConnectorType.Input, multipleAllowed))

        member _.AddOutputPort(name: string, multipleAllowed: bool) =
            outputs.Add(ConnectorPortViewModel(name, this, ConnectorType.Output, multipleAllowed))


    type EditorViewModel() as this =
        let nodes = ObservableCollection<NodeViewModel>()
        let connections = ObservableCollection<ConnectionViewModel>()
        let pendingConnection = PendingConnectionViewModel(this)

        member _.Nodes = nodes

        member _.Connections = connections

        member _.PendingConnection = pendingConnection

        member _.AddNode(node: NodeViewModel) = nodes.Add(node)

        member _.AddConnection(source: ConnectorPortViewModel, target: ConnectorPortViewModel) =
            // Do any checks.
            // Like are the two connector ports the same?

            if source.Parent.Id = target.Parent.Id then
                ()
            else
                match source.ConnectorType, target.ConnectorType with
                | ConnectorType.Output, ConnectorType.Input -> Result.Ok(source, target)
                | ConnectorType.Input, ConnectorType.Output -> Result.Ok(target, source)
                | ConnectorType.Any, _ -> Error()
                | _, ConnectorType.Any -> Error()
                | _ -> Error()
                |> Result.iter (fun (source, target) ->
                    // Run any extra condition checks here.

                    connections.Add(ConnectionViewModel(source, target)))