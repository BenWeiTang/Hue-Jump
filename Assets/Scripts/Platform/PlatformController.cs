using UnityEngine;

public class PlatformController : MonoBehaviour
{
    public PlatformType PlatformType => _platformType;
    
    [SerializeField] private PlatformType _platformType = PlatformType.Default;
}

public enum PlatformType
{
    Default,
    OneTime,
    Trampoline,
    Swapper
}