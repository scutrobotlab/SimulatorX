# 输入处理
在旧版模拟器中，获取输入的逻辑都在车辆控制器中实现，导致了难以对其统一管理、处理逻辑重复等问题。在 SimulatorX 中，我们设计了输入管理器，用于统一接收、映射输入事件，并将输入信息以 Action 的形式流入 Flux 系统，发送到需要的机器人处。
输入处理分为几个部分：功能定义、输入映射、杆量映射、延迟模拟。
## 功能定义
SimulatorX 中的输入事件按功能划分，不同的机器人 Store 会选择需要的输入功能进行响应。
```
/// <summary>
/// <c>InputActionID</c> 是 InputSystem 中所注册动作的对应名称。
/// <br/>目前包括主辅轴、视角控制（单独处理）、主副开火、A~J六个功能键。
/// </summary>
public static class InputActionID
{
    // Vector2 Axis
    public const string PrimaryAxis = "PrimaryAxis";
    public const string SecondaryAxis = "SecondaryAxis";
    // Buttons
    public const string Fire = "Fire";
    public const string SecondaryFire = "SecondaryFire";
    public const string FunctionA = "FunctionA";
    public const string FunctionB = "FunctionB";
    public const string FunctionC = "FunctionC";
    public const string FunctionD = "FunctionD";
    ...
}
```
## 输入映射
来自鼠标、键盘的输入大多都是按键输入，本质上只有按下和松开两种状态。在操控机器人时，不同的功能对于按下和松开有着不同的解释。比如自瞄、小陀螺功能是按住生效，而连发、超级电容功能是按下切换状态。所以输入处理的第一个部分是解释按键输入，将按键状态映射为同样是布尔值的功能状态。
```
// 输入映射方式
private readonly Dictionary<string, InputButtonConfig>
    _buttonControlPatterns = new Dictionary<string, InputButtonConfig>
    {
        {InputActionID.Fire, InputButtonConfig.Push}, // 开火、自瞄按下生效
        {InputActionID.SecondaryFire, InputButtonConfig.Push},
        {InputActionID.FunctionA, InputButtonConfig.Toggle}, // 其他功能
        {InputActionID.FunctionB, InputButtonConfig.Toggle}, // 按下切换
        ...
    };
```
## 杆量映射

某些特殊的键位，是需要映射成为虚拟摇杆的。虽然 Unity 的输入系统有这样的功能，但为了统一输入处理，更灵活地自定义映射参数，我们在输入管理器中手动进行了虚拟摇杆的映射。

```c#
/// <summary>
/// 对两轴数字输入进行模拟插值处理（JoyStick Gravity）。
/// </summary>
/// <param name="accumulator">积分器</param>
/// <param name="delta">输入数据</param>
/// <param name="gravity">衰减系数</param>
/// <param name="maximum">积分上限</param>
/// <returns>模拟输出</returns>
private static Vector2 ApplyGravity(
    ref Vector2 accumulator,
    Vector2 delta,
    float gravity = 0.6f,
    float maximum = 5.0f)
{ ... }

/// <summary>
/// 每帧对数字轴向输入进行处理。
/// </summary>
private void FixedUpdate()
{
    primaryAxis = ApplyGravity(ref _primaryAxisGravity, _primaryAxisCache);
    secondaryAxis = ApplyGravity(ref _secondaryAxisGravity, _secondaryAxisCache);
}
```

## 延迟模拟

作为模拟 RoboMaster 赛场手感的特色功能，我们故意对用户输入进行了延迟响应。所有的输入事件会在队列中等待一定时间后再被发送给机器人。在联机游戏中，延迟模拟功能会被用于平滑不同客户端之间的延迟差异，为所有玩家提供相同的操作延迟体验。

```c#
/// <summary>
/// 在发送其他输入前模拟图传延迟。
/// </summary>
/// <param name="input">输入数据</param>
/// <param name="delay">延迟时间</param>
/// <returns></returns>
private IEnumerator SendDelayedInput(uint input, float delay)
{
    if (localRobot != null && localRobot.id.IsRobot())
    {
        if (delay > (float) NetworkTime.rtt)
        {
            yield return new WaitForSeconds(delay - (float) NetworkTime.rtt);
        }
    }
    // 防止 Delay 执行报错
    if (NetworkManager.singleton.isNetworkActive)
    {
        CmdSendInput(input);
    }
}
```


