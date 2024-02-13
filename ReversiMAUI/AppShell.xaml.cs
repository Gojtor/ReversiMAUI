using ReversiMAUI.View;
using Reversi.Model;
using Reversi.Persistance;
using ReversiMAUI.ViewModel;

namespace ReversiMAUI;

public partial class AppShell : Shell
{
    private IStore store;
    private StoredGameBrowserModel storedGBM;
    private StoredGameBrowserViewModel storedGBVM;

    private ReversiGameModel model = null!;
    private ReversiViewModel reversiViewModel = null!;
    private IReversiDataAccess dataAccess = null!;

    private IDispatcherTimer timer;
    public AppShell(IStore s,ReversiGameModel m,ReversiViewModel vm, IReversiDataAccess rda)
	{
		InitializeComponent();

        model = m;
        reversiViewModel = vm;
        dataAccess = rda;
        store = s;

        timer = Dispatcher.CreateTimer();
        timer.Interval = TimeSpan.FromSeconds(1);
        timer.Tick += (_, _) => model.Tick();

        model.Advance += Model_Advance;
        model.GameOver += Model_GameOver;

        reversiViewModel.NewGame += ReversiViewModel_NewGame;
        reversiViewModel.PauseGame += ReversiViewModel_PauseGame;
        reversiViewModel.Pass += ReversiViewModel_Pass;
        reversiViewModel.LoadGame += ReversiViewModel_LoadGame;
        reversiViewModel.SaveGame += ReversiViewModel_SaveGame;

        storedGBM = new StoredGameBrowserModel(store);
        storedGBVM = new StoredGameBrowserViewModel(storedGBM);
        storedGBVM.GameLoading += StoredGBVM_GameLoading;
        storedGBVM.GameSaving += StoredGBVM_GameSaving;

	}

    internal void StartTimer() => timer.Start();
    internal void StopTimer() => timer.Stop();
    private async void StoredGBVM_GameSaving(object sender, StoredGameEventArgs e)
    {
        await Navigation.PopAsync();
        timer.Stop();

        try
        {
            await model.SaveGameAsync(e.Name);
            await DisplayAlert("Reversi", "Saved successfully.", "OK");
        }
        catch
        {
            await DisplayAlert("Reversi", "Couldn't save.", "OK");
        }
    }

    private async void StoredGBVM_GameLoading(object sender, StoredGameEventArgs e)
    {
        await Navigation.PopAsync();
        try
        {
            await model.LoadGameAsync(e.Name);
            await Navigation.PopAsync();
            await DisplayAlert("Reversi", "Loaded successfully.", "OK");

            timer.Start();
        }
        catch
        {
            await DisplayAlert("Reversi", "Couldn't load.", "OK");
        }
    }

    private async void ReversiViewModel_SaveGame(object sender, EventArgs e)
    {
        await storedGBM.UpdateAsync();
        await Navigation.PushAsync(new SaveGamePage
        {
            BindingContext = storedGBVM
        });
    }

    private async void ReversiViewModel_LoadGame(object sender, EventArgs e)
    {
        await storedGBM.UpdateAsync();
        await Navigation.PushAsync(new LoadGamePage
        {
            BindingContext = storedGBVM
        });
    }

    private async void Model_GameOver(object sender, ReversiEventArgs e)
    {
        timer.Stop();

        if (e.Player1Won)
        {
            await DisplayAlert("Reversi", "Player 1 won the game!", "OK");
        }
        else if(e.Player2Won)
        {
            await DisplayAlert("Reversi", "Player 2 won the game!", "OK");
        }
        else
        {
            await DisplayAlert("Reversi", "It's a tie !","OK");
        }
    }

    private void ReversiViewModel_Pass(object sender, EventArgs e)
    {
        model.ChangePlayer();
    }

    private void ReversiViewModel_NewGame(object sender, EventArgs e)
    {
        model.NewGame();
        if (reversiViewModel.Buttons != null)
        {
            reversiViewModel.Buttons.Clear();
        }
        model.CheckValidPlaces();
        reversiViewModel.GenerateButtons();
        timer.Start();
    }
    private async void ReversiViewModel_PauseGame(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new PausePage
        {
            BindingContext = reversiViewModel
        });
    }
    private void Model_Advance(object sender, ReversiEventArgs e)
    {
        if (!model.IsGameOver)
        {
            if (e.Player1Pass)
            {
                reversiViewModel.P1PassEnabled = true;
            }
            else
            {
                reversiViewModel.P1PassEnabled = false;
            }
            if (e.Player2Pass)
            {
                reversiViewModel.P2PassEnabled = true;
            }
            else
            {
                reversiViewModel.P2PassEnabled = false;
            }
            if (model.GetCurrentPlayerStatus)
            {
                reversiViewModel.P2Background = Colors.Green;
                reversiViewModel.P1Background = Colors.LightGray;
            }
            else
            {
                reversiViewModel.P1Background = Colors.Green;
                reversiViewModel.P2Background = Colors.LightGray;
            }
        }
    }
}
