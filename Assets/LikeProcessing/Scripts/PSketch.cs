﻿using UnityEngine;
using UnityStandardAssets.ImageEffects;

namespace LikeProcessing
{
    public class PSketch : MonoBehaviour
    {

        GameObject cameraObj;
        GameObject lightObj;

        void Awake()
        {
            Application.runInBackground = true;

            cameraObj = new GameObject("PSketch MainCamera");
            Camera camera = cameraObj.AddComponent<Camera>();
            camera.tag = "MainCamera";
            camera.clearFlags = CameraClearFlags.SolidColor;
            camera.backgroundColor = Color.gray;
            setupCamera();

            lightObj = new GameObject("PSketch Light");
            Light light = lightObj.AddComponent<Light>();
            light.type = LightType.Directional;
            lightObj.transform.Rotate(15, 15, 0);
        }

        public static void setupCamera(bool origin = false)
        {
            setupCamera(Camera.main, origin);
        }

        public static void setupCamera(Camera camera, bool origin = false)
        {
            setupCamera(camera, Screen.height, origin);
        }

        public static void setupCamera(Camera camera, int height, bool origin = false)
        {
            camera.fieldOfView = Mathf.Rad2Deg * Mathf.PI / 3.0f;
            if (!origin)
                camera.transform.position = new Vector3(0, 0, -1 * ((height / 2.0f) / Mathf.Tan(Mathf.PI * 30.0f / 180.0f)));
            else
                camera.transform.position = new Vector3(0, 0, 0);
        }

        public static void cameraRotateWithMouse(float sensitivity = 0.5f)
        {
            Camera.main.transform.rotation = Quaternion.identity;
            Vector3 rotateV = new Vector3(Input.mousePosition.y - Screen.height / 2, Input.mousePosition.x - Screen.width / 2);
            Camera.main.transform.Rotate(rotateV * sensitivity);
        }

        public void blur()
        {
            BlurOptimized blurComp = cameraObj.AddComponent<BlurOptimized>();
            blurComp.blurShader = Shader.Find("Hidden/FastBlur");
        }

        public void background(Color color)
        {
            cameraObj.GetComponent<Camera>().backgroundColor = color;
        }

    }

}
