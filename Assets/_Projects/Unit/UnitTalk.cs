using MyUtils.Parameter.Basic;
using MyUtils.TalkUtils;
using R3;
using UnityEngine;
using VContainer;

namespace _Projects.Unit
{
    public class UnitTalk : MonoBehaviour
    {
        [SerializeField] protected string _deadKey = "ally1_dead";

        [Inject] protected TalkManager _talkManager;
        [Inject] protected Health _health;

        protected virtual void Start()
        {
            _health.IsEmpty.Where(isEmpty => isEmpty)
                .Subscribe(_ => _talkManager.Talk(_deadKey))
                .AddTo(this);
        }
    }
}