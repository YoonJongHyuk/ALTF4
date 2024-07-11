using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ApplyMaterialWithTiling : MonoBehaviour
{
    public Material originalMaterial;
    public Vector2 textureScale = new Vector2(1, 1);  // �⺻ �ؽ�ó Ÿ�ϸ� ��

    void Start()
    {
        Renderer renderer = GetComponent<Renderer>();
        if (renderer != null)
        {
            // ���� ��Ƽ������ �ν��Ͻ��� �����մϴ�.
            Material instanceMaterial = Instantiate(originalMaterial);
            instanceMaterial.mainTextureScale = textureScale;
            renderer.material = instanceMaterial;
        }
    }
}
