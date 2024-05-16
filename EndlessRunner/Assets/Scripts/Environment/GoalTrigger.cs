using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoalTrigger : MonoBehaviour
{
    public ParticleSystem[] VictoryParticles;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            if (FindObjectOfType<PlayerController>())
            {
                PlayerController controller = FindObjectOfType<PlayerController>();
                controller.StopMovement();
            }
            else if (FindObjectOfType<NewPlayerController>())
            {
                NewPlayerController controller = FindObjectOfType<NewPlayerController>();
                controller.StopMovement();
            }
            PlayerManager.isGameOver = true;
            foreach(ParticleSystem part in VictoryParticles)
            {
                part.Play();
            }
            AudioSource[] audios = transform.parent.GetComponentsInChildren<AudioSource>();
            foreach(AudioSource a in audios)
            {
                a.Play();
            }
            StartCoroutine(TurnOnVictoryPanel());
        }
    }

    protected IEnumerator TurnOnVictoryPanel()
    {
        yield return new WaitForSeconds(2.5f);
        GUIManager.Current.OpenVictoryMenu();
        yield return null;
    }
}
