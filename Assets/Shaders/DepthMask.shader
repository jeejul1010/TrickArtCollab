// "Invisible" Occlusion Shader. Useful for AR, Masking, etc
// Mark Johns / Doomlaser - https://twitter.com/Doomlaser

Shader "DepthMask"
{
    Properties
    {
    }
    SubShader
    {
        LOD 100
        Tags{"Queue" ="Geometry-50" "IgnoreProjector"="True" "RenderType"="Opaque"}
        Pass{ColorMask 0}
    }
}