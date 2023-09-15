using Doozy.Runtime.Signals;
using Doozy.Runtime.UIManager.Input;
using UnityEngine;
using UnityEngine.Events;

namespace Doozy.Runtime
{
    public class BackButtonListener : MonoBehaviour
    {
        public UnityEvent OnBackButton = new UnityEvent();

        private SignalReceiver m_Receiver;

        private void Awake() =>
            m_Receiver = new SignalReceiver().AddOnSignalCallback(signal => OnBackButton?.Invoke());

        private void OnEnable() =>
            BackButton.stream.ConnectReceiver(m_Receiver);

        private void OnDisable() =>
            BackButton.stream.DisconnectReceiver(m_Receiver);
    }
}
