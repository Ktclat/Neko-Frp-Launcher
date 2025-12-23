# 🐱 Neko Frp Launcher

![Platform](https://img.shields.io/badge/Platform-Windows-blue)
![Framework](https://img.shields.io/badge/Framework-.NET%208.0-purple)
![License](https://img.shields.io/badge/License-MIT-green)

**[English](#-english) | [简体中文](#-简体中文)**

---

## 🌏 English

**Neko Frp Launcher** is a lightweight, high-performance, and native GUI launcher for [frp](https://github.com/fatedier/frp) (Fast Reverse Proxy) on Windows.

Unlike other launchers based on Electron or Web technologies, Neko Frp Launcher is built with **native .NET 8 (C#)**, ensuring minimal resource usage, instant startup, and a clean user experience.

### ✨ Features

* **⚡ High Performance:** Native WinForms application, extremely low RAM (<20MB) and CPU usage.
* **🚀 Fast Config:** Quick setup for common gaming/service scenarios (TCP/UDP support).
* **🔒 Privacy First:** Smart password masking (shows `******`) to protect your server tokens during streaming or screenshots.
* **🛠️ Detailed Mode:** Full control over `frpc.toml` configuration for advanced users.
* **📦 Portable:** Single executable file, no installation required.
* **🎨 Modern UI:** Clean interface with system tray support.

### 📥 Installation & Usage

1.  **Download:** Go to the [Releases](../../releases) page and download the latest `NekoFrpLauncher.exe`.
2.  **FRP Core:** Download the Windows version of `frpc.exe` from the [official frp releases](https://github.com/fatedier/frp/releases).
3.  **Setup:** Put `NekoFrpLauncher.exe` and `frpc.exe` in the **same folder**.
4.  **Run:** Launch `NekoFrpLauncher.exe` and start configuring!

### 🔨 Build from Source

Requirements:
* Visual Studio 2022 (or newer)
* .NET 8.0 SDK

```bash
# Clone the repository
git clone [https://github.com/Ktclat/Neko-Frp-Launcher.git](https://github.com/Ktclat/Neko-Frp-Launcher.git)

# Navigate to the project directory
cd Neko-Frp-Launcher

# Build (Self-contained single file)
dotnet publish -c Release -r win-x64 --self-contained -p:PublishSingleFile=true
