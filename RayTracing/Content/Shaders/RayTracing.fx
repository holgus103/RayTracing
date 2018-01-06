#if OPENGL
#define SV_POSITION POSITION
#define VS_SHADERMODEL vs_4_0
#define PS_SHADERMODEL ps_4_0
#else
#define VS_SHADERMODEL vs_4_0_level_9_3
#define PS_SHADERMODEL ps_4_0_level_9_3
#endif


matrix World;
matrix Projection;
matrix View;
float3 CameraPosition : register(c0);
static const float PI = 3.14159265f;
struct VertexShaderInputTx
{
	float4 Position : POSITION0;
	float3 Normal : NORMAL;
};

struct VertexShaderOutput
{
	float4 Position : SV_POSITION;
	float3 WorldPosition : TEXCOORD0;
	float3 Normal : NORMAL0;
};

VertexShaderOutput MainVS(in VertexShaderInputTx input)
{
	VertexShaderOutput output = (VertexShaderOutput)0;

	float4 worldPosition = mul(input.Position, World);
	float4 viewPosition = mul(worldPosition, View);
	output.Position = mul(viewPosition, Projection);
	output.WorldPosition = output.Position;
	output.Normal = input.Normal;

	return output;
}


float getDelta(float a, float b, float c)
{
	return b*b - 4 * a*c;
}

void findIntersection(float3 cam, float3 d, out float t1, out float t2)
{
	float b = 2 * cam.x * d.x + 2 * cam.y * d.y + 2 * cam.z * d.z;
	float a = d.x * d.x + d.y * d.y + d.z * d.z;
	float c = cam.x * cam.x + cam.y * cam.y + cam.z * cam.z - 4;
	float delta = getDelta(a, b, c);
	if (delta >= 0)
	{
		float sd = sqrt(delta);
		t1 = (-b + sd) / a;
		t2 = (-b - sd) / a;
		if (t1 > t2)
		{
			float3 temp = t2;
			t2 = t1;
			t1 = temp;
		}
	}
	else
	{
		discard;
	}
}

bool belongs(float z0, float t1, float t2)
{
	return ((z0 - t2)*(z0 - t1) <= 0);
}

void findQubicSolution(float3 cam, float3 d, float t1, float t2, out float z) 
{
	float a3 = 4 * d.x * d.x * d.x 
		- 4 * d.z * d.z * d.z 
		- 12 * d.x * d.y * d.y 
		+ 3 * d.x * d.x * d.z 
		+ 3 * d.y * d.y * d.z;

	float a2 = 12 * cam.x * d.x * d.x
		- 21 * cam.z * d.z * d.z
		- 12 * d.y * d.y * cam.x
		- 24 * d.x * d.y * cam.y
		+ 3 * d.x * d.x * cam.z
		+ 6 * cam.x * d.x * d.z
		+ 3 * d.y * d.y * cam.z
		+ 6 * d.y * d.z * cam.y
		- 6 * d.x * d.x
		- 6 * d.y * d.y
		- 4 * d.z * d.z;

	float a1 = 12 * cam.x * cam.x * d.x
		- 12 * cam.z * cam.z * d.z
		- 24 * d.y * cam.y * cam.x
		- 12 * d.x * cam.y * cam.y
		+ 6 * cam.x * d.x * cam.z
		+ 3 * cam.x * cam.x * d.z
		+ 6 * d.y * cam.y * cam.z
		+ 3 * cam.y * cam.y * d.z
		- 12 * d.x
		- 12 * d.y
		- 8 * d.z * cam.z
		- 3 * d.z;

	float a0 = 4 * cam.x * cam.x * cam.x
		- 7 * cam.z * cam.z * cam.z
		- 12 * cam.y * cam.y * cam.x
		+ 3 * cam.x * cam.x * cam.z
		+ 3 * cam.y * cam.y * cam.z
		- 6 * cam.x * cam.x
		- 6 * cam.y * cam.y
		- 4 * cam.z * cam.z
		- 3 * cam.z
		- 2;

	float b2 = a2 / a3;
	float b1 = a1 / a3;
	float b0 = a0 / a3;
	float Q = (3 * b1 - b2 * b2) / 9;
	float R = (9 * b1 * b2 - 27 * b0 - 2 * b2 * b2 * b2) / 54;
	float D = Q * Q * Q + R * R;
	if (D < 0)
	{
		float theta = acos(R / sqrt(-Q * Q * Q));
		float z0 = 2 * sqrt(-Q) * cos(theta / 3) - (b2 * b2) / 3;
		float z1 = 2 * sqrt(-Q) * cos(theta / 3 + 2 * PI / 3) - (b2 * b2) / 3;
		float z2 = 2 * sqrt(-Q) * cos(theta / 3 + 4 * PI / 3) - (b2 * b2) / 3;

		// really embarassing sorting
		if (z0 > z1)
		{
			float tmp = z0;
			z0 = z1;
			z1 = tmp;
		}

		if (z1 > z2)
		{
			float tmp = z1;
			z1 = z2;
			z2 = tmp;
		}

		if (z0 > z1)
		{
			float tmp = z0;
			z0 = z1;
			z1 = tmp;
		}

		if (belongs(z0, t1, t2))
		{
			z = z0;
		}

		if (belongs(z1, t1, t2))
		{
			z = z1;
		}

		if (belongs(z2, t1, t2))
		{
			z = z2;
		}
		discard;

	}
	else
	{
		float bp = R + sqrt(D);
		float bm = R - sqrt(D);
		float rtp;
		float rtm;
		if (bp < 0)
			rtp = -pow(-bp, 1 / 3);
		else
			rtp = pow(bp, 1 / 3);

		if (bm < 0)
			rtm = -pow(-bm, 1 / 3);
		else
			rtm = pow(bm, 1 / 3);
		float z0 = rtp + rtm - b2 / 3;
		if (belongs(z0, t1, t2))
		{
			z = z0;
		}
		else
		{
			discard;
		}
	}

}

float3 getNormal(float3 p)
{
	float dx = 6 * (2 * p.x * p.x + 2 * p.x - 2 * p.y * p.y + p.x * p.z);
	float dy = 6 * (-4 * p.x * p.y - 2 * p.y + p.y * p.z);
	float dz = 3 * p.x * p.x + 3 * p.y * p.y - 21 * p.z * p.z - 8 * p.z - 3;
	return float3(dx, dy, dz);
}

float4 MainPS(VertexShaderOutput input) : COLOR
{
	float3 d = input.WorldPosition - CameraPosition;
	float t1;
	float t2;
	findIntersection(CameraPosition, d, t1, t2);
	float t;
	findQubicSolution(CameraPosition, d, t1, t2, t);
	float3 p = CameraPosition + d * t;
	float3 v = normalize(-d);
	float3 n = getNormal(p);

	return float4(1, 0, 0, 1) * (0.1 + 0.4 * abs(dot(n, v)) + 0.5 * pow(abs(dot(n, v)), 20));
}

technique BasicColorDrawing
{
	pass P0
	{
		VertexShader = compile VS_SHADERMODEL MainVS();
		PixelShader = compile PS_SHADERMODEL MainPS();
	}
};