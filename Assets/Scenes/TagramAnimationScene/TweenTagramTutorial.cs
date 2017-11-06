using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TweenTagramTutorial : MonoBehaviour {

    public GameObject tutorialRoot;
    public GameObject leftHand;
    public GameObject rightHand;
    public GameObject yellowSquare;
    public GameObject greenTriangle3;
    public GameObject greenTriangle4;
    public GameObject blueTriangle;
    public GameObject redTriangle;
    public GameObject orangeParallelogram3;
    public GameObject orangeParallelogram4;


    public Vector3 oPosLeftHand;
    public Vector3 oPosRightHand;


    private bool isTweening = false;
    private void Awake()
    {
       

        GetComponent<UIButton>().onClick.Add(new EventDelegate(() =>
        {
            tween1();
            //tween(blueTriangle, redTriangle, new Vector3(-6f, 316f, 0f), new Vector3(-4f, 314f, 0f));
            //tween(orangeParallelogram3, greenTriangle3, new Vector3(-65f, 334f, 0f), new Vector3(65f, 273f, 0f));
            //tween(greenTriangle4, orangeParallelogram4, new Vector3(-67f, 261f, 0f), new Vector3(58.8f, 323.8f, 0f));
        }));
    }

    void tween1()
    {
        tween(yellowSquare, greenTriangle3, new Vector3(-8f, 261f, 0f), new Vector3(-7f, 323f, 0f));
    }

    void tween2()
    {
        tween(blueTriangle, redTriangle, new Vector3(-6f, 316f, 0f), new Vector3(-4f, 314f, 0f));
    }

    void tween3()
    {
        tween(orangeParallelogram3, greenTriangle3, new Vector3(-65f, 334f, 0f), new Vector3(65f, 273f, 0f));        
    }

    void tween4()
    {
        tween(greenTriangle4, orangeParallelogram4, new Vector3(-67f, 261f, 0f), new Vector3(58.8f, 323.8f, 0f));
    }

    void tween(GameObject tagramleft, GameObject tagramRight, Vector3 goalTagramLeft, Vector3 goalTagramRight)
    {
        var tagramleftPos = tagramleft.transform.localPosition;
        var tagramRightPos = tagramRight.transform.localPosition;
        Debug.LogFormat("new Vector3({0}f, {1}f, {2}f), new Vector3({3}f, {4}f, {5}f)", tagramleftPos.x, tagramleftPos.y, tagramleftPos.z,
            tagramRightPos.x, tagramRightPos.y, tagramRightPos.z);


        if (isTweening == true)
            return;

        isTweening = true;

        var duration = 1f;
        var delay = 0.35f;
        var uiTexture = tutorialRoot.GetComponent<UITexture>(); ;
        
        var oPosLeftHand = leftHand.transform.localPosition;
        var oPosRightHand = rightHand.transform.localPosition;        
        var desDesLeftHand = new Vector3(-311f, 12f, 0f); 
        var desDesRightHand = new Vector3(304f, 69f, 0f);
        var oPosTagramLeft = tagramleft.transform.localPosition;
        var oPosTagramRight = tagramRight.transform.localPosition;
        //goalLeftHand = new Vector3(-96f, 107f, 0f);
        //goalRightHand = new Vector3(143f, 235f, 0f);


        var LTseq = LeanTween.sequence();

        var alphaUIrootDuration = uiTexture.alpha == 1 ? 0 : 1;
        LTseq.append(LeanTween.value(uiTexture.alpha, 1, alphaUIrootDuration).setOnUpdate((float value) =>
        {
            uiTexture.alpha = value;
        }).setOnStart(() =>
        {            
            uiTexture.alpha = 0;
            tutorialRoot.gameObject.SetActive(true);
        }));
        LTseq.append(delay);
        LTseq.append(()=>
        {            
            leftHand.SetActive(true);
            tagramleft.SetActive(true);
        });
        LTseq.append(LeanTween.moveLocal(tagramleft.gameObject, goalTagramLeft, duration)
            .setOnUpdateVector3((value) =>
            {
                var gap = value - oPosTagramLeft;
                leftHand.transform.localPosition = oPosLeftHand + gap;
            }));
        LTseq.append(delay);
        LTseq.append(() =>
        {
            rightHand.SetActive(true);
            tagramRight.SetActive(true);
        });
        LTseq.append(LeanTween.moveLocal(tagramRight.gameObject, goalTagramRight, duration)
                .setOnUpdateVector3((value) =>
                {
                    var gap = value - oPosTagramRight;
                    rightHand.transform.localPosition = oPosRightHand + gap;
                }).setOnComplete(() =>
                {
                    
                }));
        LTseq.append(delay);
        LTseq.append(() =>
        {
            LeanTween.moveLocal(leftHand, desDesLeftHand, duration);            
        });

        LTseq.insert(LeanTween.moveLocal(rightHand, desDesRightHand, 1));
        
        LTseq.append(delay);

        LTseq.append(() =>
        {
            var childNumber = tutorialRoot.transform.childCount;
            for (int i = 0; i < childNumber; i++)
            {
                tutorialRoot.transform.GetChild(i).gameObject.SetActive(false);
            }

            leftHand.transform.localPosition = oPosLeftHand;
            rightHand.transform.localPosition = oPosRightHand;
            tagramleft.transform.localPosition = oPosTagramLeft;
            tagramRight.transform.localPosition = oPosTagramRight;
            isTweening = false;
        });
    }
    
}
