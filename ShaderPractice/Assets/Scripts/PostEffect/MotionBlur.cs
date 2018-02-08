using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ShaderPractice {
	public class MotionBlur : PostEffectsBase {
		public Shader motionBlurShader;
		private Material motionBlurMaterial = null;
		public Material material {
			get {
				motionBlurMaterial = CheckShaderAndCreateMaterial (motionBlurShader, motionBlurMaterial);
				return motionBlurMaterial;
			}
		}

		[Range(0f, 0.9f)]
		public float blurAmount = 0.7f;

		private RenderTexture accumulationTexture;

		void OnRenderImage (RenderTexture src, RenderTexture dest) {
			if (material != null) {
				if (accumulationTexture == null || accumulationTexture.width != src.width || accumulationTexture.height != src.height) {
					DestroyImmediate (accumulationTexture);
					accumulationTexture = new RenderTexture (src.width, src.height, 0);
					accumulationTexture.hideFlags = HideFlags.HideAndDontSave;
					Graphics.Blit (src, accumulationTexture);
				}

				//we are accumulating motion over frames without clear/discard
				//by design, so silence any performance warnings from Unity
				accumulationTexture.MarkRestoreExpected ();

				material.SetFloat ("_BlurAmount", 1f - blurAmount);

				Graphics.Blit (src, accumulationTexture, material);
				Graphics.Blit (accumulationTexture, dest);
			} else {
				Graphics.Blit (src, dest);
			}
		}

		void OnDisable () {
			DestroyImmediate (accumulationTexture);
		}
	}
}
