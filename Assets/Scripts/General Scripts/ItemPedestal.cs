using System;
using System.Net;
using UnityEngine;
using UnityEngine.UI;
using Entities;
using Items;
using Items.Executables;
using Items.Threads;

namespace Items {

    [RequireComponent(typeof(BoxCollider))]
    public class ItemPedestal : Interactable {
        [SerializeField] [Tooltip("Used with the \"Items.{ObjectName}\" to add Items to the character")]
        private string ItemTemplate = "";

        public string ItemTemplateString {
            get => ItemTemplate;
            set {
                ItemTemplate = value;
                if (OnItemTemplateChanged != null)
                    OnItemTemplateChanged();
            }
        }

        [SerializeField] private bool IsEmpty = false;

        private BoxCollider CollisionBox;
        private Image PedestalImage;

        #region Events and Delegates

        private delegate void OnItemTemplateChangedDelegate();

        private event OnItemTemplateChangedDelegate OnItemTemplateChanged;

        #endregion

        private void OnValidate() {
            TryGetComponent(out CollisionBox);
            transform.Find("Components").GetChild(0).GetChild(0).TryGetComponent(out PedestalImage);

            if (!CollisionBox || !PedestalImage) return;

            CollisionBox.isTrigger = true;
            CollisionBox.size = new Vector3(2, 1, 2);
        }

        private void Awake() {
            TryGetComponent(out CollisionBox);
            transform.Find("Components").GetChild(0).GetChild(0).TryGetComponent(out PedestalImage);

            if (!CollisionBox || !PedestalImage) return;

            CollisionBox.isTrigger = true;
            CollisionBox.size = new Vector3(2, 1, 2);
            OnItemTemplateChanged += ItemTemplateChanged;
            ItemTemplateChanged();
        }

        private void ItemTemplateChanged() {
            if (ItemTemplate == "" || ItemTemplate == "Items.NoExecutable") {
                PedestalImage.enabled = false;
                IsEmpty = true;
                return;
            }

            PedestalImage.enabled = true;
            var ResourceData = Resources.Load<Sprite>("Player/UI Images/" + ItemTemplate.Substring(6));
            PedestalImage.sprite = ResourceData ? ResourceData : Resources.Load<Sprite>("Player/UI Images/None");
        }

        private void OnExecutableChanged(ExectuableComponent Component, int slot) {
            OnInteracted(Component.Owner, slot);
        }

        protected override void OnInteracted(Entity InteractedEntity, int slot = 0) {
            if (IsEmpty) return;
            // Create maybe some way to distinguish from Threads and Executables, and have them handled
            var CreatedItem = gameObject.AddComponent(Type.GetType(ItemTemplate));
            if (CreatedItem == null) {
                Debug.LogError("Item does not exist in Assembly. Was it not referenced or misspelled?");
                return;
            }

            // Add this to the list of Items the player has...
            Thread CreatedThread = CreatedItem as Thread;
            Executable CreatedExecutable = CreatedItem as Executable;

            if (CreatedThread && InteractedEntity.ThreadComponent) {
                // Make for Threads
                InteractedEntity.ThreadComponent.AddThread(Type.GetType(ItemTemplate));
                Debug.Log("Created Item was a Thread");
                ItemTemplateString = "";
                Destroy(CreatedItem);
            }
            else if (CreatedExecutable && InteractedEntity.ExectuableComponent) {
                string TempString = ItemTemplate;
                ItemTemplateString = InteractedEntity.ExectuableComponent.Executables[slot - 1].GetType().ToString();
                InteractedEntity.ExectuableComponent.AddExecutable(Type.GetType(TempString), slot);
                Debug.Log("Created Item was a Executable");
                Destroy(CreatedItem);
            }
            else Debug.LogError("Could Not create object of type Thread or Executable from [ItemTemplate]");

            if (CreatedItem == null) Destroy(CreatedItem);
        }

        private void OnTriggerEnter(Collider other) {
            if (IsEmpty) return;

            if (GameManager.Instance.PlayerControllerInstance.Character == other.gameObject) {
                PlayerCharacter Player = other.gameObject.GetComponent<PlayerCharacter>();
                var CreatedItem = gameObject.AddComponent(Type.GetType(ItemTemplate));

                Thread CreatedThread = CreatedItem as Thread;
                if (CreatedThread && Player.ThreadComponent) {
                    OnInteracted(GameManager.Instance.PlayerControllerInstance.PlayerCharacter);
                    Destroy(CreatedItem);
                }

                Executable CreatedExecutable = CreatedItem as Executable;
                if (CreatedExecutable && Player.ExectuableComponent) {
                    Player.ExectuableComponent.InItemPedstalVolume = true;
                    Player.ExectuableComponent.OnExecutableChanged += OnExecutableChanged;
                    Destroy(CreatedItem);
                }

                if (CreatedItem == null) Destroy(CreatedItem);
            }
        }

        private void OnTriggerExit(Collider other) {
            if (IsEmpty) return;

            if (GameManager.Instance.PlayerControllerInstance.Character == other.gameObject) {
                PlayerCharacter Player = other.gameObject.GetComponent<PlayerCharacter>();
                if (Player.ExectuableComponent == null) return;

                Player.ExectuableComponent.InItemPedstalVolume = false;
                Player.ExectuableComponent.OnExecutableChanged -= OnExecutableChanged;
            }
        }
    }
}
