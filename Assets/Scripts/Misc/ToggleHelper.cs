namespace Misc
{
    /// <summary>
    /// <c>ToggleHelper</c> 用于简化检测流式状态输入判断。
    /// <br/>可以检测输入流中的上升沿和下降沿。
    /// </summary>
    public class ToggleHelper
    {
        /// <summary>
        /// 三种流状态：RisingEdge, DroppingEdge, Hold
        /// </summary>
        public enum State
        {
            Re,
            De,
            Hold
        }

        private bool _formerState;

        /// <summary>
        /// 接收状态输入，判断流状态
        /// </summary>
        /// <param name="input">当前状态输入</param>
        /// <returns>流状态</returns>
        public State Toggle(bool input)
        {
            if (input)
            {
                if (!_formerState)
                {
                    _formerState = true;
                    return State.Re;
                }
            }
            else
            {
                if (_formerState)
                {
                    _formerState = false;
                    return State.De;
                }
            }

            return State.Hold;
        }

        public bool Current() => _formerState;
    }
}