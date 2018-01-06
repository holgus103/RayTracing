#if OPENGL
#define SV_POSITION POSITION
#define VS_SHADERMODEL vs_4_0
#define PS_SHADERMODEL ps_4_0
#else
#define VS_SHADERMODEL vs_4_0_level_9_1
#define PS_SHADERMODEL ps_4_0_level_9_1
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
	float3 Normal : NORMAL0;
};

VertexShaderOutput MainVS(in VertexShaderInputTx input)
{
	VertexShaderOutput output = (VertexShaderOutput)0;

	float4 worldPosition = mul(input.Position, World);
	float4 viewPosition = mul(worldPosition, View);
	output.Position = mul(viewPosition, Projection);
	output.Normal = input.Normal;

	return output;
}

float4 MainPS(VertexShaderOutput input) : COLOR
{
	float3 d = input.Position - CameraPosition;
	return float4(1, 0, 0, 1);
}

float getDelta(float a, float b, float c)
{
	return b*b - 4 * a*c;
}

float3 findIntersection(float3 cam, float3 d)
{
	float b = 2 * cam.x * d.x + 2 * cam.y * d.y + 2 * cam.z * d.z;
	float a = d.x * d.x + d.y * d.y + d.z * d.z;
	float c = cam.x * cam.x + cam.y * cam.y + cam.z * cam.z - 4;
	float delta = getDelta(a, b, c);
	if (delta >= 0)
	{
		float sd = sqrt(delta);
		float t1 = (-b + sd) / a;
		float t2 = (-b - sd) / a;
		if (t1 < t2)
		{
			return cam + d * t1;
		}
		else
		{
			return cam + d * t2;
		}
	}
	else
	{
		discard;
	}
}

float3 findQubicSolution(float3 cam, float3 d) 
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
		float3 z0 = 2 * sqrt(-Q) * cos(theta / 3) - (b2 * b2) / 3;
		float3 z1 = 2 * sqrt(-Q) * cos(theta / 3 + 2 * PI / 3) - (b2 * b2) / 3;
		float3 z2 = 2 * sqrt(-Q) * cos(theta / 3 + 4 * PI / 3) - (b2 * b2) / 3;
	}
	else
	{
		float3 z0 = pow(R + sqrt(D), 1 / 3) + pow(R - sqrt(D), 1 / 3) - b2 / 3;
	}

}

technique BasicColorDrawing
{
	pass P0
	{
		VertexShader = compile VS_SHADERMODEL MainVS();
		PixelShader = compile PS_SHADERMODEL MainPS();
	}
};