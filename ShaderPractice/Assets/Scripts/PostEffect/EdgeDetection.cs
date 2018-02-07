using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ShaderPractice {
	public class EdgeDetection : PostEffectsBase {
		public Shader edgeDetectShader;
		private Material edgeDetectMaterial = null;
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

		void OnRenderImage (RenderTexture src, RenderTexture dest) {
			if (material != null) {
				material.SetFloat ("_EdgeOnly", edgesOnly);
				material.SetColor ("_EdgeColor", edgeColor);
				material.SetColor ("_BackgroundColor", backgroundColor);

				Graphics.Blit (src, dest, material);
			} else {
				Graphics.Blit (src, dest);
			}
		}
	}
}
