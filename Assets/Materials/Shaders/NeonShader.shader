Shader "Unlit/NeonShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _EdgeColor ("Edge Color", Color) = (0.0, 0.2, 0.8, 1.0)
        _BaseColor ("Base Color", Color) = (0.0, 0.0, 0.0, 1.0)
        _EdgePercent ("Edge Percent", Range(0,0.5)) = 0.25
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" }
        LOD 100

        Pass
        {
            ZWrite Off
            Cull Off
            Blend SrcAlpha OneMinusSrcAlpha
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _EdgeColor;
            float4 _BaseColor;
            float _EdgePercent;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // apply fog
                UNITY_APPLY_FOG(i.fogCoord, col);

                // For i.u or i.v: 
                // 0 -> _EdgePercent = _EdgeColor
                // _EdgePercent -> 1 - _EdgePercent = _BaseColor
                // 1 - _EdgePercent -> 1.0 = _EdgeColor

                
                fixed modifier = floor(sin(i.uv.x) * 2);
                fixed offset = _EdgePercent * 4.0 - 1.0;
                fixed u_mod = floor(abs(i.uv.x * 4.0 - 2.0) + offset);
                fixed v_mod = floor(abs(i.uv.y * 4.0 - 2.0) + offset);
                fixed mod = saturate(max(u_mod, v_mod));
                fixed4 out_color = (1.0 * mod) * _EdgeColor + (1.0 - mod) * _BaseColor;
                return out_color;
            }
            ENDCG
        }
    }
}
