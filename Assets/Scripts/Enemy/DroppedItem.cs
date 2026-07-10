using System.Collections;
using UnityEngine;

public class DroppedItem : MonoBehaviour
{
    [Header("Loot settings")]
    [SerializeField] private GameObject myPrefab;

    public enum LootType { Health, Artifact, Coin, Ammo }
    public LootType lootType;

    [Header("Drop Jump Settings")]
    [SerializeField] private float _jumpDuration = 0.6f;
    [SerializeField] private float _jumpHeight = 0.5f;
    [SerializeField] private float _scatterRadius = 0.5f;
    private Coroutine _dropCoroutine;
    private Vector3 _baseScale;

    [Header("Disappear Settings")]
    [SerializeField] private float _lifeTime = 30.0f;
    [SerializeField] private float _disappearDuration = 1.0f;
    private Coroutine _lifeTimerCoroutine;

    [Header("Heal Settings")]
    [Tooltip("Percent of Max HP")]
    [Range(0f, 100f)]
    [SerializeField] private float healPercent = 15f;

    [Header("Artifact Settings")]
    [SerializeField] private int value = 10;
    private void Awake()
    {
        _baseScale = transform.localScale;
    }
    public void Drop(Vector3 enemyPosition)
    {
        transform.position = enemyPosition;
        transform.localScale = _baseScale;

        Vector2 randomOffset = Random.insideUnitCircle * _scatterRadius;
        Vector3 targetPosition = enemyPosition + new Vector3(randomOffset.x, randomOffset.y, 0f);

        StopAllCoroutines();

        _dropCoroutine = StartCoroutine(AnimateDropRoutine(enemyPosition, targetPosition));
    }
    private IEnumerator AnimateDropRoutine(Vector3 startPos, Vector3 targetPos)
    {
        float timer = 0f;

        while (timer < _jumpDuration)
        {
            timer += Time.deltaTime;
            float progress = timer / _jumpDuration;

            Vector3 currentPos = Vector3.Lerp(startPos, targetPos, progress);

            float arc = 4f * progress * (1f - progress);
            currentPos.y += arc * _jumpHeight;

            transform.position = currentPos;

            float scaleMultiplier = 1f + (arc * 0.15f);
            transform.localScale = new Vector3(_baseScale.x, _baseScale.y * scaleMultiplier, _baseScale.z);

            yield return null;
        }

        transform.position = targetPos;
        transform.localScale = _baseScale; 

        _dropCoroutine = null;
        _lifeTimerCoroutine = StartCoroutine(LifeTimerRoutine());
    }
    private IEnumerator LifeTimerRoutine()
    {
        yield return new WaitForSeconds(_lifeTime);

        float timer = 0f;
        Vector3 startScale = _baseScale;

        while (timer < _disappearDuration)
        {
            timer += Time.deltaTime;
            float progress = timer / _disappearDuration;

            transform.localScale = Vector3.Lerp(startScale, Vector3.zero, progress);
            yield return null;
        }

        LootSpawner.Instance.ReturnLootToPool(myPrefab, gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            ApplyLootToPlayer(collision.gameObject);
            StopAllCoroutines();
            LootSpawner.Instance.ReturnLootToPool(myPrefab, gameObject);
        }
    }
    private void ApplyLootToPlayer(GameObject player)
    {
        PlayerStats playerStats = player.GetComponent<PlayerStats>();
        switch (lootType)
        {
            case LootType.Health:
                int finalHealAmount = Mathf.RoundToInt((playerStats.MaxHealth * healPercent) / 100f);
                playerStats.Heal(finalHealAmount);
                break;

            case LootType.Artifact:
                // player.GetComponent<PlayerInventory>().AddArtifact(value);
                Debug.Log($"Подобран артефакт с ID {value}");
                break;

            case LootType.Coin:
                // GameManager.Instance.AddMoney(value);
                Debug.Log($"Подобрано золото: {value}");
                break;
        }
    }
    private void OnDisable()
    {
        StopAllCoroutines();
    }
}
