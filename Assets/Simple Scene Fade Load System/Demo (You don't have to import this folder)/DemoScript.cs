using UnityEngine;
using System.Collections;

namespace DemoTransitionBetweenScene
{
    public class DemoScript : MonoBehaviour
    {
        public string scene;
        public Color loadToColor = Color.white;
        
        void OnGUI()
        {            
            if (GUI.Button(new Rect(0, 0, 100, 30), "Start"))
            {
                Initiate.Fade(scene, loadToColor, 0.5f);
            }
        }
    }

}

