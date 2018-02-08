using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ShaderPractice {
	public class MotionBlurWithDepthTexture : PostEffectsBase {
		public Shader motionBlurShader;
		private Material motionBlurMaterial;
		public Material material {
			get {
				motionBlurMaterial = CheckShaderAndCreateMaterial (motionBlurShader, motionBlurMaterial);
				return motionBlurMaterial;
			}
		}

		[Range (0f, 1f)]
		public float BlurSize = 0.5f;

		private Camera myCamera;
		public Camera camera {
			get {
				if (myCamera == null) {
					myCamera = GetComponent<Camera> ();
				}
				return myCamera;
			}
		}

		private Matrix4x4 previousViewProjectionMatrix;

		void OnEnable () {
			camera.depthTextureMode |= DepthTextureMode.Depth;
			previousViewProjectionMatrix = camera.projectionMatrix* camera.worldToCameraMatrix;
		}

		void OnRenderImage (RenderTexture src, RenderTexture dest) {
			if (material != null) {
				material.SetFloat ("_BlurSize", BlurSize);

				material.SetMatrix("_PreviousViewProjectionMatrix", previousViewProjectionMatrix);
				Matrix4x4 currentViewProjectionMatrix = camera.projectionMatrix * camera.worldToCameraMatrix;
				Matrix4x4 currentViewProjectionInverseMatrix = currentViewProjectionMatrix.inverse;
				material.SetMatrix("_CurrentViewProjectionInverseMatrix", currentViewProjectionInverseMatrix);
				previousViewProjectionMatrix = currentViewProjectionMatrix;

				Graphics.Blit (src, dest, material);
			} else {
				Graphics.Blit (src, dest);
			}
		}
	}
}
