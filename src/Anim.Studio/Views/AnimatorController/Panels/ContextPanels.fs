namespace Anim.Studio.Views.AnimatorController.Panels

open Anim.Studio.Views.AnimatorController.Data
open Avalonia.Controls
open FsToolbox.Avalonia.Dsl

module ContextPanels =
    
    type NodeContextPanel() as this =
        inherit Grid()
        
        let mutable node: NodeViewModel option = None
        
        let nameInput =
            TextBox.create ControlStyle.Default
            
        do  
            this
            |> Grid.withRows [  RowDefinition(36, GridUnitType.Pixel); RowDefinition(GridLength.Star) ]
            |> setChildren [
                Label.create ControlStyle.Default
                |> Label.withContent "Node Settings"
                |> withClass "panel-title"
                |> withGridRow 0
                
                StackPanel.create ControlStyle.Fill
                |> withGridRow 1
                |> withChildren [
                    Label.create ControlStyle.Default
                    |> Label.withContent "Name"
                    |> Label.withTarget nameInput
                    
                    nameInput
                    |> TextBox.onTextChanged (fun _ ->
                        match node with
                        | Some node -> node.Name <- nameInput.Text
                        | None -> ()
                        ())
                    
                ]
            ]
          
        member _.SetNode(newNode: NodeViewModel) =
            node <- Some newNode
            nameInput.Text <- newNode.Name
            