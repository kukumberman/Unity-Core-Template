# Changelog

## 2023-09-21 - Final support for UI Toolkit huds
Refactoring existing support of UI Toolkit in project for `HudManager`.

Now you don't need to create separate `Mediator` for **UI Toolkit**, only hud implementation can vary (either **UGUI** or **UI Toolkit**).

## 2023-09-13 - Script templates
Script templates to quickly define structure of the **Model-Hud-Mediator** classes.

Looks like **Unity Engine** can't recognize this files in package folder, so new feature via `[MenuItem]` was introduced.

## 2023-08-10 - Save system on WebGL platform
New implementation of the `ISaveSystem` to store data in `localStorage`.

## 2023-08-07 - Better injection technique
Specify `MonoBehaviour` and `ScriptableObject` instances to be auto-injected at the startup.

Introduced `[Inject(typeof(ISomeService))]` attribute for polymorphic classes.

## 2023-07-19 - Hud orientation
Allows to create multiple UI layouts for different screen orientations.

## 2023-07-14 - Secured save data
New feature that allows to save data using secured format (encryption and decryption) instead of plain text.

Introduced `ISaveEncoderDecoder` contract.

## 2023-07-08 - Project as custom package
I have used this package in 3+ projects by that time, it was very annoying to copy-paste files, so I decided to make custom package and install it via Package Manager.

## 2023-04-20 - Editor utility to show local save file
`MenuItem` helper feature to quickly navigate to local save file.

## 2023-04-20 - Experimenal support for UI Toolkit huds
`HudManager` is able to use **UI Toolkit** features. 

## 2023-04-20 - Injection by name
New feature that allows to inject objects by unique name, main purpose - store multiple instances of the same type.

## 2023-03-20 - Sandbox project
Making simple playground with demonstration purposes.

## 2023-03-19 - Refactoring core
Making some types to be independent on project structure.

## 2022-09-19 - Initial
Initial state of the project at that time.
