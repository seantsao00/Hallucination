using UnityEngine;

public class Hyperlink : MonoBehaviour {
    [SerializeField] string url;

    public void VisitLink() {
        Application.OpenURL(url);
    }
}
