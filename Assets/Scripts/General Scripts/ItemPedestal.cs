using System;
using UnityEngine;
using UnityEngine.UI;
using Items;
using Items.Executables;
using Items.Threads;

[RequireComponent(typeof(BoxCollider))]
public class ItemPedestal : Interactable {
    [SerializeField] private string ItemTemplate = ""; // "Items." is to be not used, then the XXXXX is to be saved or transported etc.
    private Item CreatedItem;
    
    private BoxCollider CollisionBox;
    private Image PedestalImage;

    private void OnValidate() {
        TryGetComponent(out CollisionBox);
        transform.Find("Components").GetChild(0).GetChild(0).TryGetComponent(out PedestalImage);

        if (!CollisionBox || !PedestalImage) return;

        CollisionBox.isTrigger = true;
        CollisionBox.size = new Vector3(2, 1, 2);
        
        CreatedItem = Activator.CreateInstance(Type.GetType(ItemTemplate), false) as Item;
        if (CreatedItem != null) PedestalImage.sprite = Resources.Load<Sprite>(CreatedItem.GetStats().Sprite);
        else if (ItemTemplate != "") PedestalImage.sprite = Resources.Load<Sprite>("Player/UI Images/" + ItemTemplate);
        else PedestalImage.sprite = Resources.Load<Sprite>("Player/UI Images/None");
    }
    
    private void Awake() {
        TryGetComponent(out CollisionBox);
        transform.Find("Components").GetChild(0).GetChild(0).TryGetComponent(out PedestalImage);

        if (!CollisionBox || !PedestalImage) return;

        CollisionBox.isTrigger = true;
        CollisionBox.size = new Vector3(2, 1, 2);
        
        CreatedItem = Activator.CreateInstance(Type.GetType(ItemTemplate), false) as Item;
        if (CreatedItem != null) PedestalImage.sprite = Resources.Load<Sprite>(CreatedItem.GetStats().Sprite);
        else if (ItemTemplate != "") PedestalImage.sprite = Resources.Load<Sprite>("Player/UI Images/" + ItemTemplate);
        else PedestalImage.sprite = Resources.Load<Sprite>("Player/UI Images/None");
    }

    protected override void OnInteracted(Entity InteractedEntity) {
        InteractedEntity.gameObject.AddComponent(Type.GetType("Items." + ItemTemplate));
        // Add this to the list of Items the player has...
    }

    private void OnTriggerEnter(Collider other) {
        if (GameManager.Instance.PlayerInstance == other.gameObject)
            OnInteracted(GameManager.Instance.PlayerInstance.GetComponent<Entity>());
    }
}
