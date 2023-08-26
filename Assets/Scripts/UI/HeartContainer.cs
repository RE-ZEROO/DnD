using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeartContainer : MonoBehaviour
{
    [SerializeField] private GameObject heartPrefab;

    List<HeartUI> hearts = new List<HeartUI>();

    void OnEnable()
    {
        GameController.OnPlayerDamaged += DrawHearts;
        GameController.OnPlayerHeal += DrawHearts;
        GameController.OnInitializeStats += DrawHearts;
    }

    void OnDisable()
    {
        GameController.OnPlayerDamaged -= DrawHearts;
        GameController.OnPlayerHeal -= DrawHearts;
        GameController.OnInitializeStats -= DrawHearts;
    }

    private void DrawHearts()
    {
        ClearHearts();

        //Claculate hearts based of max health
        float maxHealthRemainder = GameController.MaxHealth % 2;
        int heartsToMake = (int)((GameController.MaxHealth / 2) + maxHealthRemainder);

        //Draw hearts based of max health
        for(int i = 0; i < heartsToMake; i++)
        {
            CreateEmptyHeart();
        }

        //Go through every heart and check for status
        for(int i = 0; i < hearts.Count; i++)
        {
            int heartStatusRemainder = (int)Mathf.Clamp(GameController.Health - (i*2), 0, 2);
            hearts[i].SetHeartImage((HeartStatus)heartStatusRemainder);
        }
    }

    private void CreateEmptyHeart()
    {
        GameObject newHeart = Instantiate(heartPrefab);
        newHeart.transform.SetParent(transform);
        newHeart.transform.localScale = heartPrefab.transform.localScale;

        HeartUI heartComponent = newHeart.GetComponent<HeartUI>();
        heartComponent.SetHeartImage(HeartStatus.Empty);
        hearts.Add(heartComponent);
    }

    private void ClearHearts()
    {
        foreach(Transform t in transform)
        {
            Destroy(t.gameObject);
        }

        hearts = new List<HeartUI>();
    }
}
