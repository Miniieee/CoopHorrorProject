using System;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class DoorHandler : MonoBehaviour
{
    private Transform[] Childs;
        private Transform Joint01;
        private Transform Joint02;

        public enum OpenStyle
        {
            BUTTON,
            AUTOMATIC
        }

        [Serializable]
        public class DoorControls
        {
            public float openingSpeed = 1;
            public float closingSpeed = 1.3f;
            [Range(0, 1)]
            public float closeStartFrom = 0.6f;
            public OpenStyle openMethod; // Open by button or automatically?
            public InputActionReference interactAction;
            public bool autoClose = false; // Automatically close the door. Forced to true when in AUTOMATIC mode.
            [Tooltip("0 - the door is closed, 1 - the door is opened. You can set it to something like 0.15 to get a semi-opened door at start.")]
            [Range(0, 1)]
            public float OpenedAtStart = 0; // If greater than 0, the door will be open at start.
        }

        [Serializable]
        public class AnimNames
        {
            public string OpeningAnim = "Door_open";
            public string LockedAnim = "Door_locked";
        }

        [Serializable]
        public class DoorSounds
        {
            public bool enabled = true;
            public AudioClip open;
            public AudioClip close;
            public AudioClip closed;
            [Range(0, 1.0f)]
            public float volume = 1.0f;
            [Range(0, 0.4f)]
            public float pitchRandom = 0.2f;
        }

        [Serializable]
        public class DoorTexts
        {
            public bool enabled = false;
            public string openingText = "Press [BUTTON] to open";
            public string closingText = "Press [BUTTON] to close";
            public string lockText = "You need a key!";
            public GameObject TextPrefab;
        }

        [Serializable]
        public class KeySystem
        {
            public bool enabled = false;
            [HideInInspector]
            public bool isUnlock = false;
            [Tooltip("If you have a padlock model, you can put the prefab here.")]
            public GameObject LockPrefab;
        }

        [Tooltip("Player's head (with a trigger collider). Set your tag here (usually 'MainCamera')")]
        public string PlayerHeadTag = "MainCamera";

        [Tooltip("Empty GameObject in the door knobs area. If you don't assign one, the script will look for a child named 'doorKnob'.")]
        public Transform knob;

        public DoorControls controls = new DoorControls();
        public AnimNames AnimationNames = new AnimNames();
        public DoorSounds doorSounds = new DoorSounds();
        public DoorTexts doorTexts = new DoorTexts();
        public KeySystem keySystem = new KeySystem();

        Transform player;
        bool Opened = false;
        bool inZone = false;
        Canvas TextObj;
        TextMeshProUGUI theText;
        AudioSource SoundFX;
        Animation doorAnimation;
        Animation LockAnim;

        void Awake()
        {
            // Find door joints and reparent door parts accordingly.
            Childs = GetComponentsInChildren<Transform>();

            foreach (Transform Child in Childs)
            {
                if (Child.name == "Joint01")
                    Joint01 = Child;
                else if (Child.name == "Joint02")
                    Joint02 = Child;
            }

            foreach (Transform Child in Childs)
            {
                if (Child.name == "Door_bottom01")
                    Child.parent = Joint01;
                else if (Child.name == "Door_bottom02")
                    Child.parent = Joint02;
            }
        }

        void Start()
        {
            if (controls.openMethod == OpenStyle.AUTOMATIC)
                controls.autoClose = true;

            if (string.IsNullOrEmpty(PlayerHeadTag))
                Debug.LogError("You need to set a player tag!");

            GameObject playerObj = GameObject.FindWithTag(PlayerHeadTag);
            if (playerObj != null)
                player = playerObj.transform;
            else
                Debug.LogWarning($"{gameObject.name}: Unable to find an object with tag '{PlayerHeadTag}'. The door won't open/close without it.");

            AddText();
            AddLock();
            AddAudioSource();
            DetectDoorKnob();

            doorAnimation = GetComponent<Animation>();

            // Optionally set the door's starting state (semi-opened/closed)
            if (controls.OpenedAtStart > 0)
            {
                doorAnimation[AnimationNames.OpeningAnim].normalizedTime = controls.OpenedAtStart;
                doorAnimation[AnimationNames.OpeningAnim].speed = 0;
                doorAnimation.Play(AnimationNames.OpeningAnim);
            }
        }

        private void OnEnable()
        {
            if (controls.interactAction != null && controls.interactAction.action != null)
            {
                controls.interactAction.action.performed += OnInteract;
                controls.interactAction.action.Enable();
            }
        }

        private void OnDisable()
        {
            if (controls.interactAction != null && controls.interactAction.action != null)
            {
                controls.interactAction.action.performed -= OnInteract;
                controls.interactAction.action.Disable();
            }
        }

        void AddText()
        {
            if (doorTexts.enabled)
            {
                if (doorTexts.TextPrefab == null)
                {
                    Debug.LogWarning($"{gameObject.name}: Text prefab missing. Please assign it in the inspector.");
                    return;
                }
                GameObject go = Instantiate(doorTexts.TextPrefab, Vector3.zero, Quaternion.identity) as GameObject;
                TextObj = go.GetComponent<Canvas>();
                theText = TextObj.GetComponentInChildren<TextMeshProUGUI>();
            }
        }

        void AddLock()
        {
            if (!keySystem.enabled)
                return;

            if (keySystem.LockPrefab != null)
            {
                LockAnim = keySystem.LockPrefab.GetComponent<Animation>();
                keySystem.enabled = true;
            }
        }

        void AddAudioSource()
        {
            GameObject go = new GameObject("SoundFX");
            go.transform.position = transform.position;
            go.transform.rotation = transform.rotation;
            go.transform.parent = transform;
            SoundFX = go.AddComponent<AudioSource>();
            SoundFX.volume = doorSounds.volume;
            SoundFX.spatialBlend = 1;
            SoundFX.playOnAwake = false;
            SoundFX.clip = doorSounds.open;
        }

        void DetectDoorKnob()
        {
            if (knob == null)
            {
                Transform[] children = GetComponentsInChildren<Transform>(true);
                foreach (Transform child in children)
                {
                    if (child.name == "doorKnob")
                    {
                        knob = child;
                        break;
                    }
                }
            }
        }

        void Update()
        {
            // Stop audio if the door animation has finished playing
            if (!doorAnimation.isPlaying && SoundFX.isPlaying)
                SoundFX.Stop();

            if (!inZone)
            {
                HideHint();
                return;
            }

            // In AUTOMATIC mode, the door opens as soon as the player enters the zone
            if (controls.openMethod == OpenStyle.AUTOMATIC && !Opened)
                OpenDoor();

            // Show or hide hint text based on whether the player is looking at the door knob
            if (PLayerIsLookingAtDoorKnob())
            {
                if (controls.openMethod == OpenStyle.BUTTON)
                    ShowHint();
            }
            else
            {
                HideHint();
            }
        }
        void ShowHint()
        {
            if (Opened)
            {
                if (!controls.autoClose)
                    CloseText();
            }
            else
            {
                if (keySystem.enabled && !keySystem.isUnlock)
                    LockText();
                else
                    OpenText();
            }
        }

        /// <summary>
        /// Called when the new input system's Interact action is triggered.
        /// </summary>
        /// <param name="context">The input callback context.</param>
        private void OnInteract(InputAction.CallbackContext context)
        {
            // Only process if in button mode and within the door trigger zone
            if (!inZone || controls.openMethod != OpenStyle.BUTTON)
                return;

            // Verify that the player is looking at the door knob
            if (!PLayerIsLookingAtDoorKnob())
                return;

            if (Opened)
            {
                if (!controls.autoClose)
                    CloseDoor();
            }
            else
            {
                if (keySystem.enabled)
                {
                    if (keySystem.isUnlock)
                        OpenLockDoor();
                    else
                        PlayClosedFXs();
                }
                else
                {
                    OpenDoor();
                }
            }
        }

        bool PLayerIsLookingAtDoorKnob()
        {
            Vector3 forward = player.TransformDirection(Vector3.back);
            Vector3 directionToKnob = knob.position - player.position;
            float dotProd = Vector3.Dot(forward.normalized, directionToKnob.normalized);
            return (dotProd < 0 && dotProd < -0.9f);
        }

        void OpenLockDoor()
        {
            if (keySystem.LockPrefab != null)
            {
                LockAnim.Play("Lock_open");
                Invoke("OpenDoor", 1);
            }
            else
            {
                OpenDoor();
            }
        }

        public void Unlock()
        {
            keySystem.isUnlock = true;
        }

        void OnTriggerEnter(Collider other)
        {
            if (other.tag != PlayerHeadTag)
                return;

            inZone = true;
        }

        void OnTriggerExit(Collider other)
        {
            if (other.tag != PlayerHeadTag)
                return;

            if (Opened && controls.autoClose)
                CloseDoor();

            inZone = false;
        }

        #region TEXT
        public void OpenText()
        {
            ShowText(doorTexts.openingText);
        }

        void LockText()
        {
            ShowText(doorTexts.lockText);
        }

        void CloseText()
        {
            ShowText(doorTexts.closingText);
        }

        void ShowText(string txt)
        {
            if (!doorTexts.enabled)
                return;

            string tempTxt = txt;

            // Replace [BUTTON] with the binding display string from the new input system.
            if (controls.openMethod == OpenStyle.BUTTON)
            {
                if (controls.interactAction != null && controls.interactAction.action != null)
                    tempTxt = txt.Replace("[BUTTON]", $"'{controls.interactAction.action.GetBindingDisplayString()}'");
                else
                    tempTxt = txt.Replace("[BUTTON]", "'E'");
            }

            if (TextObj != null)
            {
                TextObj.enabled = false;
                theText.text = tempTxt;
                TextObj.enabled = true;
            }
        }

        void HideHint()
        {
            if (!doorTexts.enabled)
                return;
            if (TextObj != null)
                TextObj.enabled = false;
            else
                doorTexts.enabled = false;
        }
        #endregion

        #region AUDIO
        void PlaySFX(AudioClip clip)
        {
            if (!doorSounds.enabled)
                return;

            SoundFX.pitch = UnityEngine.Random.Range(1 - doorSounds.pitchRandom, 1 + doorSounds.pitchRandom);
            SoundFX.clip = clip;
            SoundFX.Play();
        }

        void PlayClosedFXs()
        {
            if (doorSounds.closed != null)
            {
                SoundFX.clip = doorSounds.closed;
                SoundFX.Play();
                if (doorAnimation[AnimationNames.LockedAnim] != null)
                {
                    doorAnimation.Play(AnimationNames.LockedAnim);
                    doorAnimation[AnimationNames.LockedAnim].speed = 1;
                    doorAnimation[AnimationNames.LockedAnim].normalizedTime = 0;
                }
            }
        }

        void CloseSound()
        {
            if (doorAnimation[AnimationNames.OpeningAnim].speed < 0 && doorSounds.close != null)
                PlaySFX(doorSounds.close);
        }
        #endregion

        void OpenDoor()
        {
            doorAnimation[AnimationNames.OpeningAnim].speed = controls.openingSpeed;
            // Continue the animation from its current normalized time.
            doorAnimation[AnimationNames.OpeningAnim].normalizedTime = doorAnimation[AnimationNames.OpeningAnim].normalizedTime;
            doorAnimation.Play(AnimationNames.OpeningAnim);

            if (doorSounds.open != null)
                PlaySFX(doorSounds.open);

            Opened = true;
            if (controls.openMethod == OpenStyle.BUTTON)
                HideHint();

            keySystem.enabled = false;
        }

        void CloseDoor()
        {
            // Reverse the animation based on the current normalized time,
            // ensuring that if we are nearly closed we reset to a defined start
            if (doorAnimation[AnimationNames.OpeningAnim].normalizedTime < 0.98f &&
                doorAnimation[AnimationNames.OpeningAnim].normalizedTime > 0)
            {
                doorAnimation[AnimationNames.OpeningAnim].speed = -controls.closingSpeed;
                doorAnimation[AnimationNames.OpeningAnim].normalizedTime = doorAnimation[AnimationNames.OpeningAnim].normalizedTime;
                doorAnimation.Play(AnimationNames.OpeningAnim);
            }
            else
            {
                doorAnimation[AnimationNames.OpeningAnim].speed = -controls.closingSpeed;
                doorAnimation[AnimationNames.OpeningAnim].normalizedTime = controls.closeStartFrom;
                doorAnimation.Play(AnimationNames.OpeningAnim);
            }
            if (doorAnimation[AnimationNames.OpeningAnim].normalizedTime > controls.closeStartFrom)
            {
                doorAnimation[AnimationNames.OpeningAnim].speed = -controls.closingSpeed;
                doorAnimation[AnimationNames.OpeningAnim].normalizedTime = controls.closeStartFrom;
                doorAnimation.Play(AnimationNames.OpeningAnim);
            }
            Opened = false;

            if (controls.openMethod == OpenStyle.BUTTON && !controls.autoClose)
                HideHint();
        }
}
