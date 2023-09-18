![Demo4](https://github.com/scutrobotlab/SimulatorX/blob/docs/docs/.vuepress/public/static/images/demo-04.png?raw=true)

# SimulatorX [![License: GPL3.0](https://img.shields.io/badge/License-GPL3.0-yellow.svg)](https://opensource.org/license/gpl-3-0/) [![Unity Project](https://img.shields.io/badge/Engine-Unity-aqua.svg)](https://unity3d.com) [![Scutbot](https://img.shields.io/badge/因为我们是-虎-red.svg)](https://www.scutbot.cn)

这是华南理工大学华南虎战队 2023 赛季 SimulatorX 模拟器的开源技术报告。本文档主要对模拟器及其支持软件的基础信息进行分享。
# 详细的资料请移步官方开源网站： [simulatorx.org](https://simulatorx.org)
我们建议您先阅读以下内容，以便于更好地理解官方网站中的文档内容：
- [GitHub Wiki RM2022_SimulatiorX](https://github.com/scutrobotlab/RM2022_SimulatorX)
- [RM2022 华南理工大学 华南虎 RMUC 模拟器开源](https://bbs.robomaster.com/forum.php?mod=viewthread&tid=22206)

其他的相关资料：
- [SimulatorX 2023UC 发布会视频](https://www.bilibili.com/video/BV1FP41147NX/?share_source=copy_web&vd_source=4511493d03a23ab2d4ade49f2c3695a8)

## Powered By
![Unity](https://cdn-icons-png.flaticon.com/128/5969/5969347.png)

![Rider](https://resources.jetbrains.com/storage/products/rider/img/meta/rider_logo_300x300.png)![Blender](https://upload.wikimedia.org/wikipedia/commons/thumb/0/0c/Blender_logo_no_text.svg/300px-Blender_logo_no_text.svg.png)

## Dependencies | 项目依赖
- Unity 2020.3.22f1
- Blender 2.93
- Rider

## Platform | 平台
- Windows (服务端/客户端)
- Linux (服务端/客户端)
- Android (客户端)

## Dictionaries | 目录说明
- `Assets` Unity项目资源
- `Assets/Scripts` Unity项目脚本
- `Assets/Models` 项目模型
- `Assets/Scenes` 项目场景
- `Assets/Plugins` 项目插件
- `Assets/Fonts` 项目字体
- `Assets/Textures` 项目贴图
- `Assets/Resources/Language` 项目语言包

## Architecture | 原理架构说明

**基本原理**

![image](https://github.com/scutrobotlab/SimulatorX/assets/68735689/2825fe02-3f5f-4570-8764-7b4a8d4c5e9b)


**Flux：**
由于不好区分 Store 和 View，我们将这两个合到了一起，由 Dispatcher，Store，Action 三种成分构成。

Pros：
- 组件结构规范、耦合度低。
- Dispatcher 的存在方便实现录制、统计等功能。
- 应对规则修改，迭代快速

Cons：
- 需要同步修改 Action，InputActions，Receive 存在心智负担。
- 受 Flux 结构限制，事件传播只有单链，某些逻辑实现很绕。
- 运行在主线程上的 Dispatcher 和分帧发送可能形成性能瓶颈。
- 事件没有复用，无限 new 导致内存占用增长。
- 没有事件监控，传播路径和影响范围难以查明，Debug 困难。
- 跨端同步实现不够自然。
- 使用字符串做事件索引，存在心智负担、性能损耗。

**Mirror**
结合 Flux，两端之间通信的时机和内容都管理得比较好。

Pros:
- 组件丰富，又加上了 Smooth Sync，总的来说使用很舒适。
- 两端之间通信链路限死，再怎么 Bug 都是那一条，排查方便。
- 同时可以做服务端客户端，而且局域网公网可切换。
- 公网运行稳定，没有出过太多幺蛾子。

Cons:
- 从零基础到学习上手，有很多概念要掌握，包括共用一套代码。
- 今年全部移到服务端，搞得客户端都没有预测，同步会卡。
- 跨端同步强行弄了个 ChildAction 出来，能用但是很诡异。
- Mirror 自带的房间管理不算特别好用。
- 子弹等依然是各仿真各的，结果一致。

**今年改进：**
我们对网络同步逻辑进行了进一步优化，解决了部分前几个版本没能解决的遗留问题。
Flux与Mirror融合的架构
Flux的Action在服务器端流动，实现功能后同步至客户端进行效果展示。 
但是，在这种情况下，Action异常过多时会导致服务器端信息阻塞，从而影响所有游戏玩家的游戏体验。在2023版本中进行了一定的修正。

**仍然存在一些问题：**
比如机器人飞天、地震和谜之抖动，此类问题由于复现条件苛刻导致难以定位问题，一直没能完全修复是遗憾之一，希望在下个赛季的新版本可以修复，或借助于社区的力量。

**场地模型**

相较上版本使用更高清的纹理材质。
未来版本 将使用插件进行更高效，精细的建模，创建更精美的贴图以及使用高清渲染管线(HDRP)。

## RoadMap | 优化方向
![image](https://github.com/scutrobotlab/SimulatorX/assets/104719627/05826c7b-15b5-4294-8105-189625d976c3)

## Demo | 演示
![Demo](https://github.com/scutrobotlab/SimulatorX/blob/docs/docs/.vuepress/public/static/images/demo.png?raw=true)
![Demo1](https://github.com/scutrobotlab/SimulatorX/blob/docs/docs/.vuepress/public/static/images/demo-01.png?raw=true)
![Demo2](https://github.com/scutrobotlab/SimulatorX/blob/docs/docs/.vuepress/public/static/images/demo-02.png?raw=true)
![Demo3](https://github.com/scutrobotlab/SimulatorX/blob/docs/docs/.vuepress/public/static/images/demo-03.png?raw=true)

## Downloads
[![GitHub release (latest by date)](https://img.shields.io/github/v/release/scutrobotlab/SimulatorX)](https://github.com/scutrobotlab/SimulatorX/releases/latest)

## LICENSE
[![License: GPL3.0](https://img.shields.io/badge/License-GPL3.0-yellow.svg)](LICENSE.txt)

## Contribution
Issue first, then pull request. Welcome to join us!


<div class="half" style="text-align: center;">
    <img src="https://github.com/scutrobotlab/SimulatorX/blob/docs/docs/.vuepress/public/static/images/brand.png?raw=true" width="60%"/>
    <img src="https://github.com/scutrobotlab/SimulatorX/blob/docs/docs/.vuepress/public/static/images/icon.png?raw=true" width="20%"/>
</div>

