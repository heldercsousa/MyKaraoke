using CommunityToolkit.Maui.Views;

namespace MyVocaList.View.Components
{
    /// <summary>
    /// MD3-compliant confirmation popup for destructive actions
    /// </summary>
    public partial class ConfirmationPopup : Popup
    {
        public ConfirmationPopup(string title, string message, string confirmText = "Excluir", string cancelText = "Cancelar")
        {
            InitializeComponent();

            // Set text content
            titleLabel.Text = title;
            messageLabel.Text = message;
            confirmButton.Text = confirmText;
            cancelButton.Text = cancelText;

            // Allow dismissal by tapping backdrop (will return false)
            this.CanBeDismissedByTappingOutsideOfPopup = true;
        }

        /// <summary>
        /// Shows the popup and returns user's choice asynchronously
        /// Uses CommunityToolkit.Maui's built-in ShowPopupAsync
        /// </summary>
        public async Task<bool> ShowAsync()
        {
            var currentPage = Application.Current?.MainPage;
            if (currentPage == null)
            {
                return false;
            }

            // ShowPopupAsync returns the result passed to Close()
            var result = await currentPage.ShowPopupAsync(this);

            // If result is null (backdrop tap) or explicitly false, return false
            // If result is true, return true
            return result is bool boolResult && boolResult;
        }

        private void OnConfirmClicked(object sender, EventArgs e)
        {
            // Pass true to Close() - user confirmed
            Close(true);
        }

        private void OnCancelClicked(object sender, EventArgs e)
        {
            // Pass false to Close() - user cancelled
            Close(false);
        }
    }
}
