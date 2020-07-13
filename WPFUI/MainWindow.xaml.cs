using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using Engine.EventArgs;
using Engine.Models;
using Engine.Services;
using Engine.ViewModels;

namespace WPFUI
{
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly GameSession _gameSession;
        private readonly MessageBroker _messageBroker = MessageBroker.GetInstance();

        private readonly Dictionary<Key, Action> _userInputActions =
            new Dictionary<Key, Action>();

        public MainWindow()
        {
            InitializeComponent();

            InitializeUserInputActions();

            _messageBroker.OnMessageRaised += OnGameMessageRaised;

            _gameSession = SaveGameService.LoadLastSaveOrCreateNew();

            DataContext = _gameSession;
        }

        private void InitializeUserInputActions()
        {
            _userInputActions.Add(Key.W, () => _gameSession.MoveNorth());
            _userInputActions.Add(Key.A, () => _gameSession.MoveWest());
            _userInputActions.Add(Key.S, () => _gameSession.MoveSouth());
            _userInputActions.Add(Key.D, () => _gameSession.MoveEast());

            _userInputActions.Add(Key.Z, () => _gameSession.AttackCurrentMonster());
            _userInputActions.Add(Key.C, () => _gameSession.UseCurrentConsumable());

            _userInputActions.Add(Key.Q, () => SetTabFocus("QuestsTabItem"));
            _userInputActions.Add(Key.R, () => SetTabFocus("RecipesTabItem"));
            _userInputActions.Add(Key.I, () => SetTabFocus("InventoryTabItem"));
            _userInputActions.Add(Key.X, () => SetTabFocus("ScrollsTabItem"));
            _userInputActions.Add(Key.T, () => OnClick_DisplayTradeScreen(this, new RoutedEventArgs()));
        }

        private void OnGameMessageRaised(object sender, GameMessageEventArgs e)
        {
            GameMessages.Document.Blocks.Add(new Paragraph(new Run(e.Message)));
            GameMessages.ScrollToEnd();
        }

        private void OnClick_MoveNorth(object sender, RoutedEventArgs e)
        {
            _gameSession.MoveNorth();
        }

        private void OnClick_MoveWest(object sender, RoutedEventArgs e)
        {
            _gameSession.MoveWest();
        }

        private void OnClick_MoveEast(object sender, RoutedEventArgs e)
        {
            _gameSession.MoveEast();
        }

        private void OnClick_MoveSouth(object sender, RoutedEventArgs e)
        {
            _gameSession.MoveSouth();
        }

        private void OnClick_AttackMonster(object sender, RoutedEventArgs e)
        {
            _gameSession.AttackCurrentMonster();
        }

        private void OnClick_DisplayTradeScreen(object sender, RoutedEventArgs e)
        {
            if (_gameSession.CurrentTrader == null)
            {
                return;
            }

            var tradeScreen = new TradeScreen
            {
                Owner = this,
                DataContext = _gameSession
            };
            tradeScreen.ShowDialog();
        }

        private void OnClick_UseCurrentConsumable(object sender, RoutedEventArgs e)
        {
            _gameSession.UseCurrentConsumable();
        }

        private void OnClick_Craft(object sender, RoutedEventArgs e)
        {
            var recipe = ((FrameworkElement) sender).DataContext as Recipe;
            _gameSession.CraftItemUsing(recipe);
        }

        private void OnClick_UseScroll(object sender, RoutedEventArgs e)
        {
            var scroll = ((FrameworkElement) sender).DataContext as GameItem;
            _gameSession.UseScroll(scroll);
        }

        private void MainWindow_OnKeyDown(object sender, KeyEventArgs e)
        {
            if (_userInputActions.TryGetValue(e.Key, out var action))
            {
                action.Invoke();
            }
        }

        private void SetTabFocus(string tabName)
        {
            foreach (TabItem item in PlayerDataTabControl.Items)
            {
                if (item is TabItem tabItem)
                {
                    if (tabItem.Name == tabName)
                    {
                        tabItem.IsSelected = true;
                    }
                }
            }
        }

        private void MainWindow_OnClosing(object sender, CancelEventArgs e)
        {
            SaveGameService.Save(_gameSession);
        }
    }
}