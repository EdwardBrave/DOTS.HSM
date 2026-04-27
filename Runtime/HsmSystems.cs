using Unity.Entities;

namespace EB.DOTS.HSM
{
    [UpdateInGroup(typeof(InitializationSystemGroup), OrderFirst = true)]
    public partial class HsmPreUpdateSystem : SystemBase
    {
        private bool _isFirstUpdate = true;
        
        protected override void OnCreate()
        {
            RequireForUpdate<HsmRoot>();
        }

        protected override void OnUpdate()
        {
            var hsm = SystemAPI.ManagedAPI.GetSingleton<HsmRoot>();
            if (_isFirstUpdate)
            {
                hsm.OnEnterInternal(this);
                _isFirstUpdate = false;
            }
            
            hsm.OnUpdateInternal(this);
        }
    }
    
    [UpdateInGroup(typeof(LateSimulationSystemGroup), OrderLast = true)]
    public partial class HsmLateUpdateSystem : SystemBase
    {
        protected override void OnCreate()
        {
            RequireForUpdate<HsmRoot>();
        }

        protected override void OnUpdate()
        {
            var hsm = SystemAPI.ManagedAPI.GetSingleton<HsmRoot>();
            hsm.OnLateUpdateInternal(this);
            hsm.TryChangeStateInternal(this);
        }

        protected override void OnDestroy()
        {
            if (!SystemAPI.ManagedAPI.HasSingleton<HsmRoot>())
            {
                return;
            }
            
            var hsm = SystemAPI.ManagedAPI.GetSingleton<HsmRoot>();
            hsm.OnExitInternal(this);
        }
    }
}
