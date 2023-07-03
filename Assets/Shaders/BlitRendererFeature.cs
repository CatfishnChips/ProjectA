using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class BlitRendererFeature : ScriptableRendererFeature
{
    [System.Serializable]
    public class PassSettings
    {
        public RenderPassEvent renderPassEvent = RenderPassEvent.AfterRenderingTransparents;
        public Material material = null;
    }
    public PassSettings passSettings = new PassSettings();

    private CustomRenderPass m_ScriptablePass;

    public override void Create()
    {
        m_ScriptablePass = new CustomRenderPass(passSettings);
        m_ScriptablePass.renderPassEvent = passSettings.renderPassEvent;
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        if (passSettings.material == null) return;

        if (renderingData.cameraData.isPreviewCamera) return;   

        renderer.EnqueuePass(m_ScriptablePass);
    }

    class CustomRenderPass : ScriptableRenderPass
    {
        const string ProfilerTag = "Blit Pass";

        private Material material = null;
        private RenderTargetIdentifier source;
        private RenderTargetHandle tempText;
        private PassSettings passSettings = null;

        public CustomRenderPass(PassSettings passSettings)
        {
            this.material = passSettings.material;
            this.passSettings = passSettings;
            renderPassEvent = passSettings.renderPassEvent;
            tempText.Init("_TempTexture");
        }

        public void SetSource(RenderTargetIdentifier source)
        {
            this.source = source;
        }
        

        public override void Configure(CommandBuffer cmd, RenderTextureDescriptor cameraTextureDescriptor)
        {
            cmd.GetTemporaryRT(tempText.id, cameraTextureDescriptor, FilterMode.Point);
        }

        public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
        {
            SetSource(renderingData.cameraData.renderer.cameraColorTarget);
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            CommandBuffer cmd = CommandBufferPool.Get("CustomBlitRenderPass");

            using (new ProfilingScope(cmd, new ProfilingSampler(ProfilerTag)))
            {
                Blit(cmd, source, tempText.id, material, 0); 
                Blit(cmd, tempText.id, source);               
            }

            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }

        public override void OnCameraCleanup(CommandBuffer cmd)
        {
            cmd.ReleaseTemporaryRT(tempText.id);
        }
    }
}


