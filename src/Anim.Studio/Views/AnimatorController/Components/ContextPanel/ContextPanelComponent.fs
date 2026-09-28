namespace Anim.Studio.Views.AnimatorController.Components.ContextPanel

open System
open Anim.Studio.Views.AnimatorController.Data
open Avalonia.Animation
open Avalonia.Controls
open Avalonia.Layout
open Avalonia.Media
open FsToolbox.Avalonia.Dsl

type ContextPanelComponent() as this =
    inherit Grid()
    
    let mutable selectedNode: NodeViewModel option = None
    
    let mutable isExpanded = false
    
    let content = ContentControl()
    
    let sideBarWidth = 36.
    let contentWidth = 256.

    do
        this.HorizontalAlignment <- HorizontalAlignment.Stretch
        this.VerticalAlignment <- VerticalAlignment.Stretch
        
        this.Width <- sideBarWidth
        
        let mutable t = DoubleTransition()

        t.Duration <- TimeSpan.FromSeconds 0.1
        t.Property <- Grid.WidthProperty

        let transitions = Transitions()

        transitions.Add(t)
        
        this.Transitions <- transitions
        
        content.Width <- contentWidth - 20.
        content.HorizontalAlignment <- HorizontalAlignment.Stretch
        content.VerticalAlignment <- VerticalAlignment.Stretch
        content.FlowDirection <- FlowDirection.LeftToRight
        
        this
        |> withGridRow 1
        |> withGridColumn 2
        |> setChildren
            [ StackPanel.create ControlStyle.Fill
              |> StackPanel.withOrientation Orientation.Horizontal
              |> StackPanel.withFlowDirection FlowDirection.RightToLeft
              |> withChildren
                  [ StackPanel.create ControlStyle.Fill
                    |> withBackground Brushes.Transparent
                    |> withChildren
                        [ Button.create ControlStyle.Default
                          |> withWidth 36.0
                          |> withHeight 36.0
                          |> Button.withPathIcon "edit_settings_regular"
                          |> Button.onClick (fun _ -> this.ToggleNodeSettingPanel()) ]

                    content
                    |> Border.createFill
                    |> Border.withMargin  ] ]

    member _.IsExpanded = isExpanded
    
    member _.ToggleNodeSettingPanel() =
        isExpanded <- not isExpanded
        
        if isExpanded then
            content.Content <-
                Grid.create ControlStyle.Fill None None
                |> withChildren [ Label.create ControlStyle.Default |> Label.withContent "Node Settings" ]
            this.Width <- contentWidth + sideBarWidth
        else
            content.Content <- null
            this.Width <- sideBarWidth
    
    member _.SetSelectedNode(node: NodeViewModel) =
        selectedNode <- Some node
        content.Content <-
                StackPanel.create ControlStyle.Fill
                |> withChildren [
                    Label.create ControlStyle.Default |> Label.withContent node.Name
                    Label.create ControlStyle.Default |> Label.withContent (node.Id.ToString())
                    Button.create ControlStyle.Default
                    |> Button.withContent "Add Transition"
                    |> withHeight 24.
                    |> Button.onClick (fun _ -> node.AddOutputPort("Test", true))
                ]
        
        