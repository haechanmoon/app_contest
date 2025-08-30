using ShinhanLostAndFound.Models;

namespace ShinhanLostAndFound.Pages;

[QueryProperty(nameof(Item), "Item")]
public partial class DetailPage : ContentPage
{
    LostItem? _item;
    public LostItem Item
    {
        get => _item;
        set
        {
            _item = value;
            BindingContext = _item;
        }
    }

    public DetailPage()
    {
        InitializeComponent();
    }
}