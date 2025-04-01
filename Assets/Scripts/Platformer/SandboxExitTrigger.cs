using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Platformer
{
    public class SandboxExitTrigger : MonoBehaviour
    {
        private bool isTriggered;
        
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag("Player") && !isTriggered)
            {
                isTriggered = true;
                SandboxManager.Instance.QuitSandbox().Forget();
            }
        }
    }
}