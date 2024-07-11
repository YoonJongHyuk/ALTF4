using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ApplyMaterialWithTiling : MonoBehaviour
{
    public Material originalMaterial;
    public Vector2 textureScale = new Vector2(1, 1);  // 기본 텍스처 타일링 값

    void Start()
    {
        Renderer renderer = GetComponent<Renderer>();
        if (renderer != null)
        {
            // 원본 머티리얼의 인스턴스를 생성합니다.
            Material instanceMaterial = Instantiate(originalMaterial);
            instanceMaterial.mainTextureScale = textureScale;
            renderer.material = instanceMaterial;
        }
    }
}
