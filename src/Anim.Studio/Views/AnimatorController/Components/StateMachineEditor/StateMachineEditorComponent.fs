namespace Anim.Studio.Views.AnimatorController.Components.StateMachineEditor

open System
open System.Collections.Generic
open System.Collections.ObjectModel
open System.ComponentModel
open Anim.Studio.Views.AnimatorController.Data
open Anim.Studio.Views.AnimatorController.Data.EventArgs
open Avalonia
open Avalonia.Controls
open Avalonia.Controls.Primitives
open Avalonia.Controls.Templates
open Avalonia.Data
open Avalonia.Input
open Avalonia.Layout
open Avalonia.Markup.Xaml.Templates
open Avalonia.Media
open Avalonia.Styling
open FsToolbox.Avalonia.Commands
open FsToolbox.Avalonia.Dsl
open Nodify

type StateMachineEditorComponent() as this =
    inherit Grid()

    let showAddNodeRequested = Event<EventArgs>()

    let nodeSelected = Event<NodeSelectedEventArgs>()

    let editor = EditorViewModel()

    let nodeEditor = NodifyEditor()

    let selectedNode: NodeViewModel option = None

    let mutable lastMousePosition = Point(0, 0)

    do
        this.InitializeEditor()

        this.KeyDown.Add(fun e ->
            match e.Key with
            | Key.A ->
                match e.KeyModifiers with
                | KeyModifiers.Shift -> showAddNodeRequested.Trigger(EventArgs())
                | _ -> ()
            | _ -> ())

        this.PointerMoved.Add(fun e -> lastMousePosition <- e.GetPosition(this))

        this |> setChildren [ nodeEditor ]

    member private _.InitializeEditor() =
        // Create and set the item template.
        let itemTemple = createItemTemplate ()
        nodeEditor.ItemTemplate <- itemTemple

        // Create and set the connection template.
        let connDT = createConnectionTemplate ()
        nodeEditor.ConnectionTemplate <- connDT

        // Create and set the pending connection template.
        let pendingConnectionTemplate = createPendingConnectionTemplate ()
        nodeEditor.PendingConnectionTemplate <- pendingConnectionTemplate

        // Set bindings.
        nodeEditor
        |> setBindings
            [ NodifyEditor.ItemsSourceProperty, (Binding.create () |> Binding.withSource editor.Nodes :> BindingBase)
              NodifyEditor.ConnectionsProperty,
              (Binding.create () |> Binding.withSource editor.Connections :> BindingBase)
              NodifyEditor.PendingConnectionProperty,
              (Binding.create () |> Binding.withSource editor.PendingConnection :> BindingBase) ]

        // Add style and node location.
        let itemContainerStyle = Style(fun x -> x.OfType<ItemContainer>())
        itemContainerStyle.Add(Setter(ItemContainer.LocationProperty, Binding("Location")))

        nodeEditor.Styles.Add(itemContainerStyle)

        nodeEditor.SelectionChanged.Add(fun e ->
            if e.AddedItems.Count = 0 then
                ()
            else
                let item = e.AddedItems[0]

                let node = item :?> NodeViewModel

                nodeSelected.Trigger(NodeSelectedEventArgs(node)))

    [<CLIEvent>]
    member this.ShowAddNodeRequested = showAddNodeRequested.Publish

    [<CLIEvent>]
    member this.NodeSelected = nodeSelected.Publish

    member _.AttemptConnection(source: ConnectorPortViewModel, target: ConnectorPortViewModel) =
        this.ConnectInternal(source, target)

    member private _.ConnectInternal(source: ConnectorPortViewModel, target: ConnectorPortViewModel) =
        editor.AddConnection(source, target)

    member _.AddNode(node: NodeViewModel, useLastMousePosition: bool) =
        if useLastMousePosition then
            node.Location <- lastMousePosition

        editor.AddNode(node)
