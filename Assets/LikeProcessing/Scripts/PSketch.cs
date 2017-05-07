﻿using UnityEngine;
using UnityEditor;
using UnityStandardAssets.ImageEffects;
using UnityEngine.PostProcessing;

namespace LikeProcessing
{
    public class PSketch : MonoBehaviour
    {

        protected GameObject cameraObj;
        GameObject lightObj;
        PostProcessingProfile postProcessingProfile;
        UTJ.MP4Recorder mp4Recorder;

        void Awake()
        {
            Application.runInBackground = true;

            cameraObj = new GameObject("PSketch MainCamera");
            Camera camera = cameraObj.AddComponent<Camera>();
            camera.tag = "MainCamera";
            camera.clearFlags = CameraClearFlags.SolidColor;
            camera.backgroundColor = Color.HSVToRGB(241.0f / 359.0f, 40.0f / 100.0f, 20.0f / 100.0f);
            //camera.farClipPlane = (Screen.height / 10.0f) / 2.0f * 10;
            camera.hdr = true;
            camera.renderingPath = RenderingPath.DeferredShading;
			cameraObj.AddComponent<PCamera> ();
            PostProcessingBehaviour postProcessingBehaviour = cameraObj.AddComponent<PostProcessingBehaviour>();
            //this.postProcessingProfile = (PostProcessingProfile)AssetDatabase.LoadAssetAtPath("Assets/LikeProcessing/Assets/Post-Processing-Profile.asset", typeof(PostProcessingProfile));
            this.postProcessingProfile = (PostProcessingProfile) ScriptableObject.CreateInstance("PostProcessingProfile");
            postProcessingBehaviour.profile = this.postProcessingProfile;
            this.mp4Recorder = cameraObj.AddComponent<UTJ.MP4Recorder>();
            mp4Recorder.m_outputDir = new UTJ.DataPath(UTJ.DataPath.Root.CurrentDirectory, "");
            setupCamera();

            lightObj = new GameObject("PSketch Light");
            Light light = lightObj.AddComponent<Light>();
            light.type = LightType.Directional;
            lightObj.transform.Rotate(15, 15, 0);
			light.shadows = LightShadows.Soft;
            light.intensity = 0.5f;

			QualitySettings.shadowDistance = (Screen.height/100.0f) / 2.0f * 10;
        }

//		public PSketch() {
//			this.gameObject.name = "PSketch";
//		}

        public static void setupCamera(bool origin = false)
        {
            setupCamera(Camera.main, origin);
        }

        public static void setupCamera(Camera camera, bool origin = false)
        {
            setupCamera(camera, Screen.height/100.0f, origin);
        }

        public static void setupCamera(Camera camera, float height, bool origin = false)
        {
			camera.farClipPlane = height / 2.0f * 10;
            camera.fieldOfView = Mathf.Rad2Deg * Mathf.PI / 3.0f;
            if (!origin)
                camera.transform.position = new Vector3(0, 1, -1 * ((height / 2.0f) / Mathf.Tan(Mathf.PI * 30.0f / 180.0f)));
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

        public void bloom(float threshold = 1.0f)
        {
            //Bloom bloom = cameraObj.AddComponent<Bloom>();
            //bloom.sh
            //bloom.blurAndFlaresShader = Shader.Find("Hidden/BlurAndFlares");
            //bloom.bloomThreshold = threshold;
            this.postProcessingProfile.bloom.enabled = true;
        }

        public void background(Color color)
        {
            cameraObj.GetComponent<Camera>().backgroundColor = color;
        }

		public static Vector3 randomVector() {
			float angle = Random.value * Mathf.PI * 2;
			float vz = Random.value * 2 - 1;
			float vx = Mathf.Sqrt(1-vz*vz) * Mathf.Cos(angle);
			float vy = Mathf.Sqrt(1-vz*vz) * Mathf.Sin(angle);
			return new Vector3(vx, vy, vz);
		}

		public static Vector3 randomVector(float noise1, float noise2) {
			float angle = noise1 * Mathf.PI * 2;
			float vz = noise2 * 2 - 1;
			float vx = Mathf.Sqrt(1-vz*vz) * Mathf.Cos(angle);
			float vy = Mathf.Sqrt(1-vz*vz) * Mathf.Sin(angle);
			return new Vector3(vx, vy, vz);
		}

		public static void fill(GameObject obj, Color color) {
			if (obj.GetComponent<MeshRenderer>() == null) obj.AddComponent<MeshRenderer>();
			MeshRenderer meshRenderer = obj.GetComponent<MeshRenderer> ();
			Material material = new Material(Shader.Find("Standard"));
			material.color = color;
			meshRenderer.material = material;
		}

        public void record() {
            if (!this.mp4Recorder.m_recording)
            {
                this.mp4Recorder.BeginRecording();
            }
            else {
                this.mp4Recorder.EndRecording();
            }
        }

    }

}
