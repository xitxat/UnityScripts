using UnityEngine;
using Unity.MLAgents;
using System.Collections;

public class AreaSettings : MonoBehaviour
{
    public float agentRunSpeed;
    public float agentRotationSpeed;
    public float spawnAreaMarginMultiplier;

    [Header("Material Swap Settings")]
    public MeshRenderer trophyMeshRenderer;
    public Material defaultMaterial;
    public Material bonusMaterial;
    public Material successMaterial;
    public Material failureMaterial;


    public IEnumerator SwapGroundMaterial(Material mat, float time)
    {
        trophyMeshRenderer.material = mat;
        yield return new WaitForSeconds(time); // Wait for x sec
        trophyMeshRenderer.material = defaultMaterial;
    }



}
