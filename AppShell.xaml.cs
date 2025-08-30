using ShinhanLostAndFound.Pages;

namespace ShinhanLostAndFound;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();

        // 탭 메뉴에 없는 별도의 페이지들의 경로를 모두 등록합니다.
        Routing.RegisterRoute(nameof(AddLostItemPage), typeof(AddLostItemPage));
        Routing.RegisterRoute(nameof(DetailPage), typeof(DetailPage));
    }
}