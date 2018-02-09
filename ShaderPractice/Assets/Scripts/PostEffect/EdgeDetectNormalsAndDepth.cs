using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ShaderPractice {
	public class EdgeDetectNormalsAndDepth : PostEffectsBase {
		public Shader edgeDetectShader;
		private Material edgeDetectMaterial;
		public Material material {
			get {
				edgeDetectMaterial = CheckShaderAndCreateMaterial (edgeDetectShader, edgeDetectMaterial);
				return edgeDetectMaterial;
			}
		}

		[Range (0f, 1f)]
		public float edgesOnly = 0f;

		public Color edgeColor = Color.black;

		public Color backgroundColor = Color.white;

		public float sampleDistance = 1f;

		public float sensitivityDepth = 1f;

		public float sensituvityNormals = 1f;

		void OnEnable () {
			GetComponent<Camera> ().depthTextureMode |= DepthTextureMode.DepthNormals;
		}

		[ImageEffectOpaque]
		void OnRenderImage (RenderTexture src, RenderTexture dest) {
			if (material != null) {
				material.SetFloat ("_EdgeOnly", edgesOnly);
				material.SetColor ("_EdgeColor", edgeColor);
				material.SetColor ("_BackgroundColor", backgroundColor);
				material.SetFloat ("_SampleDistance", sampleDistance);
				material.SetVector ("_Sensitivity", new Vector4 (sensituvityNormals, sensitivityDepth, 0f, 0f));

				Graphics.Blit (src, dest, material);
			} else {
				Graphics.Blit (src, dest);
			}
		}
	}
}
