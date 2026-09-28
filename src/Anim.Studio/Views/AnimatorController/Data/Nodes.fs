namespace Anim.Studio.Views.AnimatorController.Data

open System

module Nodes =
    
    module AnimationClip =
        
        let create (name: string) =
            let node = NodeViewModel(Guid.NewGuid(), name, NodeType.Clip)
            
            node.AddInputPort("Input", true)
            node.AddOutputPort("On complete", true)
            
            node
            
    module Blend1D =
          
        let create (name: string) =
            let node = NodeViewModel(Guid.NewGuid(), name, NodeType.Blend1D)
            
            node.AddInputPort("Input", true)
            
            node
            
    module Blend2D =
          
        let create (name: string) =
            let node = NodeViewModel(Guid.NewGuid(), name, NodeType.Blend2D)
            
            node.AddInputPort("Input", true)
            
            node
            
    module Junction =
        let create (name: string) =
            let node = NodeViewModel(Guid.NewGuid(), name, NodeType.Junction)
            
            node.AddInputPort("Input", true)
            node.AddOutputPort("Output", true)
            
            node
            
    module Portal =
        let createEntry (name: string) =
            let node = NodeViewModel(Guid.NewGuid(), "", NodeType.Portal)
            
            node.AddInputPort(name, true)
            
            node
            
        
        let createExit (name: string) =
            let node = NodeViewModel(Guid.NewGuid(), "", NodeType.Portal)
            
            node.AddOutputPort(name, true)
            
            node
            

