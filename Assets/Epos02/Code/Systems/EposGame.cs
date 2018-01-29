using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace The5_Epos02
{
    public enum EGameMode { Menu = 0, ThirdPerson = 1, RTS = 2 };

    [RequireComponent(typeof(AISystem))]
    public class EposGame : MonoBehaviour
    {
        public EGameMode gameMode = EGameMode.ThirdPerson;

        public static EposGame game;
        public static CActor playerActor;
        public static AISystem aiSystem;
        public static CameraSystem cameraSystem;

        private void init()
        {
            DontDestroyOnLoad(this);
            aiSystem = this.GetComponent<AISystem>();
            EposGame.game = this;

            //spawnPlayer();
            DEBUG_findAlternatePlayerGO("_player_test");
            initCameraSystem();

        }

        private void initCameraSystem()
        {
            GameObject cameraGO = new GameObject("_Camera");
            DontDestroyOnLoad(cameraGO);
            cameraSystem = cameraGO.AddComponent<CameraSystem>();
        }

        private void spawnPlayer()
        {
            GameObject playerGO = new GameObject("_Player");
            DontDestroyOnLoad(playerGO);
            EposGame.playerActor = playerGO.AddComponent<CActor>();
        }

        private void DEBUG_findAlternatePlayerGO(string name)
        {
            CActor[] actors = FindObjectsOfType<CActor>();
            foreach (CActor actor in actors)
            {
                if (actor.gameObject.name == name)
                {
                    EposGame.playerActor = actor;
                    return;
                }
            }
        }



        private void Awake()
        {
            init();
        }

    }

}