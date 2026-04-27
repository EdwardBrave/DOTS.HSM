namespace EB.DOTS.HSM
{
    public interface ISubState<TParent> where TParent : BaseState
    {
        public TParent Parent { get;}

        internal void SetParent(TParent parent);
    }
    
    public abstract class BaseSubState<TSelf, TParent> : BaseState<TSelf>, ISubState<TParent> 
        where TSelf : BaseState
        where TParent : BaseState 
    {
        public TParent Parent { get; private set; }

        void ISubState<TParent>.SetParent(TParent parent)
        {
            Parent = parent;
        }
        
        protected void SetState<TState>(TState state) where TState : ISubState<TParent>
        {
            Parent.SetSubStateInternal(state as BaseState);
        }
    }
}