using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class Teleport : MonoBehaviour
{
    public Image fadeOverlay;
    public Transform destination;
    private GameObject player;

    // Start is called before the first frame update
    public void TeleportPlayer()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        StartCoroutine(TeleportRoutine());
    }

    IEnumerator TeleportRoutine()
    {
        while (fadeOverlay.color.a < 1f)
        {
            fadeOverlay.color = new Color(fadeOverlay.color.r, fadeOverlay.color.g, fadeOverlay.color.b, fadeOverlay.color.a + .05f);
            yield return null;
        }
        player.transform.position = destination.position;
        while (fadeOverlay.color.a > 0f)
        {
            fadeOverlay.color = new Color(fadeOverlay.color.r, fadeOverlay.color.g, fadeOverlay.color.b, fadeOverlay.color.a - .05f);
            yield return null;
        }
    }
}
