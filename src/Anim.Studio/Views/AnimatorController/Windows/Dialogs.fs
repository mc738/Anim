namespace Anim.Studio.Views.AnimatorController.Windows

open System
open Anim.Studio.Views.AnimatorController.Data
open Avalonia.Controls
open Avalonia.Interactivity
open Avalonia.Layout
open FsToolbox.Avalonia.Dsl

module Dialogs =
        
        
    [<RequireQualifiedAccess>]
    type NewNodeType =
        | AnimationClip
        | Blend1D
        | Blend2D
        | Junction
        | PortalEntry
        | PortalExit

    type AddNodeDialog() as this =
        inherit Window()

        let mutable selectedNodeType: NewNodeType option = None

        let nameInput = TextBox.create ControlStyle.Default

        let nodeTypeSelector =
            ComboBox.create ControlStyle.Fill
            |> withItems
                false
                [ ComboBoxItem.createDefault ()
                  |> ComboBoxItem.withContent "Animation Clip"
                  |> withDataContext NewNodeType.AnimationClip
                  ComboBoxItem.createDefault ()
                  |> ComboBoxItem.withContent "Blend (1D)"
                  |> withDataContext NewNodeType.Blend1D
                  ComboBoxItem.createDefault ()
                  |> ComboBoxItem.withContent "Blend (2D)"
                  |> withDataContext NewNodeType.Blend1D


                  Separator()
                  ComboBoxItem.createDefault ()
                  |> ComboBoxItem.withContent "Junction"
                  |> withDataContext NewNodeType.Junction
                  ComboBoxItem.createDefault ()
                  |> ComboBoxItem.withContent "Portal (entry)"
                  |> withDataContext NewNodeType.PortalEntry
                  ComboBoxItem.createDefault ()
                  |> ComboBoxItem.withContent "Portal (exit)"
                  |> withDataContext NewNodeType.PortalExit ]
            |> ComboBox.withSelectionChanged this.OnTypeSelectionChanged
            |> withGridRow 0

        do

            this
            |> Window.withTitle "Add Node"
            |> Window.withWindowDecorations WindowDecorations.None
            |> Window.withWidth 512.0
            |> Window.withHeight 512.0
            |> Window.setContent (
                [ RowDefinition(GridLength.Star)
                  RowDefinition(32, GridUnitType.Pixel)
                  RowDefinition(16, GridUnitType.Pixel) ]
                |> fun rows ->
                    let rds = RowDefinitions()

                    rds.AddRange(rows)

                    rds
                |> fun rds -> Grid.create ControlStyle.Fill None (Some rds)
                |> withChildren
                    [
                      // Node type selector
                      StackPanel.create ControlStyle.Fill
                      |> withGridRow 0
                      |> withChildren
                          [ Label.create ControlStyle.Default
                            |> Label.withContent "Type"
                            |> Label.withTarget nameInput

                            nodeTypeSelector ]

                      StackPanel.create ControlStyle.Fill
                      |> withGridRow 1
                      |> withChildren
                          [ Label.create ControlStyle.Default
                            |> Label.withContent "Name"
                            |> Label.withTarget nameInput

                            nameInput ]


                      StackPanel.create ControlStyle.Fill
                      |> StackPanel.withOrientation Orientation.Horizontal
                      |> withGridRow 2
                      |> withChildren
                          [ Button.create
                                { ControlStyle.Default with
                                    Classes = [ "ok" ] }
                            |> Button.withContent "Add"
                            |> Button.onClick this.OnAddNode
                            Button.create
                                { ControlStyle.Default with
                                    Classes = [ "cancel" ] }
                            |> Button.withContent "Cancel"
                            |> Button.onClick this.OnCancel ]

                      ]
            )

        member _.OnTypeSelectionChanged(e: SelectionChangedEventArgs) =
            if e.AddedItems.Count = 0 then
                ()
            else
                let item = e.AddedItems[0]

                let cb = item :?> ComboBoxItem

                match tryGetDataContext<NewNodeType> cb with
                | Error _ -> ()
                | Ok nt -> selectedNodeType <- Some nt

        member _.OnAddNode(e: RoutedEventArgs) =
            match selectedNodeType, nameInput.Text with
            | None, _ ->
                ()
            | _, name when String.IsNullOrWhiteSpace name -> ()
            | Some nodeType, name ->
                match nodeType with
                | NewNodeType.AnimationClip -> this.Close(Nodes.AnimationClip.create name |> Some)
                //| NewNodeType.SubState -> failwith "todo"
                | NewNodeType.Blend1D -> this.Close(Nodes.Blend1D.create name |> Some)
                | NewNodeType.Blend2D -> this.Close(Nodes.Blend2D.create name |> Some)
                | NewNodeType.Junction -> this.Close(Nodes.Junction.create name |> Some)
                | NewNodeType.PortalEntry -> this.Close(Nodes.Portal.createEntry name |> Some)
                | NewNodeType.PortalExit -> this.Close(Nodes.Portal.createExit name |> Some)

        member _.OnCancel(e: RoutedEventArgs) = this.Close(None)

