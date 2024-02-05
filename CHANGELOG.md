# Changelog

All notable changes to this project will be documented in this file.

## [3.1.2] - 2024-02-05
- Added script *DisableOtherCameras.cs* to deactivate all Cameras except the Rig Camera on scene load.

## [3.1.0] - 2023-10-16
- Added dynamic handmenu panels which can be placed in a scene and get loaded when the scene is entered and unloaded when leaving the scene
- Minor restructure of mainmenu layout
- Minor bugfix regarding the start state of the gaze interactor
- Added Quick Outline to the package
- Added validation check for internet settings
- NOTE: No validation check can currently be made for the Meta Quest 'Force Remove Internet' setting
- Added toggle to handmenucontroller to enable opening the panel that was open last when reopening the handmenu.

## [3.0.0] - 2023-10-12
- Major rework of the package
- Implemented project validation which informs the user if all required packages and samples are in the project
- Created new VR Rig based on the latest features from the XR Interaction Toolkit
- Gaze interaction is now available, which can be toggled in the settings menu
- Added new handmenu logic with a wrist button
- Created new InputAction set which only contains the custom actions
- Replaced teleporting behaviour with the XRITK solution
- Removed obsolete code, files and prefabs

## [2.0.9] - 2023-10-05
- Implemented Gaze Interactor and Gaze Event components

## [2.0.8] - 2023-09-04
- Implemented complete handmenu to the new rig
- fixed missing script errors in the old rig

## [2.0.7] - 2023-09-01
- Added new rig which is based on the latest example from the XRITK
- Added empty handmenu to the new rig

## [2.0.6] - 2023-08-30
- Added new component for sending events on hovering over UI Elements.

## [2.0.5] - 2023-08-29
- Updated required versions of dependencies
- Added flag to toggle the Menu on Start & Scene change on MenuController.cs

## [2.0.4] - 2023-07-05
- HMD tracking bindings are now referencing the CPVR Inputaction bindings.
- Added Invisible Panel around UI

## [2.0.3] - 2023-05-31
- emailaddress will now be correctly shown if the field in the inspector has been filled out.

## [2.0.2] - 2023-05-24
- Added possibility to adjust the angle of the teleport ray

## [2.0.1] - 2023-05-24
- Changed menu state after scene change
- Button referencing the current scene is now non interactable

## [2.0.0] - 2023-05-22
- Reworked the teleport interaction
- Seperated the teleport interaction from the remaining logic
- Input interactions are now properly seperated
- Removed PlayerBehaviour.cs and RegisterHandToXrOrigin.cs
- Update requires replacing the PersistentObjects prefab

## [1.1.1] - 2023-05-23
- Fixed menu opening and closing instantly.

## [1.1.0] - 2023-05-22
- Reworked menu interaction logic
- Opening the menu is now controlled by the system gesture on the left hand (aim flags)
- While the menu is open, teleportation is disabled. This is controlled by UnityEvents on the MenuController.cs script
- Refactored PlayerBehaviour.cs

## [1.0.9] - 2023-05-22
- Updated hand model (taken from XR Hands Demo)
- Created gameobjects for the hands (contains the XRITK components)
- Removed old hand prefabs and models

## [1.0.8] - 2023-05-20
- Added this changelog file
- Added pinchOffset to Teleporting.cs

## [1.0.7] - 2023-05-16
- Reduced unintended teleporting and fixed bug with screenshotbutton
- Move from private GitLab to public GitHub
- Scene load, Quest Input field with AndroidManifest.xml
