namespace Anim.Studio.Views.AnimatorController.Data

open System

module EventArgs =
    
    type NodeSelectedEventArgs(node: NodeViewModel) =
        inherit EventArgs()
        
        member _.Node = node

