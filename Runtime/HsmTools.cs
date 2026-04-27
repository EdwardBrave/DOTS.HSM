using System;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

namespace EB.DOTS.HSM
{
    public static class HsmTools
    {
        public static bool IsAppQuitting { get; private set; }

        public static bool IsHsmRunning()
        {
            var world = World.DefaultGameObjectInjectionWorld;
            return world is { IsCreated: true } && world.EntityManager.UniversalQuery.HasSingleton<HsmRoot>();
        }

        public static HsmRoot GetHsmRoot()
        {
            var world = World.DefaultGameObjectInjectionWorld;
            if (world is not { IsCreated: true } || !world.EntityManager.UniversalQuery.HasSingleton<HsmRoot>())
            {
                return null;
            }

            return world.EntityManager.UniversalQuery.GetSingleton<HsmRoot>();
        }
        
        
        
        public static void InitHsm(this World world, ISubState<HsmRoot> initState, List<Type> newRequiredSystems = null)
        {
            world.EntityManager.CreateSingleton(new HsmRoot(initState, newRequiredSystems));
        }
        
        public static void InitHsm(ISubState<HsmRoot> initState, List<Type> newRequiredSystems = null)
        {
            World.DefaultGameObjectInjectionWorld.EntityManager.CreateSingleton(new HsmRoot(initState, newRequiredSystems));
        }
        
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Initialize()
        {
            Application.quitting += OnApplicationQuitting;
        }
    
        private static void OnApplicationQuitting()
        {
            IsAppQuitting = true;
        }
    }

}