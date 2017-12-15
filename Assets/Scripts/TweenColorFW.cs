using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TweenColorFW : MonoBehaviour {
    LTDescr ltdes;
    public ParticleSystem ps;
    void Start () {
        int i = 0;
        ltdes = LeanTween.value(0, 1, 3f).setOnUpdate((float v) =>
         {
             if (ps != null)
                 ps.GetComponent<Renderer>().sharedMaterial.SetColor("_TintColor", Color.HSVToRGB(v, 1, 1));
         }).setRepeat(-1);
    }

    private void OnDisable()
    {
        ltdes.destroyOnComplete = true;
        LeanTween.cancelAll();
    }
}
