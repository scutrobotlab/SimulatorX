# 网络同步
::: tip 在网络同步方案上，与2022版本相同，我们仍然使用Mirror网络库。除了基本的位置、姿态同步外，为了让跨端同步与 Flux 架构相容，我们对 Flux 架构进行了一些延伸。
:::

## Flux与Mirror融合的架构
Flux的Action在服务器端流动，实现功能后同步至客户端进行效果展示。

## Flux
在旧版模拟器 Simulator 的研发后期，由于代码耦合严重、功能划分模糊，难以加入更多新功能，甚至妨碍了已有 Bug 的修复。为了解决这一问题，SimulatorX 将前端开发中常用的 Flux 架构引入到游戏开发中，以求提高系统的可维护性和可拓展性。
![image](https://github.com/scutrobotlab/SimulatorX/assets/104719627/ab2b1ae6-9f86-41bc-8f1f-df17fcea2fd2)
由于不好区分 Store 和 View，我们将这两个合到了一起，由 Dispatcher，Store，Action 三种成分构成。

#### Pros：
- 组件结构规范、耦合度低。
- Dispatcher 的存在方便实现录制、统计等功能。
- 应对规则修改，迭代快速

#### Cons：
- 需要同步修改 Action，InputActions，Receive 存在负担。
- 受 Flux 结构限制，事件传播只有单链，某些逻辑实现很绕。
- 运行在主线程上的 Dispatcher 和分帧发送可能形成性能瓶颈。
- 事件没有复用，无限 new 导致内存占用增长。
- 没有事件监控，传播路径和影响范围难以查明，Debug 困难。
- 跨端同步实现不够自然。
- 使用字符串做事件索引，存在心智负担、性能损耗。

### Flux架构下工作流的例子：
![image](https://github.com/scutrobotlab/SimulatorX/assets/104719627/756e2b4b-3fb7-41f2-9960-72dea247610f)

## Mirror：
Mirror 是一个为 Unity 2019 LTS 及以上版本设计的网络同步框架，隐藏了不同底层传输协议的复杂性。其封装简单明了，能够有效提高开发效率。此框架之所以得名“Mirror”，是因为客户端与服务端软件实际上共用一套代码，不需要维护另外的服务器软件。结合 Flux，两端之间通信的时机和内容都管理得比较好。

#### Pros:
- 组件丰富，又加上了 Smooth Sync，总的来说使用很舒适。
- 两端之间通信链路限死，再怎么 Bug 都是那一条，排查方便。
- 同时可以做服务端客户端，而且局域网公网可切换。
- 公网运行稳定，没有出过太多幺蛾子。

#### Cons:
- 从零基础到学习上手，有很多概念要掌握，包括共用一套代码。
- 今年全部移到服务端，搞得客户端都没有预测，同步会卡。
- 跨端同步强行弄了个 ChildAction 出来，能用但是很诡异。
- Mirror 自带的房间管理不算特别好用。
- 子弹等依然是各仿真各的，结果一致。

### 改造Flux
除了基本的位置、姿态同步外，为了让跨端同步与 Flux 架构相容，我们对 Flux 架构进行了一些延伸。
Flux 架构中的 Action 都在服务端流动，为了在保持设计一致性的同时，方便服务端实体控制客户端的视觉效果（车轮旋转、灯条闪烁等），我们从 StoreBase 类比出了 StoreChildBase 类。Child 类型可以与 Store 类组成层级关系。举个例子，步兵机器人由 InfantryStore 控制，他的四块装甲板继承于 StoreChildBase。当装甲板受到击打时，会发送相应 Action 通知 InfantryStore；而当 InfantryStore 判断需要启用、停用装甲板时，会发送 ChildAction 通知其子级装甲板。
```
  /// <summary>
  /// 装甲板控制器。
  /// </summary>
  public class Armor : StoreChildBase
  {
      public Identity.Camps camp;
      public bool defaultOn;
      public char text;
      ...
      /// <summary>
      /// 处理事件。
      /// </summary>
      /// <param name="action"></param>
      public override void Receive(IChildAction action)
      {
          base.Receive(action);
          switch (action.ActionName())
          {
              case ChildActionID.Armor.SyncArmor:
                  var syncAction = (SyncArmor) action;
                  camp = syncAction.Camp;
                  text = syncAction.Text;
                  RegisterChild(transform);
                  break;
              
          }
      }
  }
  ```
这样设计的好处是，步兵无需关心自己安装了几块装甲板，甚至没有装甲板也没关系。组件之间可以解耦，任意组合。

Action 只在服务端流动，推动游戏逻辑运行；ChildAction 则会被跨端同步，将视觉效果信息同步到每一个客户端。为了在同步的同时保留类型信息，需要用到多态序列化、反序列化，（如果使用 Protobuf 就需要写 DSL 了，msgpack 似乎是更合理的选择，但最后还是造了轮子。
```
// 别问，问就是轮子。
public static class PolymorphicSerializer
    {
        public static string Serialize(object value)
        {
            ...
```

### 改进
#### 节省带宽
由于每一帧客户端都需要上报输入状态，我们简单压缩了一下输入信息：
```
// 在客户端打包输入数据，发送到服务端。
var primaryAxis = BitUtil.CompressAxisInput(InputManager.Instance().primaryAxis);
var secondaryAxis = BitUtil.CompressAxisInput(InputManager.Instance().secondaryAxis);
var input = new BitMap32();
input.SetByte(2, primaryAxis);
input.SetByte(3, secondaryAxis);
input.SetBit(2, InputManager.Instance().ButtonStatus[InputActionID.FunctionA]);
input.SetBit(3, InputManager.Instance().ButtonStatus[InputActionID.FunctionB]);

```
研判了一下所需精度后，我们将部分二维杆量输入压缩到了一个字节，将普通按键输入存储为 bit ，最后每一帧的所有输入信息可以被压缩到四个字节大小。
#### 事件逻辑优化
在今年，我们对网络同步逻辑进行了进一步优化，解决了部分前几个版本没能解决的遗留问题。
ChildAction 在同步过程中会产生大量的冗余信息。例如，装甲板每一帧都会收到状态更新信息，一个只是表示点亮状态的信息每帧都被同步，导致占用大量带宽。在发送 ChildAction 之前进行简单的去重就可以降低服务器的带宽压力。在实际测试中证明，仅靠去重还是不够，需要更改事件发送的逻辑进一步从源头上减少事件数量。

:::warning 注意
Action异常过多时会导致服务器端信息阻塞，从而影响所有游戏玩家的游戏体验。在2023版本中进行了一定的修正。
:::
