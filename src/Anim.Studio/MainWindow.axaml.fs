namespace Anim.Studio

open Anim.Studio.Views.AnimatorController
open Avalonia
open Avalonia.Controls
open Avalonia.Layout
open Avalonia.Markup.Xaml
open Avalonia.Media
open FsToolbox.Avalonia.Dsl
open Nodify

type MainWindow() as this =
    inherit Window()

    let animatorControllerView = AnimationControllerView()
    
    let layout =
        [ RowDefinition(32, GridUnitType.Pixel)
          RowDefinition(GridLength.Star)
          RowDefinition(16, GridUnitType.Pixel) ]
        |> fun rows ->
            let rds = RowDefinitions()

            rds.AddRange(rows)

            rds
        |> fun rds -> Grid.create ControlStyle.Fill None (Some rds)

    let controllerTab =
        TabItem.create ControlStyle.Fill
        |> TabItem.withHeader "Controller"
        |> TabItem.withContent animatorControllerView

    let libraryTab =
        TabItem.create ControlStyle.Fill
        |> TabItem.withHeader "Library"
        |> TabItem.withContent (StackPanel.create ControlStyle.Fill |> withBackground Brushes.Azure)

    let testTab =
        TabItem.create ControlStyle.Fill
        |> TabItem.withHeader "Test"
        |> TabItem.withContent (StackPanel.create ControlStyle.Fill |> withBackground Brushes.Azure)

    let mainTabControl =
        TabControl.create ControlStyle.Fill
        |> TabControl.withTabs [ controllerTab; libraryTab; testTab ]
        |> withGridRow 1

    do this.InitializeComponent()
    
    member private this.InitializeComponent() =
        AvaloniaXamlLoader.Load(this)

        this.Content <- layout |> withChildren [ mainTabControl ]
