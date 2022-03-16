using UnityEngine;
using System.Collections;
using System.IO;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.Networking;
using System;


public class cam2 : MonoBehaviour
{
    WebCamTexture webCam;
    string your_path = "C:\\Users\\Jay\\Desktop";
    public RawImage display;
    public AspectRatioFitter fit;
    // public string base64img

    public void Start()
    {
        webCam = new WebCamTexture();
        webCam.Play();

        if(WebCamTexture.devices.Length==0)
        {
            Debug.LogError("can not found any camera!");
            return;
        }
        int index = -1;
        for (int i = 0; i < WebCamTexture.devices.Length; i++)
        {
            if (WebCamTexture.devices[i].name.ToLower().Contains("your webcam name"))
            {
                Debug.LogError("WebCam Name:" + WebCamTexture.devices[i].name + "   Webcam Index:" + i);
                index = i;
            }
        }

        // if (index == -1)
        // {
        //     Debug.LogError("can not found your camera name!");
        //     return;
        // }

        // WebCamDevice device = WebCamTexture.devices[index];
        // webCam = new WebCamTexture(device.name);
        webCam.Play();
        // StartCoroutine(TakePhoto());
        display.texture = webCam;

    }

   
    public void Update()
    {


        float ratio = (float)webCam.width / (float)webCam.height;
        fit.aspectRatio = ratio;


        float ScaleY = webCam.videoVerticallyMirrored ? -1f : 1f;
        display.rectTransform.localScale = new Vector3(1f, ScaleY, 1f);

        int orient = -webCam.videoRotationAngle;
        display.rectTransform.localEulerAngles = new Vector3(0, 0, orient);



    }



    public void callTakePhoto()
    {
        StartCoroutine(TakePhoto("http://127.0.0.1:5000/"));
        StartCoroutine(getRequest("http://127.0.0.1:5000/"));
    }
    IEnumerator TakePhoto(string url)  
    {
        yield return new WaitForEndOfFrame();

        Texture2D photo = new Texture2D(webCam.width, webCam.height);
        photo.SetPixels(webCam.GetPixels());
        photo.Apply();

        //Encode to a PNG
        byte[] bytes = photo.EncodeToPNG();
        string base64img = Convert.ToBase64String(bytes);
      
        WWWForm form = new WWWForm();
        form.AddField("name",base64img);
        // form.AddField("nice","ok");

        UnityWebRequest uwr = UnityWebRequest.Post(url, form);
        yield return uwr.SendWebRequest();
        


        if (uwr.isNetworkError)
        {
            Debug.Log("Error While Sending: " + uwr.error);
        }
        else
        {
            Debug.Log("Received: " + uwr.downloadHandler.text);
        }
    }

    
    IEnumerator getRequest(string uri)
    {
    UnityWebRequest uwr = UnityWebRequest.Get(uri);
    yield return uwr.SendWebRequest();

    if (uwr.isNetworkError)
    {
        Debug.Log("Error While Sending: " + uwr.error);
    }
    else
    {
        Debug.Log("Received: " + uwr.downloadHandler.text);
    }
}
}