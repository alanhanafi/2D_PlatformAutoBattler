using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace DefaultNamespace
{
    public class PlaceHolderScript : MonoBehaviour
    {
        [SerializeField] private GameObject newCamera;
        private void Start()
        {
            WaitThenDeactivate().Forget();
        }

        private async UniTask WaitThenDeactivate()
        {
            await UniTask.WaitForSeconds(2);
           // GetComponent<>()
            newCamera.SetActive(true);
        }
    }
}