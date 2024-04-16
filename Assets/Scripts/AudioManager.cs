using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;

    // 0 = UI Click, 1 = Jump, 2 = PlaceItem, 3 = Bounce, 4 = Die, 5 = OpenChest, 6 = Destroy, 7 = BonusChest
    [SerializeField] private List<AudioClip> clipList = new();
    [SerializeField] private List<float> volumeList = new();

    private readonly List<bool> clipPlaying = new();

    private void Awake()
    {
        for (int i = 0; i < clipList.Count; i++)
            clipPlaying.Add(false);
    }

    public void PlayClip(int clipNumber)
    {
        StartCoroutine(ClipRoutine(clipNumber));
    }
    private IEnumerator ClipRoutine(int clipNumber)
    {
        if (clipPlaying[clipNumber]) yield break;

        clipPlaying[clipNumber] = true;

        audioSource.PlayOneShot(clipList[clipNumber], volumeList[clipNumber]);

        yield return new WaitForSeconds(clipList[clipNumber].length);

        clipPlaying[clipNumber] = false;
    }
}