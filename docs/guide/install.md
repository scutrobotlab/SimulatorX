# 安装 SimulatorX

## 客户端软件
- 首先下载客户端软件 [SimulatorX Windows Client](https://github.com/scutrobotlab/SimulatorX/releases/download/1.2.3.1-release/SimulatorX-Windows-Client-1.2.3.1-Release.zip)
- 解压安装
- 运行 `SimulatorX.exe` 

> 或许你还想看：[如何多人联机](game.md#联机方式)

## 服务端软件
- 下载服务端软件 [simulatorx-linux-server.tar.gz](https://github.com/scutrobotlab/SimulatorX/releases/download/1.2.3.1-release/simulatorx-linux-server-1.2.3.1-release.tar.gz)
::: tip 对于 Windows 服务端，应当下载 [SimulatorX Windows Server]
:::
- 解压安装
- 运行 `simulatorx-server.exe`

命令行参数:
| 参数 | 说明 |
| --- | --- |
| -p | 指定端口 |
| -e | 指定外网IP（弃用） |
| -n | 指定服务器名称 |

::: info Best Practice
```bash
simulatorx-server -p 5333 -n "华南猫"
```
:::

## 掌上SimulatorX
- 下载安卓便携版 [SimulatorX Android Portable](https://github.com/scutrobotlab/SimulatorX/releases/download/1.2.3.1-release/SimulatorX-1.2.3.1-Release.apk)
- 安装运行

::: info 安卓便携版是图一乐，不保证稳定性和可用性
Known Issues: 
- 安卓版无法联机
- 安卓版仍需要外接键盘

欢迎提交PR完善 `掌上SimulatorX`
:::

## 开发者建议
- 请确保服务端IP地址对客户端联通
- 可尝试使用持久化技术运行服务端
- 可尝试内网穿透技术对特定网络环境暴露服务端。但注意，此操作造成的后果由操作者自行承担。
