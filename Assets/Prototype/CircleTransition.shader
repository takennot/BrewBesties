Shader "Custom/CircleTransition"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Color ("Color", Color) = (1,1,1,1)
        _Radius ("Circle Radius", Range(0.0, 1.0)) = 0
        _CenterX ("Center X", Range(0.0, 1.0)) = 0.5
        _CenterY ("Center Y", Range(0.0, 1.0)) = 0.5
    }
    SubShader
    {
        // To Draw the Transparent, we must change these things:
        Tags
        {
            "RenderType"="Transparent" "Queue" = "Transparent"
        }
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            fixed4 _Color;
            float _Radius;
            float _CenterX;
            float _CenterY;

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

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            void drawCircle(in float2 uv, in float2 center, in float radius, in float smoothValue, out float output)
            {
                // We check the distance between the center an a point on screen
                // if the distance is inside the radius, we discard it
                // else we draw it pixel

                float sqrDistance = pow(uv.x - center.x, 2) + pow(uv.y - center.y, 2);
                float sqrRadius = pow(radius, 2);

                if(sqrDistance < radius)
                {
                    output = smoothstep(sqrRadius, sqrRadius - smoothValue, sqrDistance);
                }
                else
                {
                    output = 0;
                }
            }

fixed4 frag(v2f i) : SV_Target
{
    float2 center = float2(_CenterX, _CenterY);
    float smoothValue = 0.001f;

    // Calculate distance from the center
    float sqrDistance = pow(i.uv.x - center.x, 2) + pow(i.uv.y - center.y, 2);
    float sqrRadius = pow(_Radius, 2);

    // Inside the circle, make it fully transparent
    if (sqrDistance < sqrRadius)
    {
        return fixed4(0.0, 0.0, 0.0, 0.0);
    }
    else
    {
        // Calculate alpha based on the distance from the circle's edge
        float alpha = smoothstep(sqrRadius, sqrRadius + smoothValue, sqrDistance);

        // Adjust the alpha to maintain partial transparency outside the circle
        fixed4 finalColor = _Color;
        finalColor.a *= alpha;

        return finalColor;
    }
}



            ENDCG
        }
    }
}