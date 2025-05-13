using UnityEngine;

public class FogOfWarPersistent : MonoBehaviour
{
    [SerializeField] private RenderTexture fogOfWarRenderTexture;
    [SerializeField] private RenderTexture fogOfWarPersistentRenderTexture;
    [SerializeField] private RenderTexture fogOfWarPersistentRenderTexture2;
    [SerializeField] private Material fogOfWarPersistentMaterial;
    private void Start()
    {
        Graphics.Blit(fogOfWarRenderTexture, fogOfWarPersistentMaterial);
        Graphics.Blit(fogOfWarRenderTexture, fogOfWarPersistentRenderTexture2);
    }
    private void Update()
    {
        Graphics.Blit(fogOfWarRenderTexture, fogOfWarPersistentRenderTexture, fogOfWarPersistentMaterial, 0);
        Graphics.CopyTexture(fogOfWarPersistentRenderTexture, fogOfWarPersistentRenderTexture2);
    }
}
