using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.Experimental.Rendering;

public static class ComputeHelper {

	// Subscribe to this event to be notified when buffers created in edit mode should be released
	// (i.e before script compilation occurs, and when exitting edit mode)
	public static event System.Action shouldReleaseEditModeBuffers;

	// Convenience method for dispatching a compute shader.
	// It calculates the number of thread groups based on the number of iterations needed.
	public static void Run (ComputeShader cs, int numIterationsX, int numIterationsY = 1, int numIterationsZ = 1, int kernelIndex = 0) {
		Vector3Int threadGroupSizes = GetThreadGroupSizes (cs, kernelIndex);
		int numGroupsX = Mathf.CeilToInt (numIterationsX / (float) threadGroupSizes.x);
		int numGroupsY = Mathf.CeilToInt (numIterationsY / (float) threadGroupSizes.y);
		int numGroupsZ = Mathf.CeilToInt (numIterationsZ / (float) threadGroupSizes.y);
		cs.Dispatch (kernelIndex, numGroupsX, numGroupsY, numGroupsZ);
	}

	// Only run compute shaders if this is true
	// This is only relevant for compute shaders that run outside of playmode
	public static bool CanRunEditModeCompute {
		get {
			return CheckIfCanRunInEditMode ();
		}
	}

	// Set all values from settings object on the shader. Note, variable names must be an exact match in the shader.
	// Settings object can be any class/struct containing vectors/ints/floats/bools
	public static void SetParams (System.Object settings, ComputeShader shader, string variableNamePrefix = "", string variableNameSuffix = "") {
		var fields = settings.GetType ().GetFields ();
		foreach (var field in fields) {
			var fieldType = field.FieldType;
			string shaderVariableName = variableNamePrefix + field.Name + variableNameSuffix;

			if (fieldType == typeof (UnityEngine.Vector4) || fieldType == typeof (Vector3) || fieldType == typeof (Vector2)) {
				shader.SetVector (shaderVariableName, (Vector4) field.GetValue (settings));
			} else if (fieldType == typeof (int)) {
				shader.SetInt (shaderVariableName, (int) field.GetValue (settings));
			} else if (fieldType == typeof (float)) {
				shader.SetFloat (shaderVariableName, (float) field.GetValue (settings));
			} else if (fieldType == typeof (bool)) {
				shader.SetBool (shaderVariableName, (bool) field.GetValue (settings));
			} else {
				Debug.Log ($"Type {fieldType} not implemented");
			}
		}
	}

	public static void CreateStructuredBuffer<T> (ref ComputeBuffer buffer, int count) {
		int stride = System.Runtime.InteropServices.Marshal.SizeOf (typeof (T));
		bool createNewBuffer = buffer == null || !buffer.IsValid () || buffer.count != count || buffer.stride != stride;
		if (createNewBuffer) {
			Release (buffer);
			buffer = new ComputeBuffer (count, stride);
		}
	}

	public static void CreateStructuredBuffer<T> (ref ComputeBuffer buffer, T[] data) {
		CreateStructuredBuffer<T> (ref buffer, data.Length);
		buffer.SetData (data);
	}

	// Test

	public static ComputeBuffer CreateAndSetBuffer<T> (T[] data, ComputeShader cs, string nameID, int kernelIndex = 0) {
		ComputeBuffer buffer = null;
		CreateAndSetBuffer<T> (ref buffer, data, cs, nameID, kernelIndex);
		return buffer;
	}

	public static void CreateAndSetBuffer<T> (ref ComputeBuffer buffer, T[] data, ComputeShader cs, string nameID, int kernelIndex = 0) {
		int stride = System.Runtime.InteropServices.Marshal.SizeOf (typeof (T));
		CreateStructuredBuffer<T> (ref buffer, data.Length);
		buffer.SetData (data);
		cs.SetBuffer (kernelIndex, nameID, buffer);
	}

	public static ComputeBuffer CreateAndSetBuffer<T> (int length, ComputeShader cs, string nameID, int kernelIndex = 0) {
		ComputeBuffer buffer = null;
		CreateAndSetBuffer<T> (ref buffer, length, cs, nameID, kernelIndex);
		return buffer;
	}

	public static void CreateAndSetBuffer<T> (ref ComputeBuffer buffer, int length, ComputeShader cs, string nameID, int kernelIndex = 0) {
		CreateStructuredBuffer<T> (ref buffer, length);
		cs.SetBuffer (kernelIndex, nameID, buffer);
	}

	// Releases supplied buffer/s if not null
	public static void Release (params ComputeBuffer[] buffers) {
		for (int i = 0; i < buffers.Length; i++) {
			if (buffers[i] != null) {
				buffers[i].Release ();
			}
		}
	}

	public static Vector3Int GetThreadGroupSizes (ComputeShader compute, int kernelIndex = 0) {
		uint x, y, z;
		compute.GetKernelThreadGroupSizes (kernelIndex, out x, out y, out z);
		return new Vector3Int ((int) x, (int) y, (int) z);
	}

	public static void CreateRenderTexture (ref RenderTexture texture, int size, FilterMode filterMode = FilterMode.Bilinear, GraphicsFormat format = GraphicsFormat.R16G16B16A16_SFloat) {
		CreateRenderTexture (ref texture, size, size, filterMode, format);
	}

	public static void CreateRenderTexture (ref RenderTexture texture, int width, int height, FilterMode filterMode = FilterMode.Bilinear, GraphicsFormat format = GraphicsFormat.R16G16B16A16_SFloat) {
		if (texture == null || !texture.IsCreated () || texture.width != width || texture.height != height || texture.graphicsFormat != format) {
			if (texture != null) {
				texture.Release ();
			}
			texture = new RenderTexture (width, height, 0);
			texture.graphicsFormat = format;
			texture.enableRandomWrite = true;

			texture.autoGenerateMips = false;
			texture.Create ();
		}
		texture.wrapMode = TextureWrapMode.Clamp;
		texture.filterMode = filterMode;
	}

	// https://cmwdexint.com/2017/12/04/computeshader-setfloats/
	public static float[] PackFloats (params float[] values) {
		float[] packed = new float[values.Length * 4];
		for (int i = 0; i < values.Length; i++) {
			packed[i * 4] = values[i];
		}
		return values;
	}

	// Editor helpers:

#if UNITY_EDITOR
	static UnityEditor.PlayModeStateChange playModeState;

	static ComputeHelper () {
		// Monitor play mode state
		UnityEditor.EditorApplication.playModeStateChanged -= MonitorPlayModeState;
		UnityEditor.EditorApplication.playModeStateChanged += MonitorPlayModeState;
		// Monitor script compilation
		UnityEditor.Compilation.CompilationPipeline.compilationStarted -= OnCompilationStarted;
		UnityEditor.Compilation.CompilationPipeline.compilationStarted += OnCompilationStarted;
	}

	static void MonitorPlayModeState (UnityEditor.PlayModeStateChange state) {
		playModeState = state;
		if (state == UnityEditor.PlayModeStateChange.ExitingEditMode) {
			if (shouldReleaseEditModeBuffers != null) {
				shouldReleaseEditModeBuffers (); //
			}
		}
	}

	static void OnCompilationStarted (System.Object obj) {
		if (shouldReleaseEditModeBuffers != null) {
			shouldReleaseEditModeBuffers ();
		}
	}
#endif

	static bool CheckIfCanRunInEditMode () {
		bool isCompilingOrExitingEditMode = false;
#if UNITY_EDITOR
		isCompilingOrExitingEditMode |= UnityEditor.EditorApplication.isCompiling;
		isCompilingOrExitingEditMode |= playModeState == UnityEditor.PlayModeStateChange.ExitingEditMode;
#endif
		bool canRun = !isCompilingOrExitingEditMode;
		return canRun;
	}
}