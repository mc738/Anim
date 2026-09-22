namespace Anim.Core

open System.Numerics

[<AutoOpen>]
module Types =
    
    type NodeIndex = int
    type JointIndex = int
        
  
    type ArmatureBone = {
        NodeId: int
        Name: string
        Children: ArmatureBone list
    }
    
    type ArmatureJoint =
        {
            JointIndex: JointIndex
            NodeIndex: NodeIndex
            InverseBindMatrix: Matrix4x4
            DefaultTranslation: Vector3
            DefaultRotation: Quaternion
            DefaultScale: Vector3
        }
        
        static member Default =
            {
                JointIndex = -1
                NodeIndex = -1
                InverseBindMatrix = Matrix4x4.Identity
                DefaultTranslation = Vector3.Zero
                DefaultRotation = Quaternion.Identity
                DefaultScale = Vector3.One
            }

    type Armature = {
        RootBones: ArmatureBone list
        Joints: Map<int, ArmatureJoint>
    }
    
    type Keyframe<'T> = {Time: float32; Value: 'T}
    
    type TranslationKeyFrame = { Time: float32; Value: Vector3 }

    type RotationKeyFrame = { Time: float32; Value: Quaternion }

    type ScaleKeyFrame = { Time: float32; Value: Vector3 }

    type Channel =
        { NodeId: int
          TranslationKeyframes: Keyframe<Vector3> array
          RotationKeyframes: Keyframe<Quaternion> array
          ScaleKeyframes: Keyframe<Vector3> array }

    type AnimationEvent =
        {
            Name: string
            Time: float32
        }
    
    type AnimationClip =
        { Name: string
          Duration: float32
          IsLoop: bool
          Channels: Map<int, Channel>
          Events: AnimationEvent array }
    
