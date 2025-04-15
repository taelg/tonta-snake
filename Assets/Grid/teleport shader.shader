Shader "Unlit/PortalEffect"
{
    Properties
    {
        _MainColor ("Main Color", Color) = (1,1,1,1)
        _Color2 ("Secondary Color", Color) = (0,1,1,1)
        _Color3 ("Tertiary Color", Color) = (1,0,1,1)
        _ScrollSpeed ("Scroll Speed", Range(0, 5)) = 2
        _GlowIntensity ("Glow Intensity", Range(1, 5)) = 2
        _DistortionStrength ("Distortion Strength", Range(0, 0.1)) = 0.05
        _NoiseTex ("Distortion Texture", 2D) = "white" {}
    }

    SubShader
    {
        Tags { 
            "RenderType"="Transparent" 
            "Queue"="Transparent"
        }

        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _NoiseTex;
            float4 _MainColor;
            float4 _Color2;
            float4 _Color3;
            float _ScrollSpeed;
            float _GlowIntensity;
            float _DistortionStrength;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // Cria duas direções de scroll diferentes
                float2 scroll1 = i.uv + float2(_Time.y * _ScrollSpeed, 0);
                float2 scroll2 = i.uv + float2(0, _Time.y * _ScrollSpeed * 0.8);
                
                // Adiciona distorção usando textura de ruído
                float2 distortion = tex2D(_NoiseTex, i.uv * 0.5 + _Time.y * 0.2).rg;
                distortion = distortion * 2 - 1;
                scroll1 += distortion * _DistortionStrength;
                scroll2 += distortion * _DistortionStrength;

                // Cria padrões de onda
                float wave1 = sin(scroll1.x * 20 + _Time.y * 5) * 0.5 + 0.5;
                float wave2 = cos(scroll2.y * 15 + _Time.y * 4) * 0.5 + 0.5;
                
                // Combina cores
                float3 color = _MainColor.rgb;
                color = lerp(color, _Color2.rgb, wave1);
                color = lerp(color, _Color3.rgb, wave2);
                
                // Cria efeito de brilho
                float glow = (wave1 + wave2) * _GlowIntensity;
                
                // Cria transparência pulsante
                float alpha = (sin(_Time.y * 3) * 0.5 + 0.5) * 0.8 + 0.2;
                alpha *= tex2D(_NoiseTex, i.uv + _Time.y).r * 0.5 + 0.5;

                return fixed4(color * glow, alpha * _MainColor.a);
            }
            ENDCG
        }
    }
}