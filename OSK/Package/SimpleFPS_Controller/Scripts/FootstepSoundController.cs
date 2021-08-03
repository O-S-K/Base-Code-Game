using UnityEngine;

public class FootstepSoundController : MonoBehaviour
{
    private CharacterController characterController;
    private AudioSource audioSource;

    void Start()
    {
        characterController = GetComponent<CharacterController>();
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        if (characterController.isGrounded)
        {
            if (characterController.velocity.magnitude > 0.1F && !audioSource.isPlaying)
            {
                audioSource.volume = Random.Range(0.8f, 1);
                audioSource.pitch = Random.Range(1.2f, 1.5f);
                audioSource.Play();
            }
            else if (characterController.velocity.magnitude <= 0 && audioSource.isPlaying)
            {
                audioSource.Stop();
            }
        }
    }
}
