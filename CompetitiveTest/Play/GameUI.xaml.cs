namespace SSU.CompetitiveTest.Play {

    using System;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;
    using Microsoft.Win32;

    public partial class GameUI : UserControl {

        #region Fields

        private Game currentGame;

        #endregion

        #region Methods

        public GameUI() {
            InitializeComponent();
            currentGame = new Games.GuessTheNumber();
            DataContext = currentGame;
        }

        private void selectPlayerHandler(Object sender, RoutedEventArgs e) {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Multiselect = true;
            dlg.Filter = "Виконуємі файли (.exe)|*.exe";
            if (dlg.ShowDialog() == true) {
                Int32 selected = Math.Max(playerList.SelectedIndex, 0), cnt, n = currentGame.Players.Count, i;
                if (n > 0) {
                    for (cnt = i = 0; cnt < n && i < dlg.FileNames.Length; ++cnt, ++i) {
                        currentGame.Players[(selected + i) % n].Path = dlg.FileNames[i];
                    }
                }
            }
        }

        private void playHandler(Object sender, RoutedEventArgs e) {
            currentGame.Play();
        }

        #endregion

        private void TextBlock_MouseLeftButtonDown_1(object sender, MouseButtonEventArgs e) {
            MessageBox.Show(e.XButton1 + " " + e.XButton2);
        }

    }

}
