/*****************************************************************************
*                                                                            *
*  @file     CustomRenderPipeline.cs                                         *
*  @brief    自定义渲染管线资源创建脚本                                      *
*  Details.                                                                  *
*                                                                            *
*  @author   zhangfan                                                        *
*                                                                            *
*----------------------------------------------------------------------------*
*  Change History :                                                          *
*  <Date>     | <Version> | <Author>       | <Description>                   *
*----------------------------------------------------------------------------*
*  2019/11/25 | 1.0.0.1   | zhangfan      | Create Custom Render Pipeline    *
*----------------------------------------------------------------------------*
*                                                                            *
*****************************************************************************/


using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public struct SRenderPipelineParam
{
    public Color clearColor;
    public bool bDynamicBatching;
    public bool bInsancing;
    public int shadowMapSize;
    public float shadowDistance;
    public int shadowCascades;
    public Vector3 shadowCascadeSplit;
    public MyPostProcessingStack defaultStack;
    public float renderScale;
    public int msaa;
    public bool allowHDR;
}

public class CustomRenderPipeline : RenderPipeline
{
    private CameraRenderer m_renderer;
    private SRenderPipelineParam m_rpp;

    public CustomRenderPipeline(SRenderPipelineParam _rpp)
    {
        //将光照强度从gamma空间变换到线性空间
        GraphicsSettings.lightsUseLinearIntensity = true;
        m_renderer = new CameraRenderer(_rpp);
    }

    protected override void Render(ScriptableRenderContext _context, Camera[] _cameras)
    {
        foreach (var camera in _cameras)
        {
            m_renderer.Render(_context, camera);
        }
    }
}
