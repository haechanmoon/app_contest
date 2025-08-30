using Plugin.LocalNotification;
using ShinhanLostAndFound.Models;
using System.Collections.ObjectModel;
using Newtonsoft.Json;

namespace ShinhanLostAndFound.Pages;

public partial class MyItemsPage : ContentPage
{
    public ObservableCollection<MyItem> MyItems { get; set; } = new ObservableCollection<MyItem>();
    private FileResult? _selectedPhoto;

    public MyItemsPage()
    {
        InitializeComponent();
        BindingContext = this;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        LoadItems();
        var savedTime = Preferences.Get("notification_time", "18:00:00");
        if (TimeSpan.TryParse(savedTime, out var time))
        {
            NotificationTimePicker.Time = time;
        }
    }

    // 👇 목록 상태에 따라 UI를 변경하는 함수
    private void UpdateEmptyView()
    {
        bool isListEmpty = !MyItems.Any();
        EmptyView.IsVisible = isListEmpty;
        ItemsCollectionView.IsVisible = !isListEmpty;
    }

    private async void OnSaveTimeClicked(object sender, EventArgs e)
    {
        if (await LocalNotificationCenter.Current.AreNotificationsEnabled() == false)
        {
            await LocalNotificationCenter.Current.RequestNotificationPermission();
        }

        LocalNotificationCenter.Current.Cancel(100);

        // --- 👇 똑똑한 맞춤 알림 로직 시작 ---

        // 1. 핸드폰에 저장된 소지품 목록 불러오기
        var myItemsList = new List<MyItem>();
        var itemsJson = Preferences.Get("my_items_list", string.Empty);
        if (!string.IsNullOrEmpty(itemsJson))
        {
            try
            {
                myItemsList = JsonConvert.DeserializeObject<List<MyItem>>(itemsJson);
            }
            catch
            {
                // 데이터가 손상되었으면 비어있는 리스트를 사용
            }
        }

        // 2. 알림 메시지 생성하기
        string notificationDescription;
        if (myItemsList != null && myItemsList.Any())
        {
            // 목록에 아이템이 있으면, 랜덤으로 하나를 뽑아서 메시지에 추가
            var randomItem = myItemsList[new Random().Next(myItemsList.Count)];
            notificationDescription = $"나가기 전, 소중한 '{randomItem.Name}' 잊지 않으셨죠?";
        }
        else
        {
            // 목록이 비어있으면 기본 메시지 사용
            notificationDescription = "나가기 전, 소지품을 잘 챙겼는지 확인해보세요!";
        }

        // --- 똑똑한 맞춤 알림 로직 끝 ---

        var scheduleTime = DateTime.Today.Add(NotificationTimePicker.Time);
        if (scheduleTime < DateTime.Now)
        {
            scheduleTime = scheduleTime.AddDays(1);
        }

        var request = new NotificationRequest
        {
            NotificationId = 100,
            Title = "챙김이 🔔",
            Description = notificationDescription, // 👈 생성된 맞춤 메시지 사용
            Schedule = new NotificationRequestSchedule
            {
                NotifyTime = scheduleTime,
                RepeatType = NotificationRepeat.Daily
            }
        };

        await LocalNotificationCenter.Current.Show(request);
        Preferences.Set("notification_time", NotificationTimePicker.Time.ToString());
        await DisplayAlert("저장 완료", $"매일 {NotificationTimePicker.Time:hh\\:mm}에 맞춤 알림을 보내드릴게요.", "확인");
    }

    private async void OnSelectImageClicked(object sender, EventArgs e)
    {
        _selectedPhoto = await MediaPicker.PickPhotoAsync();
        if (_selectedPhoto != null)
        {
            var stream = await _selectedPhoto.OpenReadAsync();
            ItemImage.Source = ImageSource.FromStream(() => stream);
        }
    }
    private async void OnAddItemClicked(object sender, EventArgs e)
    {
        if (_selectedPhoto == null || string.IsNullOrWhiteSpace(ItemEntry.Text))
        {
            await DisplayAlert("입력 오류", "사진과 소지품 이름을 모두 입력해주세요.", "확인");
            return;
        }
        var newPath = Path.Combine(FileSystem.AppDataDirectory, _selectedPhoto.FileName);
        using (var stream = await _selectedPhoto.OpenReadAsync())
        using (var newStream = File.OpenWrite(newPath))
        {
            await stream.CopyToAsync(newStream);
        }
        MyItems.Add(new MyItem
        {
            Name = ItemEntry.Text.Trim(),
            ImagePath = newPath
        });
        SaveItems();
        ItemEntry.Text = string.Empty;
        _selectedPhoto = null;
        ItemImage.Source = "dotnet_bot.png";
        UpdateEmptyView(); // 👈 목록 변경 후 UI 업데이트
    }
    private void OnDeleteItemClicked(object sender, EventArgs e)
    {
        var itemToDelete = (MyItem)((Button)sender).CommandParameter;
        if (MyItems.Contains(itemToDelete))
        {
            if (File.Exists(itemToDelete.ImagePath))
            {
                File.Delete(itemToDelete.ImagePath);
            }
            MyItems.Remove(itemToDelete);
            SaveItems();
        }
        UpdateEmptyView(); // 👈 목록 변경 후 UI 업데이트
    }
    private void SaveItems()
    {
        try
        {
            var itemsJson = JsonConvert.SerializeObject(MyItems);
            Preferences.Set("my_items_list", itemsJson);
        }
        catch (Exception ex)
        {
            DisplayAlert("저장 오류", "소지품 목록을 저장하는 데 실패했습니다.", "확인");
        }
    }
    private void LoadItems()
    {
        try
        {
            var itemsJson = Preferences.Get("my_items_list", string.Empty);
            if (!string.IsNullOrEmpty(itemsJson))
            {
                var items = JsonConvert.DeserializeObject<List<MyItem>>(itemsJson);
                if (items != null)
                {
                    MyItems.Clear();
                    foreach (var item in items)
                    {
                        MyItems.Add(item);
                    }
                }
            }
        }
        catch (Exception ex)
        {
            DisplayAlert("로딩 오류", "소지품 목록을 불러오는 데 실패했습니다. 데이터를 초기화합니다.", "확인");
            Preferences.Remove("my_items_list");
        }
        UpdateEmptyView(); // 👈 목록 로드 후 UI 업데이트
    }
}