using UnityEngine;

namespace MetalFrameCapture.Development
{
    public class Rotation : MonoBehaviour
    {
        [Range(0f, 1f)] public float Intensity = 0.5f;

        private void Update()
        {
            var intensity = Intensity * 128f;
            transform.Rotate(0, Time.deltaTime * intensity, 0);
        }
    }
}
