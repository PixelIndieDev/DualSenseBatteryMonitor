# DualSense Battery Monitor - Low battery warning overlay for PS5 controllers

![Screenshot of the DualSense battery monitor on a default Windows 11 desktop](https://github.com/PixelIndieDev/DualSenseBatteryMonitor/blob/main/readme_images/overlay_preview.avif?raw=true)
<sub>Overlay is slightly oversized on image</sub>

## Never miss a low battery warning again
**DualSense Battery Monitor** is a lightweight Windows utility that automatically alerts you with a **visual overlay** whenever any connected **PlayStation 5 DualSense controller** reaches a low battery level.

* Auto-starts with Windows
* Battery monitoring every 5 seconds
* Only shows when battery is low, or when the user holds down the PS button
* Silent

> [!NOTE]
> This only works on Windows systems

## Use Case
This utility is perfect for:
* Users who use PS5 controllers on PC and want a non-intrusive, visual heads-up for low battery levels
* Multiplayer users with multiple controllers connected — the app tracks and displays each one with its own icon and battery status (Up to 4 DualSense controllers)
* Users using the DualSense controller as a pointer/input device

The app acts **only when needed**, showing a overlay with controller status **only when a low battery is detected**, or **when the user holds down the PS button**.

## Features
* **DualSense-aware overlay:** Shows connected PS5 controllers and their individual battery levels.
* **Low battery detection:** If any controller drops below 25%, the widget appears and below 15% the effected controller's battery icon will start flashing.
* **5-second refresh rate:** Battery status updates frequently and efficiently.
* **Invisible when not needed:** If no controller is low on battery, the widget remains hidden.
* **Button to view the battery levels:** If any controllers has the PS button down (for ~1-1.5 sec), the overlay will show for a couple of seconds.
* **Tested with firmware A-0630** (latest as of December 5, 2025).
* **Framework-dependent and self-contained releases available.**
* **Built using .NET 8.0.**

## Installation & Download
You can download the latest release [here](https://github.com/PixelIndieDev/DualSenseBatteryMonitor/releases).

Two versions are provided:
* **Framework-dependent** (.NET 8.0 required on system)
* **Self-contained** (no dependencies — just run the executable with its files)

### Make it start up on Windows startup
1. Launch the application
2. Right click the icon of the application in your system bar
3. Click on start on startup
   * If the option is checked, then it will start up on Windows startup

## Uninstallation
To completely remove **DualSense Battery Monitor** from your system:

**Make sure the application is running**
1. Right click the icon of the application in your system bar
2. Click on start on startup
   * If the option is checked, then it will start up on Windows startup. When uninstalling, you want this unchecked
3. Click on exit
2. Delete the application files: remove the folder/insides of folder where you extracted the application.

## Common issues
### Holding the PS button (Home button) for ~1-1.5 sec keeps opening Steam Big Picture Mode/Steam overlay
Go to Steam->Settings->Controller and disable "Guide button focuses Steam" and "Enable guide button chords for controller", then restart Steam (Fully exit Steam by going to Steam->Exit, and then start Steam again)

## Inspiration
This application is inspired by:
* [nondebug/dualsense](https://github.com/nondebug/dualsense) – DualSense explorer tool
* [filipmachalowski/TraySense](https://github.com/filipmachalowski/TraySense) – Tray app for battery status

## This project uses
[**filipmachalowski/TraySense**](https://github.com/filipmachalowski/TraySense)
* This project uses some parts of the code (like the ProcessControllerData function) for its functioning

[**HIDSharp**](http://www.zer7.com/software/hidsharp)
* Copyright 2010-2019 James F. Bellinger
* Licensed under the [Apache License, Version 2.0](https://www.apache.org/licenses/LICENSE-2.0.txt)

[**DualSense SVG Vector**](https://www.svgrepo.com/svg/324525/dualsense)
* Copyright Alex Martynov
* Licensed under the [CC Attribution License](https://creativecommons.org/licenses/by/4.0/deed.en)
* The DualSense Edge version of the DualSense SVG is a edited version of the original

## 📃 License
MIT License. See [LICENSE](LICENSE) file for details.

## 🔍 Keywords
DualSense battery overlay, PS5 controller low battery, DualSense WPF utility, DualSense Windows auto-start, gamepad battery warning, .NET 8 controller tool, DualSense battery level Windows 10/11, low battery widget for DualSense, DualSense charge monitor, controller battery status desktop
