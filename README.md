# USB Scale Counter

Count identical items quickly and accurately using a Dymo M10 USB postal scale —
on **Windows** and **Android**.

## Features

- **Count by weight** — the app reports how many pieces are on the scale, with
  color-coded feedback (orange / green / red).
- **Presets** — save a calibration per item type and switch between them.
- **Multi-quantity calibration** — capture a few known quantities (and an optional
  empty container for tare); more samples give a more accurate count.
- **Import / export** — share calibrations as `.uscpreset` files. Double-clicking a
  file loads it on Windows; on Android it imports directly.
- **Bundled default presets**, with one-click "Load defaults".
- **Multi-count (Windows)** — watch several presets at once in a dockable panel that
  can be popped out to its own window.
- **Optional sound signals** — a chime when the target is reached, an error tone when
  there are too many, and a warning when a good count drops back below target.

## Documentation

Check here for documentation: https://opensource.duma.sh/apps/usb-scale-counter

## Installation

- **Windows (installer):** install via ClickOnce —
  https://kduma-oss.github.io/CS-usb-scale-counter/USBScaleCounter.application
- **Windows (portable):** download `usb-scale-counter-desktop-vX.Y.Z.zip` from the
  latest [release](https://github.com/kduma-OSS/CS-usb-scale-counter/releases), unzip
  and run `USBScaleCounter.exe`.
- **Android:** download `usb-scale-counter-vX.Y.Z.apk` from the latest release and
  install it. Connect the scale with a USB-OTG adapter; when Android asks, allow the
  app to open for the attached device and it connects automatically.

## Screenshots

| Disconnected | Empty Scale | Not Enough Items | Exact Count | Too Many Items |
|:---:|:---:|:---:|:---:|:---:|
| ![Disconnected](assets/1_disconnected.png) | ![Empty Scale](assets/2_empty_scale.png) | ![Not Enough](assets/3_less_than_required.png) | ![Exact Count](assets/4_exact_count.png) | ![Too Many](assets/5_too_many.png) |

## How It Works

1. Calibrate a **preset**: put a few known quantities on the scale (optionally an
   empty container first, for tare) and capture each measurement. Give the preset a
   target quantity and save it. Measuring several quantities makes the count more
   accurate.
2. Select the preset and place items on the scale.
3. The app counts by weight and gives color-coded feedback:
   - **Orange** — fewer items than the target
   - **Green** — target reached
   - **Red** — too many items
4. With sound enabled, once the reading settles you get an audible signal: success on
   reaching the target, error when there are too many, and a warning when a previously
   good/over count falls back below target.

Presets are saved, so you can keep a calibration per item type and re-calibrate
whenever a batch differs.

## Requirements

- Dymo M10 USB postal scale
- Windows, or Android 5.0+ with USB-OTG support
