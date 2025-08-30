using ShinhanLostAndFound.Models;
using Firebase.Database;
using Firebase.Database.Query;
using Firebase.Storage;
using Newtonsoft.Json;
using System.Reflection;
using System.Text;

namespace ShinhanLostAndFound.Pages;


public partial class AddLostItemPage : ContentPage
{
    private readonly FirebaseClient _firebaseClient;
    private static FirebaseSettings _settings;
    private FileResult? _selectedPhoto;
    private string? _loadedItemImagePath; // '내 소지품'에서 불러온 이미지 경로를 저장할 변수

    public AddLostItemPage()
    {
        InitializeComponent();
        // 👇 앱 전체가 공유하는 설정에서 URL을 가져옵니다.
        _firebaseClient = new FirebaseClient(App.FirebaseSettings.FirebaseDatabaseUrl);
    }

    // 👇 '내 소지품에서 불러오기' 버튼을 눌렀을 때 실행될 함수
    private async void OnLoadFromMyItemsClicked(object sender, EventArgs e)
    {
        var itemsJson = Preferences.Get("my_items_list", string.Empty);
        if (string.IsNullOrEmpty(itemsJson))
        {
            await DisplayAlert("알림", "저장된 소지품이 없습니다.", "확인");
            return;
        }

        var myItems = JsonConvert.DeserializeObject<List<MyItem>>(itemsJson);
        if (myItems == null || !myItems.Any())
        {
            await DisplayAlert("알림", "저장된 소지품이 없습니다.", "확인");
            return;
        }

        // ActionSheet을 사용해 사용자에게 소지품 목록을 보여주고 선택하게 합니다.
        var itemNames = myItems.Select(item => item.Name).ToArray();
        var selectedName = await DisplayActionSheet("소지품 선택", "취소", null, itemNames);

        if (selectedName != null && selectedName != "취소")
        {
            var selectedItem = myItems.FirstOrDefault(item => item.Name == selectedName);
            if (selectedItem != null)
            {
                // 선택된 아이템의 정보로 UI를 업데이트합니다.
                NameEntry.Text = selectedItem.Name;
                ItemImage.Source = ImageSource.FromFile(selectedItem.ImagePath);

                // '내 소지품'에서 불러왔다는 것을 기록해둡니다.
                _loadedItemImagePath = selectedItem.ImagePath;
                _selectedPhoto = null;
            }
        }
    }

    // Pages/AddLostItemPage.xaml.cs 파일에 이 함수를 새로 추가하세요.

    private void OnStatusChanged(object sender, CheckedChangedEventArgs e)
    {
        if (sender is RadioButton radioButton && radioButton.IsChecked)
        {
            // '분실했어요'가 선택된 경우
            if (radioButton == LostRadioButton)
            {
                LocationLabel.Text = "분실 추정 장소";
                LocationEntry.Placeholder = "예상되는 장소들을 자유롭게 적어주세요. (예: 기도관1층 정수기 위, 믿음관4층 남자화장실)";
            }
            // '습득했어요'가 선택된 경우
            else
            {
                LocationLabel.Text = "습득 장소";
                LocationEntry.Placeholder = "예: 기도관 2280";
            }
        }
    }

    private async void OnSelectImageClicked(object sender, EventArgs e)
    {
        _selectedPhoto = await MediaPicker.PickPhotoAsync();
        if (_selectedPhoto != null)
        {
            // 👇 이 부분이 수정되었습니다.
            // 1. 먼저 비동기적으로 사진 데이터(Stream)를 가져와서 변수에 저장합니다.
            var stream = await _selectedPhoto.OpenReadAsync();

            // 2. 다 가져온 실제 데이터를 동기 함수에 전달합니다.
            ItemImage.Source = ImageSource.FromStream(() => stream);

            // 갤러리에서 새로 선택했으므로 '불러온 이미지' 기록은 초기화합니다.
            _loadedItemImagePath = null;
        }
    }

    private async void SubmitButton_Clicked(object sender, EventArgs e)
    {
        // 사진이 선택되었는지 확인 (갤러리에서 선택 or 내 소지품에서 불러오기)
        if (_selectedPhoto == null && string.IsNullOrEmpty(_loadedItemImagePath))
        {
            await DisplayAlert("입력 오류", "사진을 선택해주세요.", "확인");
            return;
        }
        if (string.IsNullOrWhiteSpace(NameEntry.Text) || string.IsNullOrWhiteSpace(LocationEntry.Text) || string.IsNullOrWhiteSpace(ContactEntry.Text))
        {
            await DisplayAlert("입력 오류", "모든 정보를 입력해주세요.", "확인");
            return;
        }

        SubmitButton.IsEnabled = false;
        LoadingIndicator.IsVisible = true;
        LoadingIndicator.IsRunning = true;

        try
        {
            Stream photoStream;
            string fileName;

            // '내 소지품'에서 불러온 사진인지, 갤러리에서 새로 선택한 사진인지에 따라 처리
            if (!string.IsNullOrEmpty(_loadedItemImagePath))
            {
                photoStream = File.OpenRead(_loadedItemImagePath);
                fileName = Path.GetFileName(_loadedItemImagePath);
            }
            else
            {
                photoStream = await _selectedPhoto.OpenReadAsync();
                fileName = _selectedPhoto.FileName;
            }

            var storageFileName = $"{Guid.NewGuid()}_{fileName}";
            var storage = new FirebaseStorage(App.FirebaseSettings.FirebaseStorageBucket);
            var imageUrl = await storage.Child("LostItemImages").Child(storageFileName).PutAsync(photoStream);
            photoStream.Close();

            var newItem = new LostItem
            {
                Name = NameEntry.Text,
                Location = LocationEntry.Text,
                Contact = ContactEntry.Text,
                Date = DateTime.Now.ToString("yyyy-MM-dd"),
                ImageUrl = imageUrl,
                Status = LostRadioButton.IsChecked ? "분실" : "습득"
            };

            await _firebaseClient.Child("LostItems").PostAsync(newItem);

            await DisplayAlert("성공", "정상적으로 등록되었습니다.", "확인");
            await Shell.Current.GoToAsync("..");
        }
        catch (Exception ex)
        {
            await DisplayAlert("등록 오류", $"데이터를 등록하는 중 오류가 발생했습니다: {ex.Message}", "확인");
        }
        finally
        {
            LoadingIndicator.IsRunning = false;
            LoadingIndicator.IsVisible = false;
            SubmitButton.IsEnabled = true;
        }
    }
}