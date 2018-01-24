using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ShaderPractice {
	[ExecuteInEditMode]
	public class ProceduralTextureGeneration : MonoBehaviour {

		public Material material = null;
		#region material properties
		[SerializeField, SetProperty("textureWidth")]
		private int m_textureWidth = 512;
		public int textureWidth {
			get {
				return m_textureWidth;
			}
			private set {
				m_textureWidth = value;
				_UpdateMaterial ();
			}
		}

		[SerializeField, SetProperty("backgroundColor")]
		private Color m_backgroundColor = Color.white;
		public Color backgroundColor {
			get {
				return m_backgroundColor;
			}
			private set {
				m_backgroundColor = value;
				_UpdateMaterial ();
			}
		}

		[SerializeField, SetProperty ("circleColor")]
		private Color m_circleColor = Color.white;
		public Color circleColor {
			get {
				return m_circleColor;
			}
			private set {
				m_circleColor = value;
				_UpdateMaterial ();
			}
		}

		[SerializeField, SetProperty ("blurFactor")]
		private float m_blurFactor = 2.0f;
		public float blurFactor {
			get {
				return m_blurFactor;
			}
			set {
				m_blurFactor = value;
				_UpdateMaterial ();
			}
		}
		#endregion

		private Texture2D m_generatedTexture = null;

		// Use this for initialization
		void Start () {
			if (material == null) {
				Renderer render = gameObject.GetComponent<Renderer> ();
				if (render == null) {
					Debug.LogWarning ("Cannot find a Renderer");
					return;
				}
				material = render.sharedMaterial;
			}

			_UpdateMaterial ();
		}

		Color _MixColor (Color color0, Color color1, float mixFactor)
		{
			Color mixColor = Color.white;
			mixColor.r = Mathf.Lerp (color0.r, color1.r, mixFactor);
			mixColor.g = Mathf.Lerp (color0.g, color1.g, mixFactor);
			mixColor.b = Mathf.Lerp (color0.b, color1.b, mixFactor);
			mixColor.a = Mathf.Lerp (color0.a, color1.a, mixFactor);
			return mixColor;
		}

		void _UpdateMaterial () {
			if (material != null) {
				m_generatedTexture = _GenerateProceduralTexture ();
				material.SetTexture ("_MainTex", m_generatedTexture);
			}
		}

		Texture2D _GenerateProceduralTexture () {
			Texture2D generated = new Texture2D (textureWidth, textureWidth);

			float circleInterval = textureWidth / 4.0f;
			float radius = textureWidth / 10f;
			float edgeBlur = 1.0f / blurFactor;

			for (int w = 0; w < textureWidth; ++w) {
				for (int h = 0; h < textureWidth; ++h) {
					Color pixel = backgroundColor;

					for (int i = 0; i < 3; ++i) {
						for (int j = 0; j < 3; ++j) {
							Vector2 circleCenter = new Vector2 (circleInterval * (i + 1), circleInterval * (j + 1));
							float dist = Vector2.Distance (new Vector2 (w, h), circleCenter) - radius;

							Color color = _MixColor (circleColor, new Color (pixel.r, pixel.g, pixel.b, 0.0f), Mathf.SmoothStep (0f, 1.0f, dist * edgeBlur));

							pixel = _MixColor (pixel, color, color.a);
						}
					}

					generated.SetPixel (w, h, pixel);
				}
			}

			generated.Apply ();

			return generated;
		}
		
		// Update is called once per frame
		void Update () {
			
		}
	}
}
