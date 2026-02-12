# DualSense Battery Monitor - Low battery warning overlay for PS5 controllers

![Screenshot of the DualSense battery monitor on a default Windows 11 desktop](https://github.com/PixelIndieDev/DualSenseBatteryMonitor/blob/main/readme_images/overlay_preview.png)
![Screenshot of the DualSense battery monitor with settings opened on a default Windows 11 desktop](https://github.com/PixelIndieDev/DualSenseBatteryMonitor/blob/main/readme_images/overlay_preview_with_settings.png)
<sub>In both previews, the overlay is applied to a default Windows 11 desktop for a cleaner preview of the application</sub>

## Never miss a low battery warning again
**DualSense Battery Monitor** is a lightweight Windows utility that automatically alerts you with a **visual overlay** whenever any connected **PlayStation 5 DualSense controller** reaches a low battery level.

* Battery monitoring every 4 seconds
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

## Supports
* Sony PlayStation DualSense
* Sony PlayStation DualSense Edge

## Features
* **DualSense-aware overlay:** Shows connected PS5 controllers and their individual battery levels.
* **Low battery detection:** If any controller drops below 25%, the widget appears and below 15% the effected controller's battery icon will start flashing.
* **4-second refresh rate:** Battery status updates frequently and efficiently.
* **Invisible when not needed:** If no controller is low on battery, the widget remains hidden.
* **Button to view the battery levels:** If any controllers has the PS button down (for ~1-1.5 sec), the overlay will show for a couple of seconds.
* **Settings menu:** customize your user experience with just some clicks.
* **Tested with firmware A-0630** (latest as of December 5, 2025).
* **Framework-dependent and self-contained releases available.**
* **Built using .NET 8.0.**

## Getting Started
View [the wiki](https://github.com/PixelIndieDev/DualSenseBatteryMonitor/wiki) for instructions
↓
### Getting Started
* [Installation Guide](https://github.com/PixelIndieDev/DualSenseBatteryMonitor/wiki/Installation-Guide)
* [Configure Settings](https://github.com/PixelIndieDev/DualSenseBatteryMonitor/wiki/Configure-Settings)
* [Supported Controllers](https://github.com/PixelIndieDev/DualSenseBatteryMonitor/wiki/Supported-Controllers)

---

### Features & Usage
* [Understanding Icons & Statuses](https://github.com/PixelIndieDev/DualSenseBatteryMonitor/wiki/Understanding-Icons-%26-Statuses)
* [Button Mappings](https://github.com/PixelIndieDev/DualSenseBatteryMonitor/wiki/Button-Mappings)

---

### Troubleshooting
* [Error Codes](https://github.com/PixelIndieDev/DualSenseBatteryMonitor/wiki/Error-Codes)
* [Common Questions](https://github.com/PixelIndieDev/DualSenseBatteryMonitor/wiki/Common-Questions)

## Inspiration
This application is inspired by:
* [nondebug/dualsense](https://github.com/nondebug/dualsense) – DualSense explorer tool
* [filipmachalowski/TraySense](https://github.com/filipmachalowski/TraySense) – Tray app for battery status
* [filipmachalowski/TraySense](https://github.com/filipmachalowski/TraySense) - This project contains some code that is inspired by the code in the TraySense project

## This project uses
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
DualSense battery overlay, DualSense Edge battery overlay, PS5 controller low battery, DualSense WPF utility, DualSense Windows auto-start, gamepad battery warning, .NET 8 controller tool, DualSense battery level Windows 10/11, low battery widget for DualSense, DualSense charge monitor, DualSense Edge charge monitor, controller battery status desktop

---

<p align="center">
  <sub></sub>DualSense Battery Monitor is an independent project and is not affiliated with Sony Interactive Entertainment.</sub>
</p>
