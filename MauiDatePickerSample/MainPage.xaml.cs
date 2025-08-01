namespace MauiDatePickerSample
{
    public partial class MainPage : ContentPage
    {
        int count = 0;

        public MainPage()
        {
            InitializeComponent();
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            var picker = new MauiDatePicker.Pages.DatePickerFa();
            await Navigation.PushModalAsync(picker);

            var result = await picker.ShowAsync();
            if (result != null)
            {
                var a = result;
                //DateTime currentYear = result.Value;
                //UpdateCalendar();
            }
            else
            {
                // کاربر انصراف داد
            }
        }

        private void OnCounterClicked(object? sender, EventArgs e)
        {
            count++;

            if (count == 1)
                CounterBtn.Text = $"Clicked {count} time";
            else
                CounterBtn.Text = $"Clicked {count} times";

            SemanticScreenReader.Announce(CounterBtn.Text);
        }
    }
}
