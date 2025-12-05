# DualSense Battery Monitor - Low Battery Warning Overlay for PS5 Controllers

![Screenshot of the DualSense battery monitor on a default Windows 11 desktop](https://github.com/PixelIndieDev/DualSenseBatteryMonitor/blob/main/readme_images/overlay_preview.avif?raw=true)
<sub>Overlay is slightly oversized on image</sub>

## Never Miss a Low Battery Warning Again
**DualSense Battery Monitor** is a lightweight Windows utility that automatically alerts you with a **visual overlay** whenever any connected **PlayStation 5 DualSense controller** reaches a low battery level.

* Auto-starts with Windows
* Battery monitoring every 5 seconds
* Only shows when battery is low
* Silent

> [!NOTE]
> This only works on Windows systems

## Use Case
This utility is perfect for:
* Users who use PS5 controllers on PC and want a non-intrusive, visual heads-up for low battery levels
* Multiplayer users with multiple controllers connected — the app tracks and displays each one with its own icon and battery status (Up to 4 DualSense controllers)
* Users using the DualSense controller as a pointer/input device

The app acts **only when needed**, showing a overlay with controller status **only when a low battery is detected**.

## Features
* **DualSense-aware overlay:** Shows connected PS5 controllers and their individual battery levels.
* **Low battery detection:** If any controller drops below 25%, the widget appears and below 15% the effected controller's battery icon will start flashing.
* **5-second refresh rate:** Battery status updates frequently and efficiently.
* **Automatic Windows startup:** Uses registry-based startup (no Task Scheduler required).
* **Invisible when not needed:** If no controller is low on battery, the widget remains hidden.
* **Tested with firmware A-0520** (latest as of June 11, 2025).
* **Framework-dependent and self-contained releases available.**
* **Built using .NET 8.0.**

## Installation & Download
You can download the latest release [here](https://github.com/PixelIndieDev/DualSenseBatteryMonitor/releases).

Two versions are provided:
* **Framework-dependent** (.NET 8.0 required on system)
* **Self-contained** (no dependencies — just run the executable with its files)

No manual configuration is needed. On first launch:
* The app silently registers itself to **auto-start** with Windows.
* If **no controller is connected or has low battery**, the app hides itself automatically.

## Uninstallation
To completely remove **DualSense Battery Monitor** from your system:

1. **Stop the application from currently running:**

    Find the DualSenseBatteryMonitor.exe process in Task Manager and 'End Task' it.
2. **Delete the application files:**

    Remove the folder/insides of folder where you extracted the application.

> [!WARNING]
> **(Optional) (Advanced users only)**
> **Remove the auto-start registry entry:**
>
> 1. Open regedit and navigate to 'HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Run'
> 
> 2. Find the entry related to DualSenseBatteryMonitor and delete it to stop the app from launching automatically at startup.

## Inspiration
This application is inspired by:
* [nondebug/dualsense](https://github.com/nondebug/dualsense) – DualSense explorer tool
* [filipmachalowski/TraySense](https://github.com/filipmachalowski/TraySense) – Tray app for battery status

## This project uses
[**filipmachalowski/TraySense**](https://github.com/filipmachalowski/TraySense)
* This project uses some parts of the code (like the ProcessControllerData function) for its functioning

[**HIDSharp**](http://www.zer7.com/software/hidsharp)
* Copyright 2010-2019 James F. Bellinger
* Licensed under the Apache License, Version 2.0

## 📃 License
MIT License. See [LICENSE](LICENSE) file for details.

## 🔍 Keywords
DualSense battery overlay, PS5 controller low battery, DualSense WPF utility, DualSense Windows auto-start, gamepad battery warning, .NET 8 controller tool, DualSense battery level Windows 10/11, low battery widget for DualSense, DualSense charge monitor, controller battery status desktop
