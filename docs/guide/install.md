---
lang: zh-CN
title: 安装SimulatorX
description: 如何安装SimulatorX开源版或社区版
---

:::info

开源版目前为2023赛季的规则实现，如果非必要，我们更推荐您按照下面的教程安装`SimulatorX官服版`。  
安装`SimulatorX开源版`请参考[SimulatoX开源版](#安装开源版)

:::

## 安装SimulatorX官服版

1. 访问[官方下载站](#https://dl.sim.scutbot.cn/)

2. 选择你需要的版本

3. 下载SimualtorX压缩包（即普通客户端），解压后运行`SimulatorX.exe`

:::info

带有`LinuxServer`字样的版本为服务器版，可以用于搭建[**私有服务器**](#搭建私有服务器)

:::

## 官服版如何联机

:::info

客户端与服务器：想要进行对战，我们电脑上下载和使用的是客户端，客户端需要连接上**服务器端**才能创建房间、加入房间，服务器端需要**部署在机器上**才能被客户端连接。

软件开发组提供了**公开的服务器，供所有用户联机**

:::

### (最简便的方式) 使用华南虎官方服务器

- 下载SimulatorX，解压后运行`SimulatorX.exe`
- 在【创建房间】页面或者【主页面】中填写用户名，直接点击【创建房间】即可创建房间并成为房主，无需填写服务器地址与密码
- 从房间左上角可以复制口令并分享给其他用户，其他用户在【加入已有房间】页面或者【主页面】中填写用户名和房主的口令即可点击【加入房间】按钮

恭喜，至此即可愉快地进行联机游戏。

### 搭建私有服务器

若认为软件开发组提供的服务器联机特别卡顿，可以选择**自己部署服务器用于联机：**

1. 首先仍然是下载上文中[官方下载站](#https://dl.sim.scutbot.cn/)中的SimualtorX压缩包（即普通客户端）以及LinuxServer（Linux服务器端）
2. 然后需要部署Linux私有服务器，详细的私有服务器申请与部署流程参见[私有服务器](https://scutrobotlab.feishu.cn/wiki/wikcnAK9qq0uYZyLWi8xnJi6Q6b?from=from_copylink)文档,申请后可以获取密钥
3. 对于客户端，在【创建房间】页面或者【主页面】中填写用户名和在【服务器密码】一栏填写申请的密钥，点击【创建房间】即可创建房间并成为房主
4. 从房间左上角可以复制口令并分享给其他用户，其他用户填写用户名和口令连接（同方法1）

## 安装开源版
### 客户端软件

- 首先下载客户端软件 [SimulatorX Windows Client](https://github.com/scutrobotlab/SimulatorX/releases/download/1.2.3.1-release/SimulatorX-Windows-Client-1.2.3.1-Release.zip)
- 解压安装
- 运行 `SimulatorX.exe` 

> 或许你还想看：[如何多人联机](game.md#联机方式)

### 服务端软件
- 下载服务端软件:
- linux: [simulatorx-linux-server.tar.gz](https://github.com/scutrobotlab/SimulatorX/releases/download/1.2.3.1-release/simulatorx-linux-server-1.2.3.1-release.tar.gz)
- Windows: [SimulatorX Windows Server](https://github.com/scutrobotlab/SimulatorX/releases/download/1.2.3.1-release/SimulatorX-Windows-Server-1.2.3.1-Release.zip)

- 解压安装
- 运行 `simulatorx-server.exe`

命令行参数:
| 参数 | 说明 |
| --- | --- |
| `-p` | 指定端口 |
| `-n` | 指定服务器名称 |
| `-e` | 指定连接IP （私服版） |
| `-P` | 指定连接密码（私服版） |
| `-t` | 指定启动密钥（私服版） |

::: info Best Practice
```bash
# 开源版服务器
simulatorx-server -p 5333 -n "华南猫"

# 官服版私有服务器
simulatorx-server -p 5333 -n "华南猫-02" -P "喵呜" -e scutrobot.moe -t simulatorx-s-12345678
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


