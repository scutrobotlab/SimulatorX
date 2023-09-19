# 工程结构
代码工程结构介绍。

## 代码分布
SimulatorX 模拟器软件的主要实现代码分布在四个命名空间下：

![image](https://github.com/scutrobotlab/SimulatorX/assets/104719627/13302b0c-43c9-4a6e-8810-fd745aa593f5)

### Controllers
存放不同物体的控制代码。大多数物体都作为 Flux 架构中生产、消费事件的 Store 存在。

![boxcnRBNNCaaRgYl27ulNOBaUvg](https://github.com/scutrobotlab/SimulatorX/assets/104719627/29d0d2c3-64e2-4f40-a90f-3ddc67b75cbd)

![boxcnTWzI66TgyxaPTpkIRU1Jeo](https://github.com/scutrobotlab/SimulatorX/assets/104719627/0608e1d0-47fc-4aeb-a914-7e68acec1523)

（.etc)
### GamePlay
赛规、裁判系统相关代码。包括等级属性自定义、场地增益等系统。Flux 架构中流动的 Action 也都在这里声明。

![boxcnuwyXXDGBSQyTahdXncV6Tb](https://github.com/scutrobotlab/SimulatorX/assets/104719627/e88601de-2353-41c4-8c51-1b078b590869)

![boxcnMlWCtN2BKAhhjfVfWDytqf](https://github.com/scutrobotlab/SimulatorX/assets/104719627/e1c024ec-b5f4-49cf-9285-3b05e9d85174)

（.etc)

### Infrastructure
Flux 架构，输入系统等基础设施。

![boxcn4sEhS2hEAvH8CMa1JhQrGc](https://github.com/scutrobotlab/SimulatorX/assets/104719627/fd54e259-b52f-4ac8-869f-5cd366f99d5b)

（.etc)
### UI
各种用户界面脚本。

![boxcnDDoNrnHzpFPVJvKNiSMTbb](https://github.com/scutrobotlab/SimulatorX/assets/104719627/60bb5020-6501-4227-9c07-461e9d24a4f9)

（.etc)

PS：在 SimulatorX 开发过程中，在每个类型、方法、公有成员之前，都会按正确格式书写文档注释，以便后续维护与协作。

![boxcnIGJPRTFx3Px7o12QZKcn3H](https://github.com/scutrobotlab/SimulatorX/assets/104719627/66cf1019-2fa6-4192-b650-2cb79cd7ac55)


