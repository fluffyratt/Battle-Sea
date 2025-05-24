using UnityEngine;
using TMPro;

public class AboutPanel : MonoBehaviour
{
    [Header("About Section")]
    [SerializeField] private TextMeshProUGUI _aboutSection;
    [SerializeField] private string _aboutText;
    [SerializeField] private string _gameName = "BattleSea";
    [SerializeField] private string _groupName = "��-32";
    [SerializeField] private string _authorName = "��������� ĳ���� �����볿����";

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

                _aboutText = $"��� �� ������ {_gameName} \n" +
                $"�������� ���������� ����� {_groupName} \n" +
                $"{_authorName}";
            }
            else
            {
                _aboutText = $"��� �� ������ BattleSea \n" +
                 $"�������� ���������� ����� ��-32 \n" +
                 $"��������� ĳ���� �����볿����";
            }


        }
    }

    private void FillTextComponent()
    {
        _aboutSection.text = _aboutText;
    }
}
