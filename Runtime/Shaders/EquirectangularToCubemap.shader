Shader "Hidden/EquirectangularToCubemap" {
    Properties {
        _MainTex ("EquirectangularTex", 2D) = "white" {}
        _FaceIndex ("Cube Face Index", Int) = 0
    }
    SubShader {
        Tags { "RenderType"="Opaque" }
        Cull Off
        ZWrite Off
        Pass {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            int _FaceIndex;

            struct appdata {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f {
                float4 pos : SV_POSITION;
                float2 uv  : TEXCOORD0;
            };

            v2f vert(appdata v) {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            // convert direction to equirect UV
            float2 DirToEquirectUV(float3 dir) {
                float longitude = atan2(dir.z, dir.x); // -PI..PI
                float latitude = asin(saturate(dir.y)); // -PI/2..PI/2
                float u = longitude / (2.0 * UNITY_PI) + 0.5;
                float v = 0.5 - (latitude / UNITY_PI);
                return float2(u, v);
            }

            // compute direction for given face and uv in [-1,1]
            float3 CubemapFaceDir(int face, float2 uv) {
                // uv.x/y in [-1,1]
                float x = uv.x;
                float y = uv.y;
                float3 dir;
                if (face == 0) { // +X
                    dir = float3(1.0, -y, -x);
                } else if (face == 1) { // -X
                    dir = float3(-1.0, -y, x);
                } else if (face == 2) { // +Y
                    dir = float3(x, 1.0, y);
                } else if (face == 3) { // -Y
                    dir = float3(x, -1.0, -y);
                } else if (face == 4) { // +Z
                    dir = float3(x, -y, 1.0);
                } else { // face == 5 // -Z
                    dir = float3(-x, -y, -1.0);
                }
                return normalize(dir);
            }

            fixed4 frag(v2f i) : SV_Target {
                // map uv [0,1] to [-1,1]
                float2 uv = i.uv * 2.0 - 1.0;
                // flip Y because screen UV has (0,0) bottom-left in Graphics.Blit
                uv.y = -uv.y;

                float3 dir = CubemapFaceDir(_FaceIndex, uv);
                float2 eUV = DirToEquirectUV(dir);
                fixed4 col = tex2D(_MainTex, eUV);
                return col;
            }
            ENDCG
        }
    }
    FallBack Off
}

