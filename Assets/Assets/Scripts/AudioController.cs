using System.Collections;
using UnityEngine;


public class AudioController : MonoBehaviour
{
    private static AudioController _instance;
    public static AudioController Instance => _instance;

    public enum SoundEffectType
    {
        Rain,
        Door,
        Bear,
        SwordAttack,
        RippingApart,
        BirdsChirping,
        
        Count
    }

    [SerializeField] 
    private AudioSource audioSource;
    
    [SerializeField]
    [EnumNamedArray( typeof(SoundEffectType) )]
    private AudioClip[] sfx = new AudioClip[(int) SoundEffectType.Count];

    private Queue filesToPlay = new Queue(8);
    private float timeOfLastPlay = float.MinValue;

    public struct ScheduledFile
    {
        public SoundEffectType Type;
        public float PlayAfterSeconds;
        public bool ShouldStopSoundBefore;
    }

    public void SyncWithStoryState()
    {
        timeOfLastPlay = Time.time;
    }

    public void AddSoundToPlay(SoundEffectType type,
        float afterSeconds = 0.0f,
        bool shouldStopSoundBefore = true)
    {
        ScheduledFile file;
        file.Type = type;
        file.PlayAfterSeconds = afterSeconds;
        file.ShouldStopSoundBefore = shouldStopSoundBefore;
        filesToPlay.Enqueue(file);
    }

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (filesToPlay.Count == 0)
        {
            return;
        }

        ScheduledFile file = (ScheduledFile) filesToPlay.Peek();
        float timeSinceLastPlay = Time.time - timeOfLastPlay;
        bool canPlay = file.PlayAfterSeconds < timeSinceLastPlay;
        if (canPlay)
        {
            if (file.ShouldStopSoundBefore)
            {
                audioSource.Stop();
            }
            audioSource.PlayOneShot(sfx[(int) file.Type], 1);
            filesToPlay.Dequeue();

            timeOfLastPlay = Time.time + file.PlayAfterSeconds;
        }
    }
}
