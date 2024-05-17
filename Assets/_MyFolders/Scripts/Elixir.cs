using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Elixir : MonoBehaviour
{

    [field: SerializeField] public Observer<int> ElixirCount { get; private set; } = new Observer<int>(0);
    [SerializeField] Slider elixirSlider;
    [SerializeField] TextMeshProUGUI elixirCountText;

    bool incrementing = false;

    void Start()
    {
        ElixirCount.AddListener((count) => StartCoroutine(OnElixirUpdate()));
        ElixirCount.AddListener((count) => UpdateElixirUI());

        ElixirCount.Invoke();
    }

    IEnumerator OnElixirUpdate()
    {
        if (ElixirCount.Value < 10 && !incrementing)
        {
            incrementing = true;
            yield return new WaitForSeconds(1);
            incrementing = false;
            ElixirCount.Value++;       // Recurssively calls OnElixirUpdate()
        }
    }

    void UpdateElixirUI()
    {
        elixirCountText.text = ElixirCount.Value.ToString();
        elixirSlider.value = ElixirCount.Value / 10.0f;
    }

    public bool TryConsumeElixir(int amount)
    {
        if (ElixirCount.Value >= amount)
        {
            ElixirCount.Value -= amount;
            return true;
        }
        return false;
    }

}
