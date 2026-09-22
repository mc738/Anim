namespace Anim

open Anim.Core

[<RequireQualifiedAccess>]
type AnimatorValue =
    | Int of int
    | Bool of bool
    | Float of float32
    // Trigger is a special type of bool that will be automatically
    | Trigger of bool

type AnimatorFlag() =
    let mutable value = false

    member _.Value = value

type AnimatorFlags = { Values: AnimatorFlag }


type Condition =
    | GreaterThan
    | GreaterThanOrEqual
    | LessThanOrEqual
    | LessThan
    | Equal
    | NotEqual

type AnimationTransitionCondition =
    | ValueGreaterThan of int * AnimatorValue
    | ValueGreaterOrEqual of int * AnimatorValue
    | ValueLessOrEqual of int * AnimatorValue
    | ValueLess of int * AnimatorValue
    | ValueEqual of int * AnimatorValue


type AnimationTransition = { Duration: float32 }

type AnimationState =
    | Clip of AnimationState
    | SubState of AnimationStateMachine


and AnimationStateMachine =
    { DefaultState: AnimationState
      States: AnimationState list
      Transitions: AnimationState list }

type AnimationEvent = { Name: string; Time: float32 }


// The animation controller is a flat packed representation of the state machine.
// Each state can have a guid are design time that is translated into an index at runtime.
// The should remove some of the need for nested structures.
type AnimationController =
    { States: AnimationState


    }

// This will store all animation controllers, armatures, etc.
// Probably in flat arrays.
// Then each one can be updated in a single pass.
// The idea is to separate the individual animation controllers from the overall definitions.
// Because multiple entities could have use the same animation controller but be in different states.
// A few ways to handle this:
//
// * 3 Tier -> System, Controller (definition), StateMachine (instance)
// * 2 Tier -> System, Controller -> keep objects state in controller in collection.
type AnimationSystem() =
    let animators = ResizeArray<Animator>()
    let controllers = ResizeArray<AnimationController>()

    member _.Tick(dt: float32) =
        for animator in animators do
            animator.Tick(dt)

        ()
