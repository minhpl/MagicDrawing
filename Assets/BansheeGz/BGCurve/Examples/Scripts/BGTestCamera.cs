using BansheeGz.BGSpline.Components;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace BansheeGz.BGSpline.Example
{
    //camera movements
    public class BGTestCamera : MonoBehaviour
    {
        private const int Speed = 100;
        public GameObject bgcurve;

        private void Awake()
        {
            if(bgcurve!=null)
            {
                var lr = bgcurve.GetComponent<LineRenderer>();
                Vector3[] positons = new Vector3[lr.positionCount];
                var a = bgcurve.GetComponent<LineRenderer>().GetPositions(positons);
                Debug.Log(positons.Length);
                Debug.Log(positons);

                //bgcurve.GetComponent<BGCcMath>()
            }            
        }


        private GUIStyle style;

        // Update is called once per frame
        private void Update()
        {
            if (Input.GetKey(KeyCode.A)) transform.RotateAround(Vector3.zero, Vector3.up, Speed*Time.deltaTime);
            else if (Input.GetKey(KeyCode.D)) transform.RotateAround(Vector3.zero, Vector3.up, -Speed*Time.deltaTime);
        }

        private void OnGUI()
        {
            if (style == null) style = new GUIStyle(GUI.skin.label) {fontSize = 24};

            GUILayout.BeginHorizontal();

            GUILayout.Label("Use A and D to rotate camera", style);

            if (BGTestMainMenu.Inited && GUILayout.Button("To Main Menu")) SceneManager.LoadScene("BGCurveMainMenu");

            GUILayout.EndHorizontal();
        }
    }
}