namespace Doozy.Runtime.UIManager.Input
{
    /// <summary>
    /// Describes the types of controller input modes that are enabled.
    /// By default the system reacts to mouse clicks and touches.
    /// </summary>
    public enum LegacyInputMode
    {
        /// <summary> System reacts to mouse clicks and touches  </summary>
        None,

        /// <summary> System reacts to a set KeyCode, mouse clicks and touches </summary>
        KeyCode,

        /// <summary> System reacts to a set virtual button name (set up in the InputManager), mouse clicks and touches </summary>
        VirtualButton
    }
}
