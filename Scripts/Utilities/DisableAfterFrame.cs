using System.Collections;

namespace FPTemplate.Utilities
{
    public class DisableAfterFrame : ExtendedMonoBehaviour
    {
        private void OnEnable()
        {
            StartCoroutine(Disable());
        }

        private IEnumerator Disable()
        {
            yield return null;
            gameObject.SetActive(false);
        }
    }
}