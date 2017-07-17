using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

public class testin : MonoBehaviour
{
    public static WebCamTexture camTex;
    public float th = 0.09f;
    System.Diagnostics.Stopwatch s;
    static Texture2D img, rImg, gImg, bImg, kImg, roImg, goImg, boImg, koImg, ORimg, ANDimg, SUMimg, AVGimg;
    static float[,] rL, gL, bL, kL, ORL, ANDL, SUML, AVGL;
    static Color temp;
    static float t;

    void Start()
    {
        WebCamDevice[] devices = WebCamTexture.devices;
        WebCamDevice cam = devices[0];
        camTex = new WebCamTexture(cam.name, 200, 200);
        camTex.Play();
        //GetWebCamImage();
        img = new Texture2D(camTex.width, camTex.height);
        //initialize textures
        rImg = new Texture2D(img.width, img.height);
        bImg = new Texture2D(img.width, img.height);
        gImg = new Texture2D(img.width, img.height);
        kImg = new Texture2D(img.width, img.height);
        //initialize gradient arrays
        rL = new float[img.width, img.height];
        gL = new float[img.width, img.height];
        bL = new float[img.width, img.height];
        kL = new float[img.width, img.height];
        ORL = new float[img.width, img.height];
        ANDL = new float[img.width, img.height];
        SUML = new float[img.width, img.height];
        AVGL = new float[img.width, img.height];
        //initialize final textures
        roImg = new Texture2D(img.width, img.height);
        goImg = new Texture2D(img.width, img.height);
        boImg = new Texture2D(img.width, img.height);
        koImg = new Texture2D(img.width, img.height);
        ORimg = new Texture2D(img.width, img.height);
        ANDimg = new Texture2D(img.width, img.height);
        SUMimg = new Texture2D(img.width, img.height);
        AVGimg = new Texture2D(img.width, img.height);
    }

    void Update()
    {
        GetWebCamImage();
    }

    void GetWebCamImage()
    {
        img.SetPixels(camTex.GetPixels());
        CalculateEdges();
    }

    void CalculateEdges()
    {

        //calculate new textures
        for (int x = 0; x < img.width; x++)
        {
            for (int y = 0; y < img.height; y++)
            {
                temp = img.GetPixel(x, y);
                rImg.SetPixel(x, y, new Color(temp.r, 0, 0));
                gImg.SetPixel(x, y, new Color(0, temp.g, 0));
                bImg.SetPixel(x, y, new Color(0, 0, temp.b));
                t = temp.r + temp.g + temp.b;
                t /= 3f;
                kImg.SetPixel(x, y, new Color(t, t, t));
            }
        }
        rImg.Apply();
        gImg.Apply();
        bImg.Apply();
        kImg.Apply();

        //calculate gradient values
        for (int x = 0; x < img.width; x++)
        {
            for (int y = 0; y < img.height; y++)
            {
                rL[x, y] = gradientValue(x, y, 0, rImg);
                gL[x, y] = gradientValue(x, y, 1, gImg);
                bL[x, y] = gradientValue(x, y, 2, bImg);
                kL[x, y] = gradientValue(x, y, 2, kImg);
                ORL[x, y] = (rL[x, y] >= th || gL[x, y] >= th || bL[x, y] >= th) ? th : 0f;
                ANDL[x, y] = (rL[x, y] >= th && gL[x, y] >= th && bL[x, y] >= th) ? th : 0f;
                SUML[x, y] = rL[x, y] + gL[x, y] + bL[x, y];
                AVGL[x, y] = SUML[x, y] / 3f;
            }
        }
        //create texture from gradient values
        TextureFromGradientRef(rL, th, ref roImg);
        TextureFromGradientRef(gL, th, ref goImg);
        TextureFromGradientRef(bL, th, ref boImg);
        TextureFromGradientRef(kL, th, ref koImg);
        TextureFromGradientRef(ORL, th, ref ORimg);
        TextureFromGradientRef(ANDL, th, ref ANDimg);
        TextureFromGradientRef(SUML, th, ref SUMimg);
        TextureFromGradientRef(AVGL, th, ref AVGimg);
    }

    // Update is called once per frame
    void OnGUI()
    {
        GUILayout.BeginHorizontal();

        GUILayout.BeginVertical();
        GUILayout.Label("original");
        GUILayout.Label(camTex);
        GUILayout.Label("red");
        GUILayout.Label(rImg);
        GUILayout.Label("blue");
        GUILayout.Label(bImg);
        GUILayout.EndVertical();

        GUILayout.BeginVertical();
        GUILayout.Label("green");
        GUILayout.Label(gImg);
        GUILayout.Label("grey");
        GUILayout.Label(kImg);
        GUILayout.Label("grey detection");
        GUILayout.Label(koImg);
        GUILayout.EndVertical();

        GUILayout.BeginVertical();
        GUILayout.Label("red detection");
        GUILayout.Label(roImg);
        GUILayout.Label("green detection");
        GUILayout.Label(goImg);
        GUILayout.Label("blue detection");
        GUILayout.Label(boImg);
        GUILayout.EndVertical();

        GUILayout.BeginVertical();
        GUILayout.Label("OR detection");
        GUILayout.Label(ORimg);
        GUILayout.Label("AND detection");
        GUILayout.Label(ANDimg);
        GUILayout.Label("SUM detection");
        GUILayout.Label(SUMimg);
        GUILayout.EndVertical();

        GUILayout.BeginVertical();
        GUILayout.Label("Average detection");
        GUILayout.Label(AVGimg);
        GUILayout.EndVertical();

        GUILayout.EndHorizontal();
    }

    float gradientValue(int ex, int why, int colorVal, Texture2D image)
    {
        float lx = 0f;
        float ly = 0f;
        if (ex > 0 && ex < image.width)
            lx = 0.5f * (image.GetPixel(ex + 1, why)[colorVal] - image.GetPixel(ex - 1, why)[colorVal]);
        if (why > 0 && why < image.height)
            ly = 0.5f * (image.GetPixel(ex, why + 1)[colorVal] - image.GetPixel(ex, why - 1)[colorVal]);
        return Mathf.Sqrt(lx * lx + ly * ly);
    }

    Texture2D TextureFromGradient(float[,] g, float thres)
    {
        Texture2D output = new Texture2D(g.GetLength(0), g.GetLength(1));
        for (int x = 0; x < output.width; x++)
        {
            for (int y = 0; y < output.height; y++)
            {
                if (g[x, y] >= thres)
                    output.SetPixel(x, y, Color.black);
                else
                    output.SetPixel(x, y, Color.white);
            }
        }
        output.Apply();
        return output;
    }

    void TextureFromGradientRef(float[,] g, float thres, ref Texture2D output)
    {
        for (int x = 0; x < output.width; x++)
        {
            for (int y = 0; y < output.height; y++)
            {
                if (g[x, y] >= thres)
                    output.SetPixel(x, y, Color.black);
                else
                    output.SetPixel(x, y, Color.white);
            }
        }
        output.Apply();
    }
}