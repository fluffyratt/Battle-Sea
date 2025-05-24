using UnityEngine;
using TMPro;

public class AboutPanel : MonoBehaviour
{
    [Header("About Section")]
    [SerializeField] private TextMeshProUGUI _aboutSection;
    [SerializeField] private string _aboutText;
    [SerializeField] private string _gameName = "BattleSea";
    [SerializeField] private string _groupName = "ТК-32";
    [SerializeField] private string _authorName = "Хмельовою Діаною Анатоліївною";

    private void Awake()
    {
        CheckFields();
        FillTextComponent();
    }

    private void OnValidate()
    {
        CheckFields();
        FillTextComponent();
    }

    private void CheckFields()
    {
        if (_aboutSection == null)
        {
            Debug.LogError($"Text component at {this.gameObject.name} was not assigned!");
        }

        if(string.IsNullOrEmpty(_aboutText))
        {
            if (string.IsNullOrEmpty(_authorName) == false &&
                string.IsNullOrEmpty(_groupName) == false &&
                string.IsNullOrEmpty(_gameName) == false)
            {

                _aboutText = $"Гра під назвою {_gameName} \n" +
                $"Виконана студенткою групи {_groupName} \n" +
                $"{_authorName}";
            }
            else
            {
                _aboutText = $"Гра під назвою BattleSea \n" +
                 $"Виконана студенткою групи ТК-32 \n" +
                 $"Хмельовою Діаною Анатоліївною";
            }


        }
    }

    private void FillTextComponent()
    {
        _aboutSection.text = _aboutText;
    }
}
