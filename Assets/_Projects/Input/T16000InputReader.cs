using System;
using System.Collections.Generic;
using _Projects.Unit;
using MyUtils.VContainerExtensions;
using R3;
using UnityEngine;
using UnityEngine.InputSystem;
using VContainer;
using VContainer.Unity;

namespace _Projects.Input
{
    /// <summary>
    /// Thrustmaster T.16000Mを左右2本(HOSAS)で使う入力を読み取るクラス。
    /// 左右の判別はHOSASライブラリがスティック底面のスイッチを読んで{leftHand}/{rightHand}を付けている。
    /// PlayerInputReaderと同じく、Inputフェーズの先頭でInputSystem.Update()を呼んで入力を確定させる。
    /// バインディングはInspectorで変更できる(コントロール名は Window > Analysis > Input Debugger で確認)。
    /// </summary>
    public class T16000InputReader : AbstractUnitControllable,
        IScopeRegisterable
    {
        private const string LeftHand = "<HID::Thrustmaster T.16000M>{leftHand}";
        private const string RightHand = "<HID::Thrustmaster T.16000M>{rightHand}";

        [Header("Left Hand")]
        [SerializeField] private string _moveBinding = LeftHand + "/stick";
        [SerializeField] private string _boostBinding = LeftHand + "/trigger";
        [SerializeField] private string _flyBinding = LeftHand + "/button2";

        [Header("Right Hand")]
        [SerializeField] private string _lookBinding = RightHand + "/stick";
        [SerializeField] private string _fireBinding = RightHand + "/trigger";
        [SerializeField] private string _lockOnBinding = RightHand + "/button2";

        private string _lookDeviceName = string.Empty;
        public override string LookDeviceName => _lookDeviceName;

        private readonly List<InputAction> _actions = new();

        [Inject] private IUpdateObservable _updateObservable;

        public void OnRegister(IContainerBuilder builder)
        {
            builder.RegisterComponent(this).As<IUnitControllable>();
        }

        // アクションはOnEnableで有効化するので、それより前のAwakeで作っておく(Startだと有効化されない)
        protected override void Awake()
        {
            base.Awake();

            AddAction(_moveBinding, InputActionType.Value, c => _move.Value = c.ReadValue<Vector2>());
            AddAction(_lookBinding, InputActionType.Value, c =>
            {
                _look.Value = c.ReadValue<Vector2>();
                _lookDeviceName = c.control?.device?.displayName ?? string.Empty;
            });
            AddAction(_fireBinding, InputActionType.Button, c => _fire.Value = c.ReadValueAsButton());
            AddAction(_boostBinding, InputActionType.Button, c => _boost.Value = c.ReadValueAsButton());
            AddAction(_flyBinding, InputActionType.Button, c => _fly.Value = c.ReadValueAsButton());
            AddAction(_lockOnBinding, InputActionType.Button, c => _lockOn.Value = c.ReadValueAsButton());
        }

        private void Start()
        {
            InputSystem.settings.updateMode = InputSettings.UpdateMode.ProcessEventsManually;

            _updateObservable.OnUpdate(EUpdatePhase.Input)
                .Subscribe(_ => InputSystem.Update())
                .AddTo(this);
        }

        // 離した時(canceled)も同じ処理を呼ぶことで、値が0/falseに戻る
        private void AddAction(string binding, InputActionType type, Action<InputAction.CallbackContext> onChanged)
        {
            var action = new InputAction(type: type, binding: binding);
            action.performed += onChanged;
            action.canceled += onChanged;
            _actions.Add(action);
        }

        private void OnEnable()
        {
            foreach (var action in _actions) action.Enable();
        }

        private void OnDisable()
        {
            foreach (var action in _actions) action.Disable();
        }

        private void OnDestroy()
        {
            foreach (var action in _actions) action.Dispose();
        }
    }
}