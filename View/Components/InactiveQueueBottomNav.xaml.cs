using Microsoft.Maui.Controls;

namespace MyKaraoke.View.Components
{
    public partial class InactiveQueueBottomNav : ContentView
    {
        public event EventHandler LocaisClicked;
        public event EventHandler CantoresClicked;
        public event EventHandler NovaFilaClicked;
        public event EventHandler BandasMusicosClicked;
        public event EventHandler HistoricoClicked;

        public InactiveQueueBottomNav()
        {
            InitializeComponent();
        }

        private void OnLocaisClicked(object sender, EventArgs e)
        {
            LocaisClicked?.Invoke(sender, e);
        }

        private void OnCantoresClicked(object sender, EventArgs e)
        {
            CantoresClicked?.Invoke(sender, e);
        }

        private void OnNovaFilaClicked(object sender, EventArgs e)
        {
            NovaFilaClicked?.Invoke(sender, e);
        }

        private void OnBandasMusicosClicked(object sender, EventArgs e)
        {
            BandasMusicosClicked?.Invoke(sender, e);
        }

        private void OnHistoricoClicked(object sender, EventArgs e)
        {
            HistoricoClicked?.Invoke(sender, e);
        }
    }
}