// Based on bloom effect by Keijiro Takahashi (see accompanying readme and license for details)
// The original can be found here: https://github.com/keijiro/KinoBloom/releases
// Minor modifications made

using UnityEngine;

[ExecuteInEditMode, ImageEffectAllowedInSceneView]
[CreateAssetMenu (menuName = "PostProcessing/Bloom")]
public class BloomEffect : PostProcessingEffect {

	/// Prefilter threshold (gamma-encoded)
	/// Filters out pixels under this level of brightness.
	public float thresholdGamma {
		get { return Mathf.Max (_threshold, 0); }
		set { _threshold = value; }
	}

	/// Prefilter threshold (linearly-encoded)
	/// Filters out pixels under this level of brightness.
	public float thresholdLinear {
		get { return GammaToLinear (thresholdGamma); }
		set { _threshold = LinearToGamma (value); }
	}

	[SerializeField]
	[Tooltip ("Filters out pixels under this level of brightness.")]
	float _threshold = 0.8f;

	/// Soft-knee coefficient
	/// Makes transition between under/over-threshold gradual.
	public float softKnee {
		get { return _softKnee; }
		set { _softKnee = value; }
	}

	[SerializeField, Range (0, 1)]
	[Tooltip ("Makes transition between under/over-threshold gradual.")]
	float _softKnee = 0.5f;

	/// Bloom radius
	/// Changes extent of veiling effects in a screen
	/// resolution-independent fashion.
	public float radius {
		get { return _radius; }
		set { _radius = value; }
	}

	[SerializeField, Range (1, 7)]
	[Tooltip ("Changes extent of veiling effects\n" +
		"in a screen resolution-independent fashion.")]
	float _radius = 2.5f;

	/// Bloom intensity
	/// Blend factor of the result image.
	public float intensity {
		get { return Mathf.Max (_intensity, 0); }
		set { _intensity = value; }
	}

	[SerializeField]
	[Tooltip ("Blend factor of the result image.")]
	float _intensity = 0.8f;

	/// High quality mode
	/// Controls filter quality and buffer resolution.
	public bool highQuality {
		get { return _highQuality; }
		set { _highQuality = value; }
	}

	[SerializeField]
	[Tooltip ("Controls filter quality and buffer resolution.")]
	bool _highQuality = true;

	/// Anti-flicker filter
	/// Reduces flashing noise with an additional filter.
	[SerializeField]
	[Tooltip ("Reduces flashing noise with an additional filter.")]
	bool _antiFlicker = true;

	public bool antiFlicker {
		get { return _antiFlicker; }
		set { _antiFlicker = value; }
	}

	[SerializeField, HideInInspector]
	Shader _shader;

	Material _material;

	const int kMaxIterations = 16;
	RenderTexture[] _blurBuffer1 = new RenderTexture[kMaxIterations];
	RenderTexture[] _blurBuffer2 = new RenderTexture[kMaxIterations];

	float LinearToGamma (float x) {
		return Mathf.LinearToGammaSpace (x);
	}

	float GammaToLinear (float x) {
		return Mathf.GammaToLinearSpace (x);
	}

	void OnEnable () {//
		_shader = null;
		var shader = _shader ? _shader : Shader.Find ("Hidden/Kino/Bloom");
		_material = new Material (shader);
		_material.hideFlags = HideFlags.DontSave;
	}

	void OnDisable () {
		DestroyImmediate (_material);
	}

	public override void Render (RenderTexture source, RenderTexture destination) {
		var useRGBM = Application.isMobilePlatform;

		// source texture size
		var tw = source.width;
		var th = source.height;

		// halve the texture size for the low quality mode
		if (!_highQuality) {
			tw /= 2;
			th /= 2;
		}

		// blur buffer format
		var rtFormat = useRGBM ?
			RenderTextureFormat.Default : RenderTextureFormat.DefaultHDR;

		// determine the iteration count
		var logh = Mathf.Log (th, 2) + _radius - 8;
		var logh_i = (int) logh;
		var iterations = Mathf.Clamp (logh_i, 1, kMaxIterations);

		// update the shader properties
		var lthresh = thresholdLinear;
		_material.SetFloat ("_Threshold", lthresh);

		var knee = lthresh * _softKnee + 1e-5f;
		var curve = new Vector3 (lthresh - knee, knee * 2, 0.25f / knee);
		_material.SetVector ("_Curve", curve);

		var pfo = !_highQuality && _antiFlicker;
		_material.SetFloat ("_PrefilterOffs", pfo ? -0.5f : 0.0f);

		_material.SetFloat ("_SampleScale", 0.5f + logh - logh_i);
		_material.SetFloat ("_Intensity", intensity);

		// prefilter pass
		var prefiltered = RenderTexture.GetTemporary (tw, th, 0, rtFormat);
		var pass = _antiFlicker ? 1 : 0;
		Graphics.Blit (source, prefiltered, _material, pass);

		// construct a mip pyramid
		var last = prefiltered;
		for (var level = 0; level < iterations; level++) {
			_blurBuffer1[level] = RenderTexture.GetTemporary (
				last.width / 2, last.height / 2, 0, rtFormat
			);

			pass = (level == 0) ? (_antiFlicker ? 3 : 2) : 4;
			Graphics.Blit (last, _blurBuffer1[level], _material, pass);

			last = _blurBuffer1[level];
		}

		// upsample and combine loop
		for (var level = iterations - 2; level >= 0; level--) {
			var basetex = _blurBuffer1[level];
			_material.SetTexture ("_BaseTex", basetex);

			_blurBuffer2[level] = RenderTexture.GetTemporary (
				basetex.width, basetex.height, 0, rtFormat
			);

			pass = _highQuality ? 6 : 5;
			Graphics.Blit (last, _blurBuffer2[level], _material, pass);
			last = _blurBuffer2[level];
		}

		// finish process
		_material.SetTexture ("_BaseTex", source);
		pass = _highQuality ? 8 : 7;
		Graphics.Blit (last, destination, _material, pass);

		// release the temporary buffers
		for (var i = 0; i < kMaxIterations; i++) {
			if (_blurBuffer1[i] != null)
				RenderTexture.ReleaseTemporary (_blurBuffer1[i]);

			if (_blurBuffer2[i] != null)
				RenderTexture.ReleaseTemporary (_blurBuffer2[i]);

			_blurBuffer1[i] = null;
			_blurBuffer2[i] = null;
		}

		RenderTexture.ReleaseTemporary (prefiltered);
	}

}