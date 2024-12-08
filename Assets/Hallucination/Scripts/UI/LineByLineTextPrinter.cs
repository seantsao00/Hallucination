using System.Collections;
using TMPro;
using UnityEngine;

public class LineByLineTextPrinter : MonoBehaviour {
    TMP_Text tmpText;
    [SerializeField] float lineDelay = 0.5f;
    [SerializeField] float fadeDuration = 1.0f;
    TMP_TextInfo textInfo;
    public bool isPrinting { get; private set; }

    public void StartPrintLines() {
        tmpText = GetComponent<TMP_Text>();
        tmpText.ForceMeshUpdate();
        textInfo = tmpText.textInfo;
        isPrinting = true;
        StartCoroutine(PrintLines());
    }

    private IEnumerator PrintLines() {
        tmpText.ForceMeshUpdate();

        // Initialize all characters to be fully transparent
        for (int i = 0; i < textInfo.characterCount; i++) {
            SetCharacterAlpha(i, 0);
        }
        tmpText.UpdateVertexData(TMP_VertexDataUpdateFlags.Colors32);

        for (int line = 0; line < textInfo.lineCount; line++) {
            yield return StartCoroutine(FadeInLine(line, textInfo));
            yield return new WaitForSeconds(lineDelay);
        }
        isPrinting = false;
    }

    IEnumerator FadeInLine(int lineIndex, TMP_TextInfo textInfo) {
        int firstCharIndex = textInfo.lineInfo[lineIndex].firstCharacterIndex;
        int lastCharIndex = textInfo.lineInfo[lineIndex].lastCharacterIndex;

        float fadeSpeed = 1f / fadeDuration;

        for (float alpha = 0; alpha < 1; alpha += Time.deltaTime * fadeSpeed) {
            for (int i = firstCharIndex; i <= lastCharIndex; i++) {
                SetCharacterAlpha(i, (byte)(alpha * 255));
            }
            tmpText.UpdateVertexData(TMP_VertexDataUpdateFlags.Colors32);
            yield return null;
        }

        for (int i = firstCharIndex; i <= lastCharIndex; i++) {
            SetCharacterAlpha(i, 255);
        }
        tmpText.UpdateVertexData(TMP_VertexDataUpdateFlags.Colors32);
    }

    void SetCharacterAlpha(int index, byte alpha) {
        if (!textInfo.characterInfo[index].isVisible) return;
        int materialIndex = textInfo.characterInfo[index].materialReferenceIndex;
        Color32[] newVertexColors = textInfo.meshInfo[materialIndex].colors32;

        for (int j = 0; j < 4; j++) {
            newVertexColors[textInfo.characterInfo[index].vertexIndex + j].a = alpha;
        }
        // textMeshPro.UpdateVertexData(TMP_VertexDataUpdateFlags.Colors32);
    }
}
