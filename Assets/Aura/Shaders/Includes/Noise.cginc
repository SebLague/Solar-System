/*
From : 
https://forum.unity3d.com/threads/2d-3d-4d-optimised-perlin-noise-cg-hlsl-library-cginc.218372/

------------------------------------------------------------------

Description:
Array- and textureless CgFx/HLSL 2D, 3D and 4D simplex noise functions.
a.k.a. simplified and optimized Perlin noise.

The functions have very good performance
and no dependencies on external data.

2D - Very fast, very compact code.
3D - Fast, compact code.
4D - Reasonably fast, reasonably compact code.

------------------------------------------------------------------

Ported by:
Lex-DRL
I've ported the code from GLSL to CgFx/HLSL for Unity,
added a couple more optimisations (to speed it up even further)
and slightly reformatted the code to make it more readable.

Original GLSL functions:
https://github.com/ashima/webgl-noise
Credits from original glsl file are at the end of this cginc.

------------------------------------------------------------------

Usage:

half ns = snoise(v);
// v is any of: half2, half3, half4

Return type is half.
To generate 2 or more components of noise (colorful noise),
call these functions several times with different
constant offsets for the arguments.
E.g.:

half3 colorNs = half3(
snoise(v),
snoise(v + 17.0),
snoise(v - 43.0),
);


Remark about those offsets from the original author:

People have different opinions on whether these offsets should be integers
for the classic noise functions to match the spacing of the zeroes,
so we have left that for you to decide for yourself.
For most applications, the exact offsets don't really matter as long
as they are not too small or too close to the noise lattice period
(289 in this implementation).

*/

// 1 / 289
#define NOISE_SIMPLEX_1_DIV_289 0.00346020761245674740484429065744f

half mod289(half x) {
	return x - floor(x * NOISE_SIMPLEX_1_DIV_289) * 289.0;
}

half2 mod289(half2 x) {
	return x - floor(x * NOISE_SIMPLEX_1_DIV_289) * 289.0;
}

half3 mod289(half3 x) {
	return x - floor(x * NOISE_SIMPLEX_1_DIV_289) * 289.0;
}

half4 mod289(half4 x) {
	return x - floor(x * NOISE_SIMPLEX_1_DIV_289) * 289.0;
}


// ( x*34.0 + 1.0 )*x =
// x*x*34.0 + x
half permute(half x) {
	return mod289(
		x*x*34.0 + x
	);
}

half3 permute(half3 x) {
	return mod289(
		x*x*34.0 + x
	);
}

half4 permute(half4 x) {
	return mod289(
		x*x*34.0 + x
	);
}



half taylorInvSqrt(half r) {
	return 1.79284291400159 - 0.85373472095314 * r;
}

half4 taylorInvSqrt(half4 r) {
	return 1.79284291400159 - 0.85373472095314 * r;
}



half4 grad4(half j, half4 ip)
{
	const half4 ones = half4(1.0, 1.0, 1.0, -1.0);
	half4 p;// , s;
	p.xyz = floor(frac(j * ip.xyz) * 7.0) * ip.z - 1.0;
	p.w = 1.5 - dot(abs(p.xyz), ones.xyz);

	// GLSL: lessThan(x, y) = x < y
	// HLSL: 1 - step(y, x) = x < y
	//s = half4(
	//	1 - step(0.0, p)
	//	);

	// Optimization hint Dolkar
	// p.xyz = p.xyz + (s.xyz * 2 - 1) * s.www;
	p.xyz -= sign(p.xyz) * (p.w < 0);

	return p;
}



// ----------------------------------- 2D -------------------------------------

half snoise(half2 v)
{
	const half4 C = half4(
		0.211324865405187, // (3.0-sqrt(3.0))/6.0
		0.366025403784439, // 0.5*(sqrt(3.0)-1.0)
		-0.577350269189626, // -1.0 + 2.0 * C.x
		0.024390243902439  // 1.0 / 41.0
		);

	// First corner
	half2 i = floor(v + dot(v, C.yy));
	half2 x0 = v - i + dot(i, C.xx);

	// Other corners
	// half2 i1 = (x0.x > x0.y) ? half2(1.0, 0.0) : half2(0.0, 1.0);
	// Lex-DRL: afaik, step() in GPU is faster than if(), so:
	// step(x, y) = x <= y

	// Optimization hint Dolkar
	//int xLessEqual = step(x0.x, x0.y); // x <= y ?
	//int2 i1 =
	//    int2(1, 0) * (1 - xLessEqual) // x > y
	//    + int2(0, 1) * xLessEqual // x <= y
	//;
	//half4 x12 = x0.xyxy + C.xxzz;
	//x12.xy -= i1;

	half4 x12 = x0.xyxy + C.xxzz;
	int2 i1 = (x0.x > x0.y) ? half2(1.0, 0.0) : half2(0.0, 1.0);
	x12.xy -= i1;

	// Permutations
	i = mod289(i); // Avoid truncation effects in permutation
	half3 p = permute(
		permute(
			i.y + half3(0.0, i1.y, 1.0)
		) + i.x + half3(0.0, i1.x, 1.0)
	);

	half3 m = max(
		0.5 - half3(
			dot(x0, x0),
			dot(x12.xy, x12.xy),
			dot(x12.zw, x12.zw)
			),
		0.0
	);
	m = m*m;
	m = m*m;

	// Gradients: 41 points uniformly over a line, mapped onto a diamond.
	// The ring size 17*17 = 289 is close to a multiple of 41 (41*7 = 287)

	half3 x = 2.0 * frac(p * C.www) - 1.0;
	half3 h = abs(x) - 0.5;
	half3 ox = floor(x + 0.5);
	half3 a0 = x - ox;

	// Normalise gradients implicitly by scaling m
	// Approximation of: m *= inversesqrt( a0*a0 + h*h );
	m *= 1.79284291400159 - 0.85373472095314 * (a0*a0 + h*h);

	// Compute final noise value at P
	half3 g;
	g.x = a0.x * x0.x + h.x * x0.y;
	g.yz = a0.yz * x12.xz + h.yz * x12.yw;
	return 130.0 * dot(m, g);
}

// ----------------------------------- 3D -------------------------------------

half snoise(half3 v)
{
	const half2 C = half2(
		0.166666666666666667, // 1/6
		0.333333333333333333  // 1/3
		);
	const half4 D = half4(0.0, 0.5, 1.0, 2.0);

	// First corner
	half3 i = floor(v + dot(v, C.yyy));
	half3 x0 = v - i + dot(i, C.xxx);

	// Other corners
	half3 g = step(x0.yzx, x0.xyz);
	half3 l = 1 - g;
	half3 i1 = min(g.xyz, l.zxy);
	half3 i2 = max(g.xyz, l.zxy);

	half3 x1 = x0 - i1 + C.xxx;
	half3 x2 = x0 - i2 + C.yyy; // 2.0*C.x = 1/3 = C.y
	half3 x3 = x0 - D.yyy;      // -1.0+3.0*C.x = -0.5 = -D.y

								 // Permutations
	i = mod289(i);
	half4 p = permute(
		permute(
			permute(
				i.z + half4(0.0, i1.z, i2.z, 1.0)
			) + i.y + half4(0.0, i1.y, i2.y, 1.0)
		) + i.x + half4(0.0, i1.x, i2.x, 1.0)
	);

	// Gradients: 7x7 points over a square, mapped onto an octahedron.
	// The ring size 17*17 = 289 is close to a multiple of 49 (49*6 = 294)
	half n_ = 0.142857142857; // 1/7
	half3 ns = n_ * D.wyz - D.xzx;

	half4 j = p - 49.0 * floor(p * ns.z * ns.z); // mod(p,7*7)

	half4 x_ = floor(j * ns.z);
	half4 y_ = floor(j - 7.0 * x_); // mod(j,N)

	half4 x = x_ *ns.x + ns.yyyy;
	half4 y = y_ *ns.x + ns.yyyy;
	half4 h = 1.0 - abs(x) - abs(y);

	half4 b0 = half4(x.xy, y.xy);
	half4 b1 = half4(x.zw, y.zw);

	//half4 s0 = half4(lessThan(b0,0.0))*2.0 - 1.0;
	//half4 s1 = half4(lessThan(b1,0.0))*2.0 - 1.0;
	half4 s0 = floor(b0)*2.0 + 1.0;
	half4 s1 = floor(b1)*2.0 + 1.0;
	half4 sh = -step(h, 0.0);

	half4 a0 = b0.xzyw + s0.xzyw*sh.xxyy;
	half4 a1 = b1.xzyw + s1.xzyw*sh.zzww;

	half3 p0 = half3(a0.xy, h.x);
	half3 p1 = half3(a0.zw, h.y);
	half3 p2 = half3(a1.xy, h.z);
	half3 p3 = half3(a1.zw, h.w);

	//Normalise gradients
	half4 norm = taylorInvSqrt(half4(
		dot(p0, p0),
		dot(p1, p1),
		dot(p2, p2),
		dot(p3, p3)
		));
	p0 *= norm.x;
	p1 *= norm.y;
	p2 *= norm.z;
	p3 *= norm.w;

	// Mix final noise value
	half4 m = max(
		0.6 - half4(
			dot(x0, x0),
			dot(x1, x1),
			dot(x2, x2),
			dot(x3, x3)
			),
		0.0
	);
	m = m * m;
	return 42.0 * dot(
		m*m,
		half4(
			dot(p0, x0),
			dot(p1, x1),
			dot(p2, x2),
			dot(p3, x3)
			)
	);
}

// ----------------------------------- 4D -------------------------------------

half snoise(half4 v)
{
	const half4 C = half4(
		0.138196601125011, // (5 - sqrt(5))/20 G4
		0.276393202250021, // 2 * G4
		0.414589803375032, // 3 * G4
		-0.447213595499958  // -1 + 4 * G4
		);

	// First corner
	half4 i = floor(
		v +
		dot(
			v,
			0.309016994374947451 // (sqrt(5) - 1) / 4
		)
	);
	half4 x0 = v - i + dot(i, C.xxxx);

	// Other corners

	// Rank sorting originally contributed by Bill Licea-Kane, AMD (formerly ATI)
	half4 i0;
	half3 isX = step(x0.yzw, x0.xxx);
	half3 isYZ = step(x0.zww, x0.yyz);
	i0.x = isX.x + isX.y + isX.z;
	i0.yzw = 1.0 - isX;
	i0.y += isYZ.x + isYZ.y;
	i0.zw += 1.0 - isYZ.xy;
	i0.z += isYZ.z;
	i0.w += 1.0 - isYZ.z;

	// i0 now contains the unique values 0,1,2,3 in each channel
	half4 i3 = saturate(i0);
	half4 i2 = saturate(i0 - 1.0);
	half4 i1 = saturate(i0 - 2.0);

	//    x0 = x0 - 0.0 + 0.0 * C.xxxx
	//    x1 = x0 - i1  + 1.0 * C.xxxx
	//    x2 = x0 - i2  + 2.0 * C.xxxx
	//    x3 = x0 - i3  + 3.0 * C.xxxx
	//    x4 = x0 - 1.0 + 4.0 * C.xxxx
	half4 x1 = x0 - i1 + C.xxxx;
	half4 x2 = x0 - i2 + C.yyyy;
	half4 x3 = x0 - i3 + C.zzzz;
	half4 x4 = x0 + C.wwww;

	// Permutations
	i = mod289(i);
	half j0 = permute(
		permute(
			permute(
				permute(i.w) + i.z
			) + i.y
		) + i.x
	);
	half4 j1 = permute(
		permute(
			permute(
				permute(
					i.w + half4(i1.w, i2.w, i3.w, 1.0)
				) + i.z + half4(i1.z, i2.z, i3.z, 1.0)
			) + i.y + half4(i1.y, i2.y, i3.y, 1.0)
		) + i.x + half4(i1.x, i2.x, i3.x, 1.0)
	);

	// Gradients: 7x7x6 points over a cube, mapped onto a 4-cross polytope
	// 7*7*6 = 294, which is close to the ring size 17*17 = 289.
	const half4 ip = half4(
		0.003401360544217687075, // 1/294
		0.020408163265306122449, // 1/49
		0.142857142857142857143, // 1/7
		0.0
		);

	half4 p0 = grad4(j0, ip);
	half4 p1 = grad4(j1.x, ip);
	half4 p2 = grad4(j1.y, ip);
	half4 p3 = grad4(j1.z, ip);
	half4 p4 = grad4(j1.w, ip);

	// Normalise gradients
	half4 norm = taylorInvSqrt(half4(
		dot(p0, p0),
		dot(p1, p1),
		dot(p2, p2),
		dot(p3, p3)
		));
	p0 *= norm.x;
	p1 *= norm.y;
	p2 *= norm.z;
	p3 *= norm.w;
	p4 *= taylorInvSqrt(dot(p4, p4));

	// Mix contributions from the five corners
	half3 m0 = max(
		0.6 - half3(
			dot(x0, x0),
			dot(x1, x1),
			dot(x2, x2)
			),
		0.0
	);
	half2 m1 = max(
		0.6 - half2(
			dot(x3, x3),
			dot(x4, x4)
			),
		0.0
	);
	m0 = m0 * m0;
	m1 = m1 * m1;

	return 49.0 * (
		dot(
			m0*m0,
			half3(
				dot(p0, x0),
				dot(p1, x1),
				dot(p2, x2)
				)
		) + dot(
			m1*m1,
			half2(
				dot(p3, x3),
				dot(p4, x4)
				)
		)
		);
}



//                 Credits from source glsl file:
//
// Description : Array and textureless GLSL 2D/3D/4D simplex
//               noise functions.
//      Author : Ian McEwan, Ashima Arts.
//  Maintainer : ijm
//     Lastmod : 20110822 (ijm)
//     License : Copyright (C) 2011 Ashima Arts. All rights reserved.
//               Distributed under the MIT License         . See LICENSE file.
//               https://github.com/ashima/webgl-noise
//
//
//           The text from LICENSE file:
//
//
// Copyright (C) 2011 by Ashima Arts (Simplex noise)
// Copyright (C) 2011 by Stefan Gustavson (Classic noise)
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions: 
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
