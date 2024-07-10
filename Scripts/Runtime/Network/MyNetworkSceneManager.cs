using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using Util;

namespace Network
{
    /// <summary>
    /// Manage the scene changes queries and scene changes.
    /// </summary>
    public class MyNetworkSceneManager : NetworkSingleton<MyNetworkSceneManager>
    {
        public double msBetweenSceneChange = 5000;

        Scene _loadedScene;

        readonly NetworkVariable<bool> _gameStarted = new();
        readonly NetworkVariable<long> _lastSceneChangeTicks = new();

        /// <summary>
        /// Check if loading the new scene is valid and if so tells the server to load it.
        /// </summary>
        /// <param name="sceneName">Name of the scene to load.</param>
        /// <returns>If the scene is being loaded.</returns>
        public bool LoadScene(string sceneName)
        {
            if (!_gameStarted.Value)
            {
                Debug.Log("Wait for the game to start before changing the scene");
                return false;
            }

            if (_loadedScene.IsValid() && sceneName == _loadedScene.name)
            {
                Debug.Log("Scene already loaded.");
                return false;
            }

            if (!EnoughElapsedTime()) 
                return false;

            LoadSceneRpc(sceneName);

            return true;
        }

        /// <summary>
        /// Check if the time between now and the last scene change is big enough.
        /// </summary>
        /// <returns>True if it is big enough, false otherwise.</returns>
        bool EnoughElapsedTime()
        {
            // A single tick represents one hundred nanoseconds or one ten-millionth of a second. There are 10,000 ticks in
            // a millisecond (see TicksPerMillisecond) and 10 million ticks in a second
            long ticksElapsed = DateTime.Now.Ticks - _lastSceneChangeTicks.Value;
            long msElapsed = ticksElapsed / 10000;

            if (!(msElapsed < msBetweenSceneChange)) return true;

            Debug.Log("Wait " + (msBetweenSceneChange - msElapsed) / 1000 + " seconds before changing the scene again.");
            return false;
        }

        [Rpc(SendTo.Server, RequireOwnership = false)]
        void LoadSceneRpc(string sceneName)
        {
            NetworkManager.Singleton.SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
            _lastSceneChangeTicks.Value = DateTime.Now.Ticks;
        }

        [Rpc(SendTo.Server)]
        public void GameStartRpc()
        {
            _gameStarted.Value = true;
        }
    }
}