namespace Anim.Studio.Views.AnimatorController

open System
open Anim.Studio.Views.AnimatorController.StateMachineEditor
open Avalonia
open Avalonia.Animation
open Avalonia.Controls
open Avalonia.Media
open FsToolbox.Avalonia.Dsl

type AnimationControllerView() as this =
    inherit Grid()

    let mutable leftPanelExpanded = true
    let mutable rightPanelExpanded = true

    let leftSidePanel =
        Grid.create ControlStyle.Fill None None
        |> withBackground Brushes.BlueViolet
        |> withGridRow 1
        |> withGridColumn 0

    let rightSidePanel =
        Grid.create ControlStyle.Fill None None
        |> withBackground Brushes.Azure
        |> withGridRow 1
        |> withGridColumn 2

    let mainContent =
        Grid.create ControlStyle.Fill None None
        |> withBackground Brushes.Pink
        |> withGridRow 1
        |> withGridColumn 1

    let mainMenu =
        Menu.create ControlStyle.Fill
        |> withGridRow 0
        |> withGridColumnSpan 2
        |> Menu.withMenuItems [
            MenuItem.create { ControlStyle.Default with Classes = [ "main-menu" ] }
            |> MenuItem.withHeader "Controller"
            |> MenuItem.withChildren [
                MenuItem.create ControlStyle.Default
                |> MenuItem.withHeader "New State Machine"
            ]
            
            MenuItem.create { ControlStyle.Default with Classes = [ "main-menu" ] }
            |> MenuItem.withHeader "Armature"
            |> MenuItem.withChildren [
                MenuItem.create ControlStyle.Default
                |> MenuItem.withHeader "New Armature"
            ]
            
            MenuItem.create { ControlStyle.Default with Classes = [ "main-menu" ] }
            |> MenuItem.withHeader "Layers"
            |> MenuItem.withChildren [
                MenuItem.create ControlStyle.Default
                |> MenuItem.withHeader "New Layer"
            ]
            
            
            MenuItem.create { ControlStyle.Default with Classes = [ "main-menu" ] }
            |> MenuItem.withHeader "Masks"
            |> MenuItem.withChildren [
                MenuItem.create ControlStyle.Default
                |> MenuItem.withHeader "New Mask"
            ]
        ]
    
    do

        leftSidePanel
        |> setChildren
            [ Button.create ControlStyle.Fill
              |> Button.withContent ">"
              |> Button.onClick (fun _ -> this.ToggleLeftPanel()) ]
            
        rightSidePanel
        |> setChildren
            [ Button.create ControlStyle.Default
              |> Button.withContent ">"
              |> Button.onClick (fun _ -> this.ToggleRightPanel()) ]

        mainContent
        |> setChildren [ StateMachineEditorComponent() ]       
        
        leftSidePanel.Width <- 256
        
        rightSidePanel.Width <- 256

        let mutable t = DoubleTransition()
        
        t.Duration <- TimeSpan.FromSeconds 0.1
        t.Property <- Grid.WidthProperty
        
        let transitions = Transitions()
        
        transitions.Add(t)
        
        leftSidePanel.Transitions <- transitions
        rightSidePanel.Transitions <- transitions
        

        this
        |> Grid.withRows [ RowDefinition(24, GridUnitType.Pixel); RowDefinition(GridLength.Star) ]
        |> Grid.withColumns
            [ ColumnDefinition(GridLength.Auto)
              ColumnDefinition(GridLength.Star)
              ColumnDefinition(GridLength.Auto) ]
        |> setChildren [ mainMenu; leftSidePanel; mainContent; rightSidePanel ]


    member _.ToggleLeftPanel() =
        leftPanelExpanded <- not leftPanelExpanded

        if leftPanelExpanded then
            leftSidePanel.Width <- 256
        else
            leftSidePanel.Width <- 56
            
    
    member _.ToggleRightPanel() =
        rightPanelExpanded <- not rightPanelExpanded

        if rightPanelExpanded then
            rightSidePanel.Width <- 256
        else
            rightSidePanel.Width <- 56
