﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ShaderPractice {
	public class FogWithDepthTexture : PostEffectsBase {
		public Shader fogShader;
		private Material fogMaterial;
		public Material material {
			get {
				fogMaterial = CheckShaderAndCreateMaterial (fogShader, fogMaterial);
				return fogMaterial;
			}
		}

		private Camera myCamera;
		public Camera camera {
			get {
				if (myCamera == null) {
					myCamera = GetComponent<Camera> ();
				}
				return myCamera;
			}
		}

		private Transform myCameraTransform;
		public Transform cameraTransform {
			get {
				if (myCameraTransform == null) {
					myCameraTransform = camera.transform;
				}
				return myCameraTransform;
			}
		}

		[Range (0f, 3f)]
		public float fogDensity = 1.0f;

		public Color fogColor = Color.white;

		public float fogStart = 0f;
		public float fogEnd = 2f;

		void OnEnable () {
			camera.depthTextureMode |= DepthTextureMode.Depth;
		}

		void OnRenderImage (RenderTexture src, RenderTexture dest) {
			if (material != null) {
				Matrix4x4 frustumCorners = Matrix4x4.identity;

				float fov = camera.fieldOfView;
				float near = camera.nearClipPlane;
				float far = camera.farClipPlane;
				float aspect = camera.aspect;

				float halfHeight = near * Mathf.Tan (fov * 0.5f * Mathf.Deg2Rad);
				Vector3 toRight = cameraTransform.right * halfHeight * aspect;
				Vector3 toTop = halfHeight * cameraTransform.up;

				Vector3 topLeft = toTop - toRight + cameraTransform.forward * near;
				float scale = topLeft.magnitude / near;
				topLeft.Normalize ();
				topLeft *= scale;

				Vector3 topRight = toTop + toRight + cameraTransform.forward * near;
				topRight.Normalize ();
				topRight *= scale;

				Vector3 bottomLeft = -toTop - toRight + cameraTransform.forward * near;
				bottomLeft.Normalize ();
				bottomLeft *= scale;

				Vector3 bottomRight = -toTop + toRight + cameraTransform.forward * near;
				bottomRight.Normalize ();
				bottomRight *= scale;

				frustumCorners.SetRow (0, bottomLeft);
				frustumCorners.SetRow (1, bottomRight);
				frustumCorners.SetRow (2, topRight);
				frustumCorners.SetRow (3, topLeft);

				material.SetMatrix ("_FrustumCornersRay", frustumCorners);
				material.SetMatrix ("_ViewProjectionInverseMatrix", (camera.projectionMatrix * camera.worldToCameraMatrix).inverse);

				material.SetFloat ("_FogDensity", fogDensity);
				material.SetColor ("_FogColor", fogColor);
				material.SetFloat ("_FogStart", fogStart);
				material.SetFloat ("_FogEnd", fogEnd);

				Graphics.Blit (src, dest, material);
			} else {
				Graphics.Blit (src, dest);
			}
		}
	}
}
