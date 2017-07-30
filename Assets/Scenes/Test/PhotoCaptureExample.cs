﻿//using UnityEngine;
//using System.Collections;
//using System.Linq;
//using UnityEngine.VR.WSA.WebCam;

//public class PhotoCaptureExample : MonoBehaviour
//{
//    PhotoCapture photoCaptureObject = null;
//    Texture2D targetTexture = null;

//    // Use this for initialization
//    void Start()
//    {
//        Resolution cameraResolution = PhotoCapture.SupportedResolutions.OrderByDescending((res) => res.width * res.height).First();
//        targetTexture = new Texture2D(cameraResolution.width, cameraResolution.height);

//        // Create a PhotoCapture object
//        PhotoCapture.CreateAsync(false, delegate (PhotoCapture captureObject)
//        {
//            photoCaptureObject = captureObject;
//            CameraParameters cameraParameters = new CameraParameters();
//            cameraParameters.hologramOpacity = 0.0f;
//            cameraParameters.cameraResolutionWidth = cameraResolution.width;
//            cameraParameters.cameraResolutionHeight = cameraResolution.height;
//            cameraParameters.pixelFormat = CapturePixelFormat.BGRA32;

//            // Activate the camera
//            photoCaptureObject.StartPhotoModeAsync(cameraParameters, delegate (PhotoCapture.PhotoCaptureResult result)
//            {
//                // Take a picture
//                photoCaptureObject.TakePhotoAsync(delegate (PhotoCapture.PhotoCaptureResult result2, PhotoCaptureFrame photoCaptureFrame)
//                {
//                    photoCaptureFrame.UploadImageDataToTexture(targetTexture);

//                    // Create a gameobject that we can apply our texture to
//                    GameObject quad = GameObject.CreatePrimitive(PrimitiveType.Quad);
//                    Renderer quadRenderer = quad.GetComponent<Renderer>() as Renderer;
//                    quadRenderer.material = new Material(Shader.Find("Transparent/Diffuse"));

//                    quad.transform.parent = this.transform;
//                    quad.transform.localPosition = new Vector3(0.0f, 0.0f, 3.0f);
//                    quad.transform.localScale = new Vector3(12, 12, 12);
//                    quadRenderer.material.SetTexture("_MainTex", targetTexture);

//                    // Deactivate our camera
//                    photoCaptureObject.StopPhotoModeAsync((PhotoCapture.PhotoCaptureResult result3) =>
//                    {
//                        // Shutdown our photo capture resource
//                        photoCaptureObject.Dispose();
//                        photoCaptureObject = null;
//                    });
//                });
//            });
//        });
//    }

//    void OnCapturedPhotoToMemory(PhotoCapture.PhotoCaptureResult result, PhotoCaptureFrame photoCaptureFrame)
//    {
//        // Copy the raw image data into our target texture
//        photoCaptureFrame.UploadImageDataToTexture(targetTexture);

//        // Create a gameobject that we can apply our texture to
//        GameObject quad = GameObject.CreatePrimitive(PrimitiveType.Quad);
//        Renderer quadRenderer = quad.GetComponent<Renderer>() as Renderer;
//        quadRenderer.material = new Material(Shader.Find("Transparent/Diffuse"));

//        quad.transform.parent = this.transform;
//        quad.transform.localPosition = new Vector3(0.0f, 0.0f, 0.0f);

//        quadRenderer.material.SetTexture("_MainTex", targetTexture);

//        // Deactivate our camera
//        photoCaptureObject.StopPhotoModeAsync(OnStoppedPhotoMode);
//    }

//    void OnStoppedPhotoMode(PhotoCapture.PhotoCaptureResult result)
//    {
//        // Shutdown our photo capture resource
//        photoCaptureObject.Dispose();
//        photoCaptureObject = null;
//    }
//}