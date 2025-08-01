using Microsoft.Maui.Controls.Shapes;
using System.Collections.ObjectModel;
using System.Globalization;

namespace MauiDatePicker.Pages;

public partial class YearPickerFa : ContentPage
{
    private int startYear = new PersianCalendar().GetYear(DateTime.Now);
    private const int PageSize = 20; // یا 10
    private List<string?> Years=new();

    int? _selectedYear;
    public int? selectedYear
    {
        get
        {
            return _selectedYear;
        }
        set
        {
            if (_selectedYear != value)
            {
                _selectedYear = value;
            }
        }
    }
    public YearPickerFa(int startYear)
    {
        InitializeComponent();
        this.startYear = startYear;
        LoadYears();
    }

    private void LoadYears()
    {
        try
        {
            int from = startYear;
            int to = from - PageSize - 1;

            RangeLabel.Text = $"{from} - {to}";

            Years = new List<string>();
            for (int year = from; year >= to; year--)
            {
                Years.Add(year.ToString());
            }

            YearCollection.Children.Clear();

            int itm;
            int row = 0, col = 0;
            Years.Reverse();
            foreach (var year in Years)
            {    
                var lbl = new Label
                {
                    Text = year,
                    FontFamily = "Yekan",
                    HorizontalOptions = LayoutOptions.Center,
                    VerticalOptions = LayoutOptions.Center,
                    FontSize = 14,
                    TextColor = Colors.Black
                };

                var border = new Border
                {
                    BackgroundColor = Colors.Transparent,
                    StrokeShape = new RoundRectangle { CornerRadius = 14 },
                    Stroke = Colors.Transparent,
                    Padding = 8,
                    Margin = 4,
                    HeightRequest=40,                  
                    Content = lbl,
                };

                var g  =
                         
                    new TapGestureRecognizer
                    {
                        Command = new Command(() => OnYearTapped(year, border,lbl))
                    };

                border.GestureRecognizers.Add(g);

                YearCollection.Children.Add(border);
                Grid.SetRow(border, row);
                Grid.SetColumn(border, col);
                  
                col++;
                if (col >= 6)
                {
                    col = 0;
                    row++;
                }
            }

            selectedYear = null;
 
            SelectButton.IsEnabled = false;
        }
        catch (Exception ex)
        {

            throw;
        }

    }

    Label BefLbl = null;
    private void focus(object sender, EventArgs e)
    {
        if (sender is Label lbl)
            lbl.TextColor = Colors.White;
    }

    private void unfocus(object sender, EventArgs e)
    {
        if (sender is Label lbl)
            lbl.TextColor = Colors.Black;

    }

    private void OnNextPageClicked(object sender, EventArgs e)
    {
        startYear += PageSize;
        LoadYears();
    }

    private void OnPreviousPageClicked(object sender, EventArgs e)
    {
        startYear -= PageSize;
        LoadYears();
    }
    private Border selectedBorder;
    private void OnYearTapped(string year, Border border,Label lbl)
    {
        if (selectedBorder != null)
            selectedBorder.BackgroundColor = Colors.Transparent;
        selectedBorder = border;
 
        if (int.TryParse(year, out int Year))
        {
            if (BefLbl != null) BefLbl.TextColor = Colors.Black;
            selectedYear = Year;
            lbl.TextColor=Colors.White;
            BefLbl = lbl;
            border.BackgroundColor = Color.FromArgb("#2563EB"); // آبی روشن
            SelectButton.IsEnabled = true;
        }
    }

    private TaskCompletionSource<int?> _tcs;

    public Task<int?> ShowAsync()
    {
        _tcs = new TaskCompletionSource<int?>();
        return _tcs.Task;
    }

    bool OnBusy = false;
    private void OnSelectClicked(object sender, EventArgs e)
    {
        if (OnBusy) return;
        OnBusy = true;
        try
        {
            if (selectedYear != null)
            {
                _tcs?.TrySetResult(selectedYear);
                Navigation.PopModalAsync();
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

    private void OnCloseClicked(object sender, EventArgs e)
    {
        _tcs?.TrySetResult(null); // یا مقدار پیش‌فرض
        Navigation.PopModalAsync();
    }
}