using System;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

namespace EB.DOTS.HSM
{
    public abstract class BaseState
    {
        internal abstract BaseState GetSubStateInternal();
        internal abstract void SetSubStateInternal(BaseState subState);
        
        internal abstract void OnEnterInternal(SystemBase system);
        internal abstract void OnUpdateInternal(SystemBase system);
        internal abstract void OnLateUpdateInternal(SystemBase system);

        internal abstract void OnExitInternal(SystemBase system);

        internal abstract void TryChangeStateInternal(SystemBase system);
    }
    
    public abstract class BaseState<TSelf> : BaseState where TSelf : BaseState
    {
        public BaseState SubState { get; internal set; }
        private bool _isSubStateChangeRequested = false;
        private BaseState _nextSubState = null;

        protected virtual List<Type> RequiredSystems => new();
        
        protected virtual void OnEnter(SystemBase system){}
        protected virtual void OnUpdate(SystemBase system){}
        protected virtual void OnLateUpdate(SystemBase system){}
        protected virtual void OnExit(SystemBase system){}
        
        internal override BaseState GetSubStateInternal()
        {
            return SubState;
        }
        
        internal override void SetSubStateInternal(BaseState subState)
        {
            _isSubStateChangeRequested = true;
            _nextSubState = subState;
        }
        
        protected void SetSubState(ISubState<TSelf> subState)
        {
            _isSubStateChangeRequested = true;
            _nextSubState = subState as BaseState;
        }

        internal override void OnEnterInternal(SystemBase system)
        {
            SetSystemsActive(system, true);
            OnEnter(system);
            TryChangeStateInternal(system);
        }
        
        internal override void OnUpdateInternal(SystemBase system)
        {
            OnUpdate(system);
            SubState?.OnUpdateInternal(system);
        }

        internal override void OnLateUpdateInternal(SystemBase system)
        {
            OnLateUpdate(system);
            SubState?.OnLateUpdateInternal(system);
        }
        
        internal override void TryChangeStateInternal(SystemBase system)
        {
            if (!_isSubStateChangeRequested)
            {
                SubState?.TryChangeStateInternal(system);
                return;
            }

            _isSubStateChangeRequested = false;
            
            SubState?.OnExitInternal(system);
            SubState = _nextSubState;
            _nextSubState = null;
            
            if (SubState == null)
            {
                return;
            }
            
            (SubState as ISubState<TSelf>)?.SetParent(this as TSelf);
            SubState.OnEnterInternal(system);
        }

        internal override void OnExitInternal(SystemBase system)
        {
            SubState?.OnExitInternal(system);
            SubState = null;
            OnExit(system);
            SetSystemsActive(system, false);
        }
        
        private void SetSystemsActive(SystemBase system, bool isActive)
        {
            if (HsmTools.IsAppQuitting)
            {
                return;
            }
            
            foreach (var systemType in RequiredSystems)
            {
                if (systemType.IsClass)
                {
                    var systemRef = system.World.GetExistingSystemManaged(systemType);
                    if (systemRef == null)
                    {
                        Debug.LogError($"System {systemType} not found or not created yet.");
                        continue;
                    }
                    
                    systemRef.Enabled = isActive;
                }
                else
                {
                    var systemHandle = system.World.Unmanaged.GetExistingUnmanagedSystem(systemType);
                    if (systemHandle == SystemHandle.Null)
                    {
                        Debug.LogError($"System {systemType} not found or not created yet.");
                        continue;
                    }
                    
                    system.World.Unmanaged.ResolveSystemStateRef(systemHandle).Enabled = isActive;
                }
            }
        }
        
    }
}