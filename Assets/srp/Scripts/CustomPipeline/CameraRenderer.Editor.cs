/*****************************************************************************
*                                                                            *
*  @file     CameraRenderer.cs                                               *
*  @brief    单个相机自定义渲染在场景中辅助渲染脚本                          *
*  Details   用于渲染场景中不支持的材质和相机坐标轴等                        *
*                                                                            *
*  @author   zhangfan                                                        *
*                                                                            *
*----------------------------------------------------------------------------*
*  Change History :                                                          *
*  <Date>     | <Version> | <Author>    | <Description>                      *
*----------------------------------------------------------------------------*
*  2019/11/25 | 1.0.0.1   | zhangfan    | Create PostProcessing              *
*----------------------------------------------------------------------------*
*                                                                            *
*****************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Profiling;

partial class CameraRenderer
{
    partial void drawUnsupportedShaders();

    partial void prepareForSceneWindow();

    partial void drawGizmos();

    partial void prepareBuffer();

#if UNITY_EDITOR
    private static ShaderTagId[] m_legacyShaderTagIds = {
            new ShaderTagId("Always"),
            new ShaderTagId("ForwardBase"),
            new ShaderTagId("PrepassBase"),
            new ShaderTagId("Vertex"),
            new ShaderTagId("VertexLMRGBM"),
            new ShaderTagId("VertexLM"),
    };

    private Material m_errorMaterial;

    private string sampleName { get; set; }

    /// <summary>
    /// 绘制不支持的材质
    /// </summary>
    partial void drawUnsupportedShaders()
    {
        if (m_errorMaterial == null)
            m_errorMaterial = new Material(Shader.Find("Hidden/InternalErrorShader"));

        var drawingSettings = new DrawingSettings(m_legacyShaderTagIds[0], new SortingSettings(m_camera))
        {
            overrideMaterial = m_errorMaterial
        };
        for (int i = 1; i < m_legacyShaderTagIds.Length; i++)
        {
            drawingSettings.SetShaderPassName(i, m_legacyShaderTagIds[i]);
        }
        var filteringSettings = FilteringSettings.defaultValue;
        m_context.DrawRenderers(m_cullingResults, ref drawingSettings, ref filteringSettings);
    }

    /// <summary>
    /// 绘制相机空间
    /// </summary>
    partial void drawGizmos()
    {
        if (Handles.ShouldRenderGizmos())
        {
            m_context.DrawGizmos(m_camera, GizmoSubset.PreImageEffects);
            m_context.DrawGizmos(m_camera, GizmoSubset.PostImageEffects);
        }
    }

    /// <summary>
    /// 在编辑下显示UI
    /// </summary>
    partial void prepareForSceneWindow()
    {
        if (m_camera.cameraType == CameraType.SceneView)
        {
            ScriptableRenderContext.EmitWorldGeometryForSceneView(m_camera);
        }
    }

    /// <summary>
    /// 设置Command Buffer 名字
    /// </summary>
    partial void prepareBuffer()
    {
        Profiler.BeginSample("Editor Only");
        m_cameraBuffer.name = sampleName = m_camera.name;
        Profiler.EndSample();
    }
#else
    string sampleName => "Custom Renderer Pipeline";
#endif
}
