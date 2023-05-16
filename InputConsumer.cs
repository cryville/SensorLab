using Cryville.Input;

namespace SensorLab {
	internal class InputConsumer {
		static InputConsumer _instance;
		public static InputConsumer Instance {
			get {
				_instance ??= new InputConsumer(new InputManager());
				return _instance;
			}
		}
		InputFrameHandler m_onInput;
		public event InputFrameHandler OnInput {
			add {
				m_onInput -= value;
				m_onInput += value;
			}
			remove {
				if (m_onInput == null) return;
				m_onInput -= value;
			}
		}
		readonly InputManager _manager;
		public InputConsumer(InputManager manager) { _manager = manager; }
		public void Activate() {
			_manager.EnumerateHandlers(h => h.OnInput += m_onInput);
		}
		public void Deactivate() {
			_manager.EnumerateHandlers(h => h.OnInput -= m_onInput);
		}
	}
}
