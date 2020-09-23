#include "/SimplexNoise.cginc"

float simpleNoise(float3 pos, int numLayers, float scale, float persistence, float lacunarity, float multiplier) {
	float noiseSum = 0;
	float amplitude = 1;
	float frequency = scale;
	for (int i = 0; i < numLayers; i ++) {
		noiseSum += snoise(pos * frequency) * amplitude;
		amplitude *= persistence;
		frequency *= lacunarity;
	}
	return noiseSum * multiplier;
}


float simpleNoise(float3 pos, float scale, float multiplier) {
	const int numLayers = 4;
	const float persistence = .5;
	const float lacunarity = 2;

	return simpleNoise(pos, numLayers, scale, persistence, lacunarity, multiplier);
}

float simpleNoise(float3 pos) {
	const float scale = 1;
	const float multiplier = 1;

	return simpleNoise(pos, scale, multiplier);
}





float simpleNoise(float3 pos, float4 params[3]) {
	// Extract parameters for readability
	float3 offset = params[0].xyz;
	int numLayers = int(params[0].w);
	float persistence = params[1].x;
	float lacunarity = params[1].y;
	float scale = params[1].z;
	float multiplier = params[1].w;
   float verticalShift = params[2].x;

	// Sum up noise layers
    float noiseSum = 0;
    float amplitude = 1;
    float frequency = scale;
    for (int i = 0; i < numLayers; i ++) {
        noiseSum += snoise(pos * frequency + offset) * amplitude;
        amplitude *= persistence;
        frequency *= lacunarity;
    }
    return noiseSum * multiplier + verticalShift;
}

// Simple noise, but with smaller values dampened
float dampenedSimpleNoise(float3 pos, float4 params[3]) {
	// Parameters
	float3 offset = params[0].xyz;
	int numLayers = int(params[0].w);
	float persistence = params[1].x;
	float lacunarity = params[1].y;
	float scale = params[1].z;
	float multiplier = params[1].w;
    float gain = 1.2;

	// Sum up noise layers
	float noiseSum = 0;
    float amplitude = 1;
    float frequency = scale;
    float weight = 1;

    for (int i = 0; i < numLayers; i ++) {
        float noiseVal = snoise(pos * frequency + offset) * .5 + .5;
        noiseVal *= weight;
        weight = saturate(noiseVal * gain);

        noiseSum += noiseVal * amplitude;
        amplitude *= persistence;
        frequency *= lacunarity;
    }
	return noiseSum * multiplier;
}

// Noise very roughly in range 0-1
float continentNoise(float3 pos, float4 params[3]) {
	// Parameters
	float3 offset = params[0].xyz;
	int numLayers = int(params[0].w);
	float persistence = params[1].x;
	float lacunarity = params[1].y;
	float scale = params[1].z;
	float multiplier = params[1].w;
    float gain = 1;

	// Sum up noise layers
	float noiseSum = 0;
    float amplitude = 1;
    float frequency = scale;
    float weight = 1;

    for (int i = 0; i < numLayers; i ++) {
        float noiseVal = snoise(pos * frequency + offset) * .5 + .5;
        noiseVal *= weight;
        weight = saturate(noiseVal * gain);

        noiseSum += noiseVal * amplitude;
        amplitude *= persistence;
        frequency *= lacunarity;
    }
	return noiseSum;
}

float simpleNoise2(float3 pos, float4 params[3]) {
	// Parameters
	float3 offset = params[0].xyz;
	int numLayers = int(params[0].w);
	float persistence = params[1].x;
	float lacunarity = params[1].y;
	float scale = params[1].z;
	float multiplier = params[1].w;

	// Sum up noise layers
    float noiseSum = 0;
    float amplitude = 1;
    float frequency = scale;
    for (int i = 0; i < numLayers; i ++) {
        float noiseVal = snoise(pos * frequency + offset);
        noiseSum += (noiseVal+1)*.5 * amplitude;
        amplitude *= persistence;
        frequency *= lacunarity;
    }
    return noiseSum;
}

float simpleNoiseMusgrave(float3 pos, float4 params[3], float o) {
	// Parameters
	float3 offset = params[0].xyz;
	int numLayers = int(params[0].w);
	float persistence = params[1].x;
	float lacunarity = params[1].y;
	float scale = params[1].z;
	float multiplier = params[1].w;

    // first octave
  
    float result = snoise(pos) + o;
    pos *= lacunarity;
    

	// Sum up noise layers
    float amplitude = 1;
    float frequency = scale * lacunarity;
    float weight = result;

    for (int i = 1; i < numLayers; i ++) {
        weight = min(1, weight);
        float signal = (snoise(pos * frequency) + o) * pow (abs(frequency), -persistence);
        result += weight * signal;
        weight *= signal;
        //amplitude *= persistence;
        frequency *= lacunarity;
        pos *= lacunarity;
    }

    return result * multiplier;
}

float ridgidNoise(float3 pos, float4 params[3]) {
	// Extract parameters for readability
	float3 offset = params[0].xyz;
	int numLayers = int(params[0].w);
	float persistence = params[1].x;
	float lacunarity = params[1].y;
	float scale = params[1].z;
	float multiplier = params[1].w;
	float power = params[2].x;
    float gain = params[2].y;
    float verticalShift = params[2].z;

	// Sum up noise layers
	float noiseSum = 0;
    float amplitude = 1;
    float frequency = scale;
    float ridgeWeight = 1;

    for (int i = 0; i < numLayers; i ++) {
        float noiseVal = 1 - abs(snoise(pos * frequency + offset));
        noiseVal = pow(abs(noiseVal), power);
        noiseVal *= ridgeWeight;
        ridgeWeight = saturate(noiseVal * gain);

        noiseSum += noiseVal * amplitude;
        amplitude *= persistence;
        frequency *= lacunarity;
    }
	return noiseSum * multiplier + verticalShift;
}

// Sample the noise several times at small offsets from the centre and average the result
// This reduces some of the harsh jaggedness that can occur
float smoothedRidgidNoise(float3 pos, float4 params[3]) {
    float3 sphereNormal = normalize(pos);
    float3 axisA = cross(sphereNormal, float3(0,1,0));
    float3 axisB = cross(sphereNormal, axisA);

    float offsetDst = params[2].w * 0.01;
    float sample0 = ridgidNoise(pos, params);
    float sample1 = ridgidNoise(pos - axisA * offsetDst, params);
    float sample2 = ridgidNoise(pos + axisA * offsetDst, params);
    float sample3 = ridgidNoise(pos - axisB * offsetDst, params);
    float sample4 = ridgidNoise(pos + axisB * offsetDst, params);
    return (sample0 + sample1 + sample2 + sample3 + sample4) / 5;
}

float ridgidNoise2(float3 pos, float4 params[3]) {
	// Parameters
	float3 offset = params[0].xyz;
	int numLayers = int(params[0].w);
	float persistence = params[1].x;
	float lacunarity = params[1].y;
	float scale = params[1].z;
	float multiplier = params[1].w;
	float power = params[2].x;

	// Sum up noise layers
	float noiseSum = 0;
    float amplitude = 1;
    float frequency = scale;
    float ridgeWeight = 1;

    for (int i = 0; i < numLayers; i ++) {
        float noiseVal = 1 - abs(snoise(pos * frequency + offset));
        noiseVal = (noiseVal * .5 + .5);
        noiseVal *= ridgeWeight;
        ridgeWeight = saturate(noiseVal);

        noiseSum += noiseVal * amplitude;
        amplitude *= persistence;
        frequency *= lacunarity;
    }
    noiseSum = 1 - abs(noiseSum*2-1);
    noiseSum = pow(abs(noiseSum), power);
	return noiseSum * multiplier;
}

float4 fractalNoiseGrad(float3 pos, int numLayers, float scale, float persistence, float lacunarity) {
    float4 noise = 0;
    float amplitude = 1;
    float frequency = scale;
    for (int i = 0; i < numLayers; i ++) {
        noise += snoise_grad(pos * frequency) * amplitude;
        amplitude *= persistence;
        frequency *= lacunarity;
    }
    return noise;
}