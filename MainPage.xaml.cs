using Firebase.Database;
using ShinhanLostAndFound.Models;
using ShinhanLostAndFound.Pages;
using System.Collections.ObjectModel;

namespace ShinhanLostAndFound;

public partial class MainPage : ContentPage
{
    public ObservableCollection<LostItem> LostItems { get; set; } = new ObservableCollection<LostItem>();
    private readonly FirebaseClient _firebaseClient;
    // 이 파일에서는 FirebaseUrl과 _firebaseClient를 삭제합니다.

    public MainPage()
    {
        InitializeComponent();
        // 👇 앱 전체가 공유하는 설정에서 URL을 가져옵니다.
        _firebaseClient = new FirebaseClient(App.FirebaseSettings.FirebaseDatabaseUrl);
        BindingContext = this;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await LoadLostItems();
    }

    private async Task LoadLostItems()
    {
        try
        {
            LostItems.Clear();
            var items = await _firebaseClient
                .Child("LostItems")
                .OnceAsync<LostItem>();

            foreach (var item in items)
            {
                var lostItem = item.Object;
                lostItem.Id = item.Key;
                LostItems.Insert(0, lostItem);
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("데이터 로딩 오류", $"목록을 불러오는 중 오류가 발생했습니다: {ex.Message}", "확인");
        }
    }

    private async void AddButton_Clicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(AddLostItemPage));
    }

    // 👇 OnItemSelected가 아니라 이 함수가 있어야 합니다.
    private async void OnItemTapped(object sender, TappedEventArgs e)
    {
        if (e.Parameter is not LostItem selectedItem)
            return;

        await Shell.Current.GoToAsync(nameof(DetailPage), new Dictionary<string, object>
        {
            { "Item", selectedItem }
        });
    }
}