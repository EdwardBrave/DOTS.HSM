using System;
using System.Collections.Generic;
using Unity.Entities;

namespace EB.DOTS.HSM
{
    public class HsmRoot : BaseState<HsmRoot>, IComponentData
    {
#if UNITY_EDITOR && DEBUG
        public readonly List<string> statesStack = new ();

        protected override void OnUpdate(SystemBase system)
        {
            statesStack.Clear();
            BaseState subState = SubState as BaseState;
            while (subState != null)
            {
                statesStack.Add(subState.GetType().Name);
                subState = subState.GetSubStateInternal();
            }
        }
#endif
        
        private readonly List<Type> _requiredSystems;
        protected override List<Type> RequiredSystems => _requiredSystems;
        
        public HsmRoot(){}

        public HsmRoot(ISubState<HsmRoot> initState, List<Type> newRequiredSystems = null)
        {
            base.SetSubState(initState);
            _requiredSystems = newRequiredSystems ?? new List<Type>();
        }

        public new void SetSubState(ISubState<HsmRoot> nextState)
        {
            base.SetSubState(nextState);
        }
    }
}