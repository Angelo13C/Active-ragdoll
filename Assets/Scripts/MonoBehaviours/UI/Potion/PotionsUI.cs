using Unity.Entities;
using UnityEngine;

public class PotionsUI : MonoBehaviour
{
    [SerializeField] private GameObject _potionUIPrefab;
    
    private void Update()
    {
        var entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        var playerInputQuery = entityManager.CreateEntityQuery(
            ComponentType.ReadOnly<InputMover>());
        if (playerInputQuery.TryGetSingletonEntity<InputMover>(out var playerBody))
        {
            var root = entityManager.GetComponentData<StrengthMultiplier.Root>(playerBody);
            var drinkedPotions = entityManager.GetBuffer<DrinkedPotion>(root.RootEntity);
            for(var i = 0; i < drinkedPotions.Length; i++)
            {
                if (drinkedPotions[i].WasDrunkThisFrame)
                {
                    var firstInactivePotionUI = -1;
                    for (var j = 0; j < transform.childCount; j++)
                    {
                        if (!transform.GetChild(j).gameObject.activeSelf)
                        {
                            firstInactivePotionUI = j;
                            break;
                        }
                    }
                    
                    GameObject potionUI;
                    if (transform.childCount > i && firstInactivePotionUI != -1)
                    {
                        potionUI = transform.GetChild(firstInactivePotionUI).gameObject;
                        potionUI.SetActive(true);
                    }
                    else
                        potionUI = Instantiate(_potionUIPrefab, transform);

                    var potionIcon = entityManager.GetComponentData<Icon>(drinkedPotions[i].Potion);
                    potionUI.GetComponent<PotionUI>().SetIcon(potionIcon.Sprite, drinkedPotions[i].TimeLeft);
                }
            }
        }
    }
}
