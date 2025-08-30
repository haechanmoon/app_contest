using Newtonsoft.Json;
using System.Reflection;

namespace ShinhanLostAndFound;

// 👇 여기에 FirebaseSettings 클래스를 붙여넣습니다.
public class FirebaseSettings
{
    public string FirebaseDatabaseUrl { get; set; }
    public string FirebaseStorageBucket { get; set; }
}

public partial class App : Application
{
    // 👇 모든 페이지가 공유할 수 있는 정적(static) 변수를 만듭니다.
    public static FirebaseSettings FirebaseSettings { get; private set; }

    public App()
    {
        InitializeComponent();
        LoadFirebaseSettings(); // 👈 앱이 시작될 때 설정을 로드합니다.
        MainPage = new AppShell();
    }

    // 👇 secrets.json 파일을 읽어서 설정을 로드하는 함수
    private static void LoadFirebaseSettings()
    {
        var assembly = Assembly.GetExecutingAssembly();
        using var stream = assembly.GetManifestResourceStream("ShinhanLostAndFound.secrets.json");
        using var reader = new StreamReader(stream);
        var json = reader.ReadToEnd();
        FirebaseSettings = JsonConvert.DeserializeObject<FirebaseSettings>(json);
    }
}