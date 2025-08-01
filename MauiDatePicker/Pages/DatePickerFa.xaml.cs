using System.Globalization;

namespace MauiDatePicker.Pages;

public partial class DatePickerFa : ContentPage
{
    private PersianCalendar persianCalendar = new PersianCalendar();
    private int currentYear;
    private int currentMonth;
    private int currentDay;
    private DateTime? selectedDate = DateTime.Today;
    private TaskCompletionSource<string?> _taskCompletionSource;

    public DatePickerFa()
    {
        InitializeComponent();
        var today = DateTime.Today;
        currentYear = persianCalendar.GetYear(today);
        currentMonth = persianCalendar.GetMonth(today);
        currentDay = persianCalendar.GetDayOfMonth(today);
        UpdateCalendar();
    }

    private void UpdateCalendar()
    {
        var monthName = persianCalendar.GetMonthName(currentMonth);
        MonthLabel.Text = monthName;
        YearLabel.Text = currentYear.ToString();

        DaysGrid.RowDefinitions.Clear();
        DaysGrid.Children.Clear();

        int daysInMonth = GetDaysInMonth(currentYear, currentMonth);
        var firstDayGregorian = persianCalendar.ToDateTime(currentYear, currentMonth, 1, 0, 0, 0, 0);
        int firstWeekDay = ((int)firstDayGregorian.DayOfWeek + 1) % 7; // شمسی: شنبه=0

        int totalCells = firstWeekDay + daysInMonth;
        int totalRows = (int)Math.Ceiling(totalCells / 7.0);

        for (int i = 0; i < totalRows; i++)
        {
            DaysGrid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });
        }

        int day = 1;
        for (int row = 0; row < totalRows; row++)
        {
            for (int col = 0; col < 7; col++)
            {
                int cellIndex = row * 7 + col;

                if (cellIndex < firstWeekDay || day > daysInMonth)
                {
                    DaysGrid.Add(new Label { Text = "" }, col, row);
                }
                else
                {
                    var dayButton = new Button
                    {
                        Text = day.ToString(),
                        BackgroundColor = Colors.Transparent,
                        TextColor = Colors.Black,
                        FontSize = Device.GetNamedSize(NamedSize.Small, typeof(Button)),
                        CornerRadius = 18,
                        WidthRequest = 49,
                        HeightRequest = 36,
                        FontFamily = "Yekan",
                        HorizontalOptions = LayoutOptions.Center,
                        VerticalOptions = LayoutOptions.Center,
                    };

                    int capturedDay = day;
                    var date = persianCalendar.ToDateTime(currentYear, currentMonth, capturedDay, 0, 0, 0, 0);

                    if (currentDay != null && date.Date.Day == currentDay)
                    {
                        selectedDate = date;
                        dayButton.BackgroundColor = Color.FromArgb("#E0E7FF");
                        dayButton.TextColor = Color.FromArgb("#2563EB");
                        dayButton.FontAttributes = FontAttributes.Bold;
                    }

                    dayButton.Clicked += (s, e) =>
                    {
                        selectedDate = date;
                        HighlightSelectedDay();
                        SelectButton.IsEnabled = true;
                    };

                    DaysGrid.Add(dayButton, col, row);
                    day++;
                }
            }
        }
    }

    public Task<string?> ShowAsync()
    {
        _taskCompletionSource = new TaskCompletionSource<string?>();
        return _taskCompletionSource.Task;
    }
    bool OnBusy = false;
    private async void SelectButton_Clicked(object sender, EventArgs e)
    {
        if (OnBusy) return;
        OnBusy = true;

        try
        {
            if (selectedDate.HasValue)
            {
                var pc = new PersianCalendar();
                int year = pc.GetYear(selectedDate.Value);
                int month = pc.GetMonth(selectedDate.Value);
                int day = pc.GetDayOfMonth(selectedDate.Value);

                string persianDateString = $"{year:0000}/{month:00}/{day:00}";
                _taskCompletionSource?.SetResult(persianDateString);

                await Navigation.PopModalAsync();
            }
        }
        catch (Exception)
        {

            throw;
        }
        finally
        {
            OnBusy = false;
        }

    }

    private async void CloseButton_Clicked(object sender, EventArgs e)
    {
        _taskCompletionSource?.SetResult(null);
        await Navigation.PopModalAsync();
    }


    private int GetDaysInMonth(int year, int month)
    {
        if (month <= 6) return 31;
        if (month <= 11) return 30;
        return persianCalendar.IsLeapYear(year) ? 30 : 29;
    }

    private void HighlightSelectedDay()
    {
        foreach (var child in DaysGrid.Children)
        {
            if (child is Button btn && int.TryParse(btn.Text, out int btnDay))
            {
                var btnDate = persianCalendar.ToDateTime(currentYear, currentMonth, btnDay, 0, 0, 0, 0);

                if (selectedDate.HasValue && btnDate.Date == selectedDate.Value.Date)
                {
                    btn.BackgroundColor = Color.FromArgb("#2563EB");
                    btn.TextColor = Colors.White;
                    btn.FontAttributes = FontAttributes.Bold;
                }
                else if (btnDate.Date == DateTime.Today)
                {
                    btn.BackgroundColor = Color.FromArgb("#E0E7FF");
                    btn.TextColor = Color.FromArgb("#2563EB");
                    btn.FontAttributes = FontAttributes.Bold;
                }
                else
                {
                    btn.BackgroundColor = Colors.Transparent;
                    btn.TextColor = Colors.Black;
                    btn.FontAttributes = FontAttributes.None;
                }
            }
        }
    }


    private void PrevMonthButton_Clicked(object sender, EventArgs e)
    {
        currentMonth--;
        if (currentMonth == 0)
        {
            currentMonth = 12;
            currentYear--;
        }
        selectedDate = null;
        SelectButton.IsEnabled = false;
        UpdateCalendar();
    }

    private void NextMonthButton_Clicked(object sender, EventArgs e)
    {
        currentMonth++;
        if (currentMonth == 13)
        {
            currentMonth = 1;
            currentYear++;
        }
        selectedDate = null;
        SelectButton.IsEnabled = false;
        UpdateCalendar();
    }

    private void PrevYearButton_Clicked(object sender, EventArgs e)
    {
        currentYear--;
        selectedDate = null;
        SelectButton.IsEnabled = false;
        UpdateCalendar();
    }

    private void NextYearButton_Clicked(object sender, EventArgs e)
    {
        currentYear++;
        selectedDate = null;
        SelectButton.IsEnabled = false;
        UpdateCalendar();
    }

    private async void OnMonthTapped(object sender, EventArgs e)
    {
        var pickerPage = new MonthPickerFa(currentYear, currentMonth);
        await Navigation.PushModalAsync(pickerPage);

        var result = await pickerPage.ShowAsync();
        if (result is { year: var year, month: var month })
        {
            currentMonth = month;
            currentYear = year;
            UpdateCalendar(); // متد خودت برای رفرش کردن روزها
        }
    }

    private async void OnYearTapped(object sender, EventArgs e)
    {
        var pickerPage = new YearPickerFa(currentYear);
        await Navigation.PushModalAsync(pickerPage);

        var result = await pickerPage.ShowAsync();
        if (result is int year)
        {
            currentYear = year;
            UpdateCalendar(); // متد خودت برای رفرش کردن روزها
        }
    }
}

public static class PersianCalendarExtensions
{
    private static readonly string[] PersianMonthNames =
    {
            "فروردین", "اردیبهشت", "خرداد", "تیر", "مرداد", "شهریور",
            "مهر", "آبان", "آذر", "دی", "بهمن", "اسفند"
        };

    public static string GetMonthName(this PersianCalendar calendar, int month)
    {
        return PersianMonthNames[month - 1];
    }
}