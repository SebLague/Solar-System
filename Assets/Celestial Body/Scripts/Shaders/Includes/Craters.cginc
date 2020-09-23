#include "/Math.cginc"

struct Crater {
    float3 centre;
    float radius;
    float floor;
	 float smoothness;
};

// Crater data and settings
StructuredBuffer<Crater> craters;
int numCraters;
float floorHeight;
float rimSteepness;
float rimWidth;
float smoothFactor;

float calculateCraterDepth(float3 vertexPos) {
	float craterHeight = 0;

	for (int i = 0; i < numCraters; i ++) {
		float x = length(vertexPos - craters[i].centre) / max(craters[i].radius, 0.0001);

		float cavity = x * x - 1;
		float rimX = min(x - 1 - rimWidth, 0);
		float rim = rimSteepness * rimX * rimX;

		float craterShape = smoothMax(cavity, craters[i].floor, craters[i].smoothness);
		craterShape = smoothMin(craterShape, rim, craters[i].smoothness);
		craterHeight += craterShape * craters[i].radius;
	}
	return craterHeight;
}