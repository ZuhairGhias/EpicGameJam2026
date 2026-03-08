using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

[RequireComponent(typeof(AudioSource))]
public class ListeningModeController : MonoBehaviour
{
    [Header("Input")]
    public KeyCode toggleListeningKey = KeyCode.E;

    [Header("Detection")]
    public Camera playerCamera;
    public float listeningRange = 25f;
    public LayerMask detectionLayers = ~0;

    [Tooltip("Makes targeting more forgiving by widening the camera cast.")]
    public float detectionRadius = 1.25f;

    [Header("UI")]
    public GameObject listeningUIRoot;
    public Image cursorImage;
    public TMP_Text conversationText;

    [Header("Cursor Sprites")]
    public Sprite nonTargetCursorSprite;   // red
    public Sprite targetCursorSprite;      // green

    [Header("Audio")]
    public AudioClip enterListeningClip;
    public AudioClip exitListeningClip;
    public AudioClip nonTargetClip;

    [Tooltip("How often the non-target sound can play while aiming at something invalid.")]
    public float nonTargetSoundCooldown = 0.4f;

    [Header("Behavior")]
    [Tooltip("If true, hides text when not looking at a valid target.")]
    public bool hideConversationWhenNotOnTarget = true;

    [Tooltip("If true, keeps cursor hidden when not in listening mode.")]
    public bool hideCursorOutsideListeningMode = true;

    [Tooltip("If true, non-target sound only plays when aiming at a non-target object, not empty space.")]
    public bool onlyPlayNonTargetSoundWhenHitSomething = true;

    [Header("Typewriter Text")]
    [Tooltip("How many characters are revealed per second.")]
    public float charactersPerSecond = 40f;

    [Tooltip("If false, the same text will continue showing without restarting the typewriter effect.")]
    public bool restartTypingWhenSameTextIsShown = false;

    private bool isListening = false;
    private float lastNonTargetSoundTime = -999f;
    private ConversationTarget currentTarget;
    private AudioSource audioSource;

    private Coroutine typingCoroutine;
    private string currentDisplayedFullText = "";

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    void Start()
    {
        if (playerCamera == null)
            playerCamera = Camera.main;

        SetListeningMode(false, true);
    }

    void Update()
    {
        if (Input.GetKeyDown(toggleListeningKey))
            SetListeningMode(!isListening, false);

        if (!isListening)
            return;

        UpdateListeningDetection();
    }

    void SetListeningMode(bool enabled, bool instant)
    {
        if (!enabled && currentTarget != null)
            currentTarget.ClearCachedConversation();

        isListening = enabled;
        currentTarget = null;

        if (listeningUIRoot != null)
            listeningUIRoot.SetActive(enabled);

        if (cursorImage != null)
        {
            bool showCursor = enabled || !hideCursorOutsideListeningMode;
            cursorImage.gameObject.SetActive(showCursor);

            if (showCursor)
                SetCursorValid(false);
        }

        if (conversationText != null)
        {
            StopTyping();
            conversationText.text = "";
            currentDisplayedFullText = "";

            if (enabled)
                conversationText.gameObject.SetActive(!hideConversationWhenNotOnTarget);
            else
                conversationText.gameObject.SetActive(false);
        }

        if (!instant && audioSource != null)
        {
            if (enabled && enterListeningClip != null)
                audioSource.PlayOneShot(enterListeningClip);
            else if (!enabled && exitListeningClip != null)
                audioSource.PlayOneShot(exitListeningClip);
        }
    }

    void UpdateListeningDetection()
    {
        if (playerCamera == null)
            return;

        Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));

        RaycastHit[] hits = Physics.SphereCastAll(
            ray,
            detectionRadius,
            listeningRange,
            detectionLayers,
            QueryTriggerInteraction.Collide
        );

        bool hitSomething = hits != null && hits.Length > 0;
        ConversationTarget detectedTarget = null;

        if (hitSomething)
        {
            System.Array.Sort(hits, (a, b) => a.distance.CompareTo(b.distance));

            for (int i = 0; i < hits.Length; i++)
            {
                Collider hitCollider = hits[i].collider;

                if (hitCollider == null)
                    continue;

                if (IsPartOfThisObject(hitCollider.transform))
                    continue;

                ConversationTarget target = FindConversationTarget(hitCollider);

                if (target != null && HasTargetTag(hitCollider.transform))
                {
                    detectedTarget = target;
                    break;
                }
            }
        }

        if (detectedTarget != null)
        {
            SetCursorValid(true);

            if (detectedTarget != currentTarget)
            {
                if (currentTarget != null)
                    currentTarget.ClearCachedConversation();

                currentTarget = detectedTarget;
                ShowConversation(currentTarget.GetConversationText());
            }

            return;
        }

        if (currentTarget != null)
        {
            currentTarget.ClearCachedConversation();
            currentTarget = null;
        }

        SetCursorValid(false);

        if (hideConversationWhenNotOnTarget && conversationText != null)
        {
            StopTyping();
            conversationText.text = "";
            currentDisplayedFullText = "";
            conversationText.gameObject.SetActive(false);
        }

        if (!onlyPlayNonTargetSoundWhenHitSomething || hitSomething)
            PlayNonTargetSound();
    }

    ConversationTarget FindConversationTarget(Collider hitCollider)
    {
        if (hitCollider == null)
            return null;

        ConversationTarget target = hitCollider.GetComponent<ConversationTarget>();
        if (target != null)
            return target;

        target = hitCollider.GetComponentInParent<ConversationTarget>();
        if (target != null)
            return target;

        return hitCollider.GetComponentInChildren<ConversationTarget>();
    }

    bool HasTargetTag(Transform t)
    {
        while (t != null)
        {
            if (t.CompareTag("Target"))
                return true;

            t = t.parent;
        }

        return false;
    }

    bool IsPartOfThisObject(Transform t)
    {
        if (t == null)
            return false;

        return t == transform || t.IsChildOf(transform);
    }

    void SetCursorValid(bool valid)
    {
        if (cursorImage == null)
            return;

        if (valid)
        {
            if (targetCursorSprite != null)
                cursorImage.sprite = targetCursorSprite;
        }
        else
        {
            if (nonTargetCursorSprite != null)
                cursorImage.sprite = nonTargetCursorSprite;
        }

        cursorImage.SetNativeSize();
    }

    void ShowConversation(string textToShow)
    {
        if (conversationText == null)
            return;

        conversationText.gameObject.SetActive(true);

        if (!restartTypingWhenSameTextIsShown && currentDisplayedFullText == textToShow)
            return;

        StopTyping();
        typingCoroutine = StartCoroutine(TypeText(textToShow));
    }

    IEnumerator TypeText(string fullText)
    {
        currentDisplayedFullText = fullText;
        conversationText.text = "";

        if (string.IsNullOrEmpty(fullText))
        {
            typingCoroutine = null;
            yield break;
        }

        if (charactersPerSecond <= 0f)
        {
            conversationText.text = fullText;
            typingCoroutine = null;
            yield break;
        }

        float delay = 1f / charactersPerSecond;

        for (int i = 0; i < fullText.Length; i++)
        {
            conversationText.text += fullText[i];
            yield return new WaitForSeconds(delay);
        }

        typingCoroutine = null;
    }

    void StopTyping()
    {
        if (typingCoroutine != null)
        {
            StopCoroutine(typingCoroutine);
            typingCoroutine = null;
        }
    }

    void PlayNonTargetSound()
    {
        if (audioSource == null || nonTargetClip == null)
            return;

        if (Time.time >= lastNonTargetSoundTime + nonTargetSoundCooldown)
        {
            audioSource.PlayOneShot(nonTargetClip);
            lastNonTargetSoundTime = Time.time;
        }
    }
}
