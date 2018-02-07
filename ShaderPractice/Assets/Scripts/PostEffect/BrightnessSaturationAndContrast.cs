using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ShaderPractice {
	public class BrightnessSaturationAndContrast : PostEffectsBase {
		public Shader briSatConShader;
		private Material briSatConMaterial;
		public Material material {
			get {
				briSatConMaterial = CheckShaderAndCreateMaterial (briSatConShader, briSatConMaterial);
				return briSatConMaterial;
			}
		}

		[Range(0f, 3f)]
		public float brightness = 1f;

		[Range(0f, 3f)]
		public float satutation = 1f;

		[Range(0f, 3f)]
		public float contrast = 1f;

		void OnRenderImage (RenderTexture src, RenderTexture dest) {
			if (material != null) {
				material.SetFloat ("_Brightness", brightness);
				material.SetFloat ("_Saturation", satutation);
				material.SetFloat ("_Contrast", contrast);

				Graphics.Blit (src, dest, material);
			} else {
				Graphics.Blit (src, dest);
			}
		}
	}
}
