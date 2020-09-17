static const float PI = 3.14159265359;
static const float TAU = PI * 2;
static const float maxFloat = 3.402823466e+38;

// Remap a value from one range to another
float remap(float v, float minOld, float maxOld, float minNew, float maxNew) {
	 return saturate(minNew + (v-minOld) * (maxNew - minNew) / (maxOld-minOld));
}

// Remap the components of a vector from one range to another
float4 remap(float4 v, float minOld, float maxOld, float minNew, float maxNew) {
	 return saturate(minNew + (v-minOld) * (maxNew - minNew) / (maxOld-minOld));//
}

// Remap a float value (with a known mininum and maximum) to a value between 0 and 1
float remap01(float v, float minOld, float maxOld) {
	 return saturate((v-minOld) / (maxOld-minOld));
}

// Remap a float2 value (with a known mininum and maximum) to a value between 0 and 1
float2 remap01(float2 v, float2 minOld, float2 maxOld) {
	 return saturate((v-minOld) / (maxOld-minOld));
}

// Smooth minimum of two values, controlled by smoothing factor k
// When k = 0, this behaves identically to min(a, b)
float smoothMin(float a, float b, float k) {
	 k = max(0, k);
	 // https://www.iquilezles.org/www/articles/smin/smin.htm
	 float h = max(0, min(1, (b - a + k) / (2 * k)));
	 return a * h + b * (1 - h) - k * h * (1 - h);
}

// Smooth maximum of two values, controlled by smoothing factor k
// When k = 0, this behaves identically to max(a, b)
float smoothMax(float a, float b, float k) {
	 k = min(0, -k);
	 float h = max(0, min(1, (b - a + k) / (2 * k)));
	 return a * h + b * (1 - h) - k * h * (1 - h);
}

float Blend(float startHeight, float blendDst, float height) {
	 return smoothstep(startHeight - blendDst / 2, startHeight + blendDst / 2, height);
}


	// Returns vector (dstToSphere, dstThroughSphere)
	// If ray origin is inside sphere, dstToSphere = 0
	// If ray misses sphere, dstToSphere = maxValue; dstThroughSphere = 0
	float2 raySphere(float3 sphereCentre, float sphereRadius, float3 rayOrigin, float3 rayDir) {
		float3 offset = rayOrigin - sphereCentre;
		float a = 1; // Set to dot(rayDir, rayDir) if rayDir might not be normalized
		float b = 2 * dot(offset, rayDir);
		float c = dot (offset, offset) - sphereRadius * sphereRadius;
		float d = b * b - 4 * a * c; // Discriminant from quadratic formula

		// Number of intersections: 0 when d < 0; 1 when d = 0; 2 when d > 0
		if (d > 0) {
			float s = sqrt(d);
			float dstToSphereNear = max(0, (-b - s) / (2 * a));
			float dstToSphereFar = (-b + s) / (2 * a);

			// Ignore intersections that occur behind the ray
			if (dstToSphereFar >= 0) {
				return float2(dstToSphereNear, dstToSphereFar - dstToSphereNear);
			}
		}
		// Ray did not intersect sphere
		return float2(maxFloat, 0);
	}



