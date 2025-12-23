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

* **⚡ High Performance:** Native WinForms application, extremely low RAM usage (<20MB) and instant startup.
* **🚀 Fast Config:** Streamlined configuration panel designed for gaming and simple services (supports TCP & UDP).
* **🔒 Privacy First:** Smart password masking (always shows `******`) to protect your server tokens during streaming or screenshots.
* **🛠️ Detailed Mode:** Full control over `frpc.toml` configuration for advanced users.
* **📦 Portable:** Single executable file, no installation required.
* **🎨 Modern UI:** Clean interface with system tray support.

> [!IMPORTANT]
> **⚠️ Core Component Required **
> This software is a GUI Launcher. It does **NOT** include the `frpc.exe` core file.
> You **MUST** download `frpc_windows_amd64.zip` from the official repo and put `frpc.exe` in the same folder as this launcher.
>
> 🔗 **[Download frpc](https://github.com/fatedier/frp/releases)**

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
```
## 📜 License

* Licensed under the [MIT License](LICENSE).

## ❤️ Credits

* Core logic based on **[fatedier/frp](https://github.com/fatedier/frp)**.
* Thanks to the open source community.


## 🇨🇳 简体中文

**Neko Frp Launcher** 是一款专为 Windows 打造的轻量级、高性能、原生 FRP (Fast Reverse Proxy) 启动器。

与基于 Electron 或 Web 技术的启动器不同，本项目完全基于 **原生 .NET 8 (C#)** 开发，旨在提供极致的性能、超低的内存占用以及纯净的用户体验。

### ✨ 核心特性

* **⚡ 极致轻量:** 原生 WinForms 开发，内存占用极低（通常小于 20MB），秒启动。
* **🚀 快速配置:** 专为联机游戏/简单服务设计的快速配置面板（支持 TCP/UDP）。
* **🔒 隐私保护:** 智能密码掩码功能（统一显示 `******`），直播或截图时无需担心 Token 泄露。
* **🛠️ 详细模式:** 支持完整编辑 `frpc.toml`，满足高级用户的复杂需求。
* **📦 绿色便携:** 单文件运行，无需安装，解压即用。
* **🎨 现代设计:** 清爽的界面布局，支持最小化到系统托盘后台运行。

> [!IMPORTANT]
> **核心组件缺失说明**
>
> 本软件仅为启动器，**不包含** `frpc.exe` 核心文件。
> 请务必前往官方仓库下载 `frpc_windows_amd64.zip`，解压并将 `frpc.exe` 与本软件放在 **同一目录** 下。
>
> 🔗 **[下载 frpc 核心](https://github.com/fatedier/frp/releases)**

### 📥 下载与使用

1.  **下载软件:** 前往 [Releases](../../releases) 页面下载最新的 `NekoFrpLauncher.exe`。
2.  **下载核心:** 前往 [frp 官方仓库](https://github.com/fatedier/frp/releases) 下载 Windows 版本的 `frpc.exe`。
3.  **安装:** 将 `NekoFrpLauncher.exe` 和 `frpc.exe` 放在 **同一个文件夹** 内。
4.  **运行:** 双击 `NekoFrpLauncher.exe` 即可开始使用！

### 🔨 源码编译

编译环境要求：
* Visual Studio 2022 (或更新版本)
* .NET 8.0 SDK

```bash
# 克隆仓库
git clone [https://github.com/Ktclat/Neko-Frp-Launcher.git](https://github.com/Ktclat/Neko-Frp-Launcher.git)

# 进入目录
cd Neko-Frp-Launcher

# 编译发布 (生成独立单文件)
dotnet publish -c Release -r win-x64 --self-contained -p:PublishSingleFile=true
```

## 📜协议
* 本项目遵循 MIT 开源协议。

## ❤️致谢

* 核心基于 **[fatedier/frp](https://github.com/fatedier/frp)**。
* 感谢开源社区。
