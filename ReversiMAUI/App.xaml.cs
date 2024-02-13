using Reversi.Model;
using Reversi.Persistance;
using ReversiMAUI.ViewModel;
using ReversiMAUI.Persistance;

namespace ReversiMAUI;

public partial class App : Application
{
    private const string SuspendedGameSavePath = "SuspendedGame";

    private AppShell appShell;
	private IReversiDataAccess dataAccess;
	private ReversiGameModel gameModel;
	private ReversiViewModel viewModel;
	private IStore store;

	public App()
	{
		InitializeComponent();

		store = new ReversiStore();
		dataAccess = new ReversiFileDataAccess(FileSystem.AppDataDirectory);
		gameModel = new ReversiGameModel(dataAccess);
		viewModel = new ReversiViewModel(gameModel);

		appShell = new AppShell(store, gameModel, viewModel, dataAccess)
		{
			BindingContext = viewModel
		};
		MainPage = appShell;
	}
    protected override Window CreateWindow(IActivationState? activationState)
    {
            Window window = base.CreateWindow(activationState);
        window.Created += (s, e) =>
        {
            gameModel.NewGame();
            if (viewModel.Buttons != null)
            {
                viewModel.Buttons.Clear();
            }
            gameModel.CheckValidPlaces();
            viewModel.GenerateButtons();
            appShell.StartTimer();
        };
        window.Activated += (s, e) =>
        {
            if (!File.Exists(Path.Combine(FileSystem.AppDataDirectory, SuspendedGameSavePath)))
                return;

            Task.Run(async () =>
            {
                try
                {
                    await gameModel.LoadGameAsync(SuspendedGameSavePath);
                    appShell.StartTimer();
                }
                catch
                {
                }
            });
        };
        window.Deactivated += (s, e) =>
        {
            
                Task.Run(async () =>
                {
                    try
                    {
                        appShell.StopTimer();
                        await gameModel.SaveGameAsync(SuspendedGameSavePath);
                    }
                    catch
                    {
                    }
                });
        };

        return window;
    }
}
