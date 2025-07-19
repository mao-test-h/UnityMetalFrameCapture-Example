using UnityEngine;

namespace Example
{
    public sealed class Rotation : MonoBehaviour
    {
        [Range(0f, 1f)] public float intensity = 0.5f;

        private void Update()
        {
            var val = intensity * 128f;
            transform.Rotate(0, Time.deltaTime * val, 0);
        }
    }
}
