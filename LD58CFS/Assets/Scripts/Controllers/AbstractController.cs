using QFramework;
using UnityEngine;

namespace LD58
{
    public class AbstractController : MonoBehaviour, IController
    {
        public IArchitecture GetArchitecture() => LD58App.Interface;
    }
}