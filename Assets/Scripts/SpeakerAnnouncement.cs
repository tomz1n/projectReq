using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SpeakerAnnouncement : MonoBehaviour
{
    [System.Serializable]
    public class AudioLine
    {
        public AudioClip voiceClip;  
        [TextArea(2, 4)]
        public string subtitleText;  
        public float extraPauseTime = 0.5f;   
    }
    
    [SerializeField] private AudioSource speakerAudioSource;
    [SerializeField] private AudioClip staticSound;
    [SerializeField] private TextMeshProUGUI subtitleTextUI;

    [SerializeField] private List<AudioLine> audioSequence;

    [SerializeField] private bool playOnce = true;
    private bool _hasPlayed;

    private void OnTriggerEnter(Collider other)
    {
        if (_hasPlayed && playOnce) return;

        if (other.CompareTag("Player"))
        {
            _hasPlayed = true;
            StartCoroutine(PlaySequenceRoutine());
        }
    }

    private IEnumerator PlaySequenceRoutine()
    {
        if (staticSound != null && speakerAudioSource != null)
        {
            speakerAudioSource.PlayOneShot(staticSound, 0.7f);
            yield return new WaitForSeconds(staticSound.length);
        }

        foreach (AudioLine line in audioSequence)
        {
            if (line.voiceClip != null && speakerAudioSource != null)
            {
                if (subtitleTextUI != null)
                {
                    subtitleTextUI.text = line.subtitleText;
                    subtitleTextUI.gameObject.SetActive(true);
                }

                speakerAudioSource.PlayOneShot(line.voiceClip);

                yield return new WaitForSeconds(line.voiceClip.length + line.extraPauseTime);
            }
        }

        if (subtitleTextUI != null)
        {
            subtitleTextUI.gameObject.SetActive(false);
        }
    }
}