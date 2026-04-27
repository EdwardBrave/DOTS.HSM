using Unity.Entities;
using UnityEngine;

namespace EB.DOTS.HSM
{
    public class SceneTransitionState : BaseSubState<SceneTransitionState, HsmRoot>
    {
        private string _sceneName;
        private ISubState<HsmRoot> _nextState;
        private AsyncOperation _sceneLoadingOperation;

        public SceneTransitionState(string sceneName, ISubState<HsmRoot> nextState)
        {
            _sceneName = sceneName;
            _nextState = nextState;
        }

        protected override void OnEnter(SystemBase system)
        {
            _sceneLoadingOperation = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(_sceneName);
            _sceneLoadingOperation.completed += (_) => SetState(_nextState);
        }
    }
}