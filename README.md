# BluetoothBatteryViewer

一个轻量级 Windows 桌面托盘工具，自动扫描已连接蓝牙设备的电池电量，支持热键语音播报、屏幕阅读器 集成和开机自启。

## 功能

- **蓝牙电量扫描** — 通过 Windows BLE API 自动发现已连接设备并读取电量百分比
- **热键触发** — 可自定义全局热键，一键播报所有设备电量
- **语音播报** — 支持 Tolk 读屏器（NVDA / JAWS / Dolphin 等），无读屏时自动回退到 SAPI5 TTS
- **开机自起** — 可选开机自动运行，注册表写入 HKCU Run 键
- **启动提示音** — 支持自定义 start.wav 启动音效
- **单实例** — 全局命名 Mutex 防止重复启动

## 系统要求

- Windows 10（版本 1803 及以上）或 Windows 11
- .NET Framework 4.8（Windows 10 1903+ 内置）
- 蓝牙适配器及支持电池服务的蓝牙外设


## 依赖项
- System.Text.Json： JSON 配置文件读写
- System.Speech： SAPI5 TTS 回退引擎
- Tolk： 屏幕阅读器集成库
- zdsrapi： 争度读屏 API
- *注：调用 Windows BLE API 依赖了 Windows 10 SDK 契约 (Contracts)。*

## 技术架构

```
BluetoothBatteryViewer/
├── Models/          # 数据模型
├── Services/        # 业务逻辑
├── UI/              # WinForms 界面
├── Interop/         # P/Invoke 互操作
├── TrayApplicationContext.cs  # 托盘与应用生命周期
└── Program.cs                  # 入口，单实例 Mutex
```

基于 .NET Framework 4.8 + Windows Forms，通过 WinRT API 与蓝牙 LE 设备通信。

## 许可

本项目基于 [MIT License](https://opensource.org/licenses/MIT) 开源。你可以自由地使用、修改和分发。
