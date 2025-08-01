using System.Globalization;

namespace MauiDatePicker.Pages;

public partial class MonthPickerFa : ContentPage
{

    private int selectedYear = new PersianCalendar().GetYear(DateTime.Now);
    private int? selectedMonth = new PersianCalendar().GetMonth(DateTime.Now);
    private TaskCompletionSource<(int year, int month)?> tcs;
    private readonly PersianCalendar persianCalendar = new();

    public MonthPickerFa(int currentYear, int? currentMonth)
    {
        InitializeComponent();

        selectedYear = currentYear;
        selectedMonth = currentMonth;
        YearLabel.Text = selectedYear.ToString();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();

        if (selectedMonth.HasValue)
        {
            var persianMonthName = persianCalendar.GetMonthName(selectedMonth ?? 0);

            foreach (var child in (MonthGrid?.Children))
            {
                if (child is Border border && border.Content is Label lbl)
                {
                    if (lbl.Text == persianMonthName)
                    {
                        // اعمال استایل انتخاب‌شده
                        border.BackgroundColor = Color.FromArgb("#2563EB");
                        lbl.TextColor = Colors.White;
                        lbl.FontAttributes = FontAttributes.Bold;
                    }
                    else
                    {
                        // بازنشانی به حالت پیش‌فرض
                        border.BackgroundColor = Colors.White;
                        lbl.TextColor = Colors.Black;
                        lbl.FontAttributes = FontAttributes.None;
                    }
                }
            }
        }
    }

    public Task<(int year, int month)?> ShowAsync()
    {
        tcs = new TaskCompletionSource<(int year, int month)?>();
        return tcs.Task;
    }

    private void OnPrevYearClicked(object sender, EventArgs e)
    {
        selectedYear--;
        YearLabel.Text = selectedYear.ToString();
    }

    private void OnNextYearClicked(object sender, EventArgs e)
    {
        selectedYear++;
        YearLabel.Text = selectedYear.ToString();
    }

    private void OnYearTapped(object sender, EventArgs e)
    {
        // جایگزینی Label با Entry برای ویرایش مستقیم
        var entry = new Entry
        {
            Text = selectedYear.ToString(),
            HorizontalOptions = LayoutOptions.Center,
            HorizontalTextAlignment = TextAlignment.Center,
            FontSize = 18,
            FontAttributes = FontAttributes.Bold,
            WidthRequest = 100
        };

        entry.Completed += (s, ev) =>
        {
            if (int.TryParse(entry.Text, out var newYear))
                selectedYear = newYear;

            // برگردوندن به حالت لیبل
            YearLabel.Text = selectedYear.ToString();
            YearLabel.IsVisible = true;
            ((Grid)YearLabel.Parent).Children.Remove(entry);
        };

        YearLabel.IsVisible = false;
        ((Grid)YearLabel.Parent).Children.Add(entry);
    }

    private void OnMonthTapped(object sender, EventArgs e)
    {
        if (sender is Label lbl &&
            lbl.Parent is Border border &&
            border.Parent is Grid grid)
        {
            // حذف استایل انتخاب قبلی
            foreach (var child in grid.Children)
            {
                if (child is Border b && b.Content is Label other)
                {
                    b.BackgroundColor = Colors.White;
                    other.TextColor = Colors.Black;
                    other.FontAttributes = FontAttributes.None;
                }
            }

            // اعمال استایل انتخاب فعلی
            border.BackgroundColor = Color.FromArgb("#2563EB");
            //border.BackgroundColor = Colors.White;
            lbl.TextColor = Colors.White;
            lbl.FontAttributes = FontAttributes.Bold;

            selectedMonth = GetMonthIndexFromPersian(lbl.Text);
        }
    }

    private int GetMonthIndexFromPersian(string name)
    {
        return name switch
        {
            "فروردین" => 1,
            "اردیبهشت" => 2,
            "خرداد" => 3,
            "تیر" => 4,
            "مرداد" => 5,
            "شهریور" => 6,
            "مهر" => 7,
            "آبان" => 8,
            "آذر" => 9,
            "دی" => 10,
            "بهمن" => 11,
            "اسفند" => 12,
            _ => 1
        };
    }
    bool OnBusy = false;
    private void OnConfirmClicked(object sender, EventArgs e)
    {
        if (OnBusy) return;
        OnBusy = true;

        try
        {
            if (selectedMonth.HasValue)
            {
                tcs?.TrySetResult((selectedYear, selectedMonth.Value));
            }
            else
            {
                tcs?.TrySetResult(null);
            }


            ClosePage();
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

    private void OnCloseClicked(object sender, EventArgs e)
    {
        tcs?.SetResult(null);
        ClosePage();
    }

    private async void ClosePage()
    {
        await Navigation.PopModalAsync();
    }
}