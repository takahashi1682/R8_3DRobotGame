using _Projects.Unit;
using MyUtils.TalkUtils;
using MyUtils.VContainerExtensions;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace _Projects.Game
{
    public class GameScopeRoot : AbstractScopeRoot
    {
        [SerializeField] private Camera _camera;
        [SerializeField] private TalkManager _talkManager;

        protected override void ConfigureScope(IContainerBuilder builder)
        {
            base.ConfigureScope(builder);
            builder.RegisterComponent(_camera);
            builder.RegisterComponent(_talkManager);

            builder.RegisterEntryPoint<UnitManager>();
            builder.RegisterEntryPoint<UpdateScheduler>();
        }
    }
}