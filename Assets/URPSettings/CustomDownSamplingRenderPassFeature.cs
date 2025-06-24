using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

[CreateAssetMenu(menuName = "Rendering/Custom/DownsamplingFeature")]
public class CustomDownSamplingRenderPassFeature : ScriptableRendererFeature
{
    class DownsamplingPass : ScriptableRenderPass
    {
        private Material blitMaterial;
        private RenderTargetIdentifier source;
        private RenderTargetHandle tempTexture;
        private int downSampleFactor;

        public DownsamplingPass(Material material, int factor)
        {
            blitMaterial = material;
            downSampleFactor = Mathf.Max(1, factor);
            tempTexture.Init("_DownsampleTemp");
        }

        public void Setup(RenderTargetIdentifier src)
        {
            source = src;
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            CommandBuffer cmd = CommandBufferPool.Get("Downsample Pass");

            RenderTextureDescriptor desc = renderingData.cameraData.cameraTargetDescriptor;
            desc.width /= downSampleFactor;
            desc.height /= downSampleFactor;
            desc.depthBufferBits = 0;

            cmd.GetTemporaryRT(tempTexture.id, desc, FilterMode.Bilinear);

            // ダウンサンプル
            Blit(cmd, source, tempTexture.Identifier(), blitMaterial);

            // アップスケールして元に戻す
            Blit(cmd, tempTexture.Identifier(), source);

            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }

        public override void FrameCleanup(CommandBuffer cmd)
        {
            if (cmd == null) return;
            cmd.ReleaseTemporaryRT(tempTexture.id);
        }
    }

    public Material blitMaterial;
    [Range(1, 4)] public int downSampleFactor = 2;

    DownsamplingPass downsamplingPass;

    public override void Create()
    {
        downsamplingPass = new DownsamplingPass(blitMaterial, downSampleFactor);
        downsamplingPass.renderPassEvent = RenderPassEvent.AfterRenderingTransparents;
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        renderer.EnqueuePass(downsamplingPass);
    }

}


