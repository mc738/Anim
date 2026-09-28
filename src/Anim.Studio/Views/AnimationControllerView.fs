namespace Anim.Studio.Views.AnimatorController

open System
open Anim.Studio.Views.AnimatorController.Components.ContextPanel
open Anim.Studio.Views.AnimatorController.Components.StateMachineEditor
open Anim.Studio.Views.AnimatorController.Data
open Anim.Studio.Views.AnimatorController.Windows.Dialogs
open Avalonia
open Avalonia.Animation
open Avalonia.Controls
open Avalonia.Interactivity
open Avalonia.Layout
open Avalonia.Media
open FsToolbox.Avalonia.Dsl

type AnimationControllerView(parentWindow: Window) as this =
    inherit Grid()

    let mutable leftPanelExpanded = true
    let mutable rightPanelExpanded = true

    let stateMachineEditor = StateMachineEditorComponent()
    
    let contextPanel = ContextPanelComponent()

    //let rightPanelContent = ContentControl()

    let leftSidePanel =
        Grid.create ControlStyle.Fill None None
        |> withBackground Brushes.BlueViolet
        |> withGridRow 1
        |> withGridColumn 0

    //let rightSidePanel =
    //    Grid.create ControlStyle.Fill None None
    //    |> withGridRow 1
    //    |> withGridColumn 2

    let mainContent =
        Grid.create ControlStyle.Fill None None
        |> withBackground Brushes.Pink
        |> withGridRow 1
        |> withGridColumn 1

    let mainMenu =
        Menu.create ControlStyle.Fill
        |> withGridRow 0
        |> withGridColumnSpan 2
        |> Menu.withMenuItems
            [ MenuItem.create
                  { ControlStyle.Default with
                      Classes = [ "main-menu" ] }
              |> MenuItem.withHeader "Controller"
              |> MenuItem.withChildren
                  [ MenuItem.create ControlStyle.Default |> MenuItem.withHeader "New State Machine" ]

              MenuItem.create
                  { ControlStyle.Default with
                      Classes = [ "main-menu" ] }
              |> MenuItem.withHeader "Armature"
              |> MenuItem.withChildren [ MenuItem.create ControlStyle.Default |> MenuItem.withHeader "New Armature" ]

              MenuItem.create
                  { ControlStyle.Default with
                      Classes = [ "main-menu" ] }
              |> MenuItem.withHeader "Layers"
              |> MenuItem.withChildren [ MenuItem.create ControlStyle.Default |> MenuItem.withHeader "New Layer" ]


              MenuItem.create
                  { ControlStyle.Default with
                      Classes = [ "main-menu" ] }
              |> MenuItem.withHeader "Masks"
              |> MenuItem.withChildren [ MenuItem.create ControlStyle.Default |> MenuItem.withHeader "New Mask" ] ]

    do

        stateMachineEditor.ShowAddNodeRequested.Add(fun _ -> this.ShowAddNodeDialog())
        stateMachineEditor.NodeSelected.Add(fun e -> contextPanel.SetSelectedNode(e.Node))

        //rightPanelContent.HorizontalAlignment <- HorizontalAlignment.Stretch
        //rightPanelContent.VerticalAlignment <- VerticalAlignment.Stretch

        leftSidePanel
        |> setChildren
            [ Button.create ControlStyle.Fill
              |> Button.withContent ">"
              |> Button.onClick (fun _ -> this.ToggleLeftPanel()) ]


        //rightSidePanel
        //|> setChildren
        //    [ StackPanel.create ControlStyle.Fill
        //      |> StackPanel.withOrientation Orientation.Horizontal
        //      |> StackPanel.withFlowDirection FlowDirection.RightToLeft
        //      |> withChildren
        //          [ StackPanel.create ControlStyle.Fill
        //            |> withBackground Brushes.Transparent
        //            |> withChildren
        //                [ Button.create ControlStyle.Default
        //                  |> withWidth 56.0
        //                  |> withHeight 56.0
        //                  |> Button.withContent "<"
        //                  |> Button.onClick (fun _ -> this.ToggleNodeSettingPanel()) ]
//
        //            rightPanelContent ] ]

        mainContent |> setChildren [ stateMachineEditor ]

        leftSidePanel.Width <- 56.

        //rightSidePanel.Width <- 56

        let mutable t = DoubleTransition()

        t.Duration <- TimeSpan.FromSeconds 0.1
        t.Property <- Grid.WidthProperty

        let transitions = Transitions()

        transitions.Add(t)

        leftSidePanel.Transitions <- transitions
        //rightSidePanel.Transitions <- transitions


        this
        |> Grid.withRows [ RowDefinition(24, GridUnitType.Pixel); RowDefinition(GridLength.Star) ]
        |> Grid.withColumns
            [ ColumnDefinition(GridLength.Auto)
              ColumnDefinition(GridLength.Star)
              ColumnDefinition(GridLength.Auto) ]
        |> setChildren [ mainMenu; leftSidePanel; mainContent; contextPanel ]


    member _.ToggleLeftPanel() =
        leftPanelExpanded <- not leftPanelExpanded

        if leftPanelExpanded then
            leftSidePanel.Width <- 256
        else
            leftSidePanel.Width <- 56


    //member _.ToggleNodeSettingPanel() =
    //    rightPanelExpanded <- not rightPanelExpanded
    //    
    //    if rightPanelExpanded then
    //        rightPanelContent.Content <-
    //            Grid.create ControlStyle.Fill None None
    //            |> withChildren [ Label.create ControlStyle.Default |> Label.withContent "Node Settings" ]
    //        rightSidePanel.Width <- 256. + 56.
    //    else
    //        rightPanelContent.Content <- null
    //        rightSidePanel.Width <- 56.
//
    //member _.ToggleRightPanel() =
    //    rightPanelExpanded <- not rightPanelExpanded
//
    //    if rightPanelExpanded then
    //        rightSidePanel.Width <- 256
    //    else
    //        rightSidePanel.Width <- 56
//
    member _.ShowAddNodeDialog() =
        async {

            let assetStoreManagerWindow = AddNodeDialog()

            let! result =
                assetStoreManagerWindow.ShowDialog<NodeViewModel option>(parentWindow)
                |> Async.AwaitTask

            match result with
            | None -> ()
            | Some node -> stateMachineEditor.AddNode(node, true)

        }
        |> Async.StartImmediate
