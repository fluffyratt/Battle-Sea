using UnityEngine;

public class ApplicationPlatformComponentChecker : MonoBehaviour
{
    private void Awake()
    {
        DeactivateObjectOnWeb();
    }

    private void DeactivateObjectOnWeb()
    {
        bool isWebGL = Application.platform == RuntimePlatform.WebGLPlayer;

        if (isWebGL)
        {
            gameObject.SetActive(false);
        }
    }

}
