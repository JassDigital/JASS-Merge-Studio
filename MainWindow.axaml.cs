using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;

namespace JASS_Merge_Studio;

public partial class MainWindow : Window
{
    private string? _leftPath;
    private string? _rightPath;

    public MainWindow()
    {
        InitializeComponent();
    }

    private async Task<string?> PickTextFileAsync(string title)
    {
        var files = await StorageProvider.OpenFilePickerAsync(
            new FilePickerOpenOptions
            {
                Title = title,
                AllowMultiple = false,
                FileTypeFilter =
                [
                    new FilePickerFileType("Text and source files")
                    {
                        Patterns = ["*.txt", "*.md", "*.cs", "*.py", "*.js",
                                    "*.ts", "*.json", "*.xml", "*.yaml",
                                    "*.yml", "*.ini", "*.csv", "*.sql",
                                    "*.html", "*.css", "*.axaml", "*.log"]
                    },
                    FilePickerFileTypes.All
                ]
            });

        return files.Count == 0 ? null : files[0].TryGetLocalPath();
    }

    private async void OpenLeft_Click(object? sender, RoutedEventArgs e)
    {
        try
        {
            var path = await PickTextFileAsync("Choose left version");
            if (path is null) return;

            LeftText.Text = await File.ReadAllTextAsync(path);
            _leftPath = path;
            LeftPath.Text = path;
            StatusText.Text = "Left file loaded.";
            CompareIfReady();
        }
        catch (Exception ex) { Error("Opening left file", ex); }
    }

    private async void OpenRight_Click(object? sender, RoutedEventArgs e)
    {
        try
        {
            var path = await PickTextFileAsync("Choose right version");
            if (path is null) return;

            RightText.Text = await File.ReadAllTextAsync(path);
            _rightPath = path;
            RightPath.Text = path;
            StatusText.Text = "Right file loaded.";
            CompareIfReady();
        }
        catch (Exception ex) { Error("Opening right file", ex); }
    }

    private void Compare_Click(object? sender, RoutedEventArgs e) => CompareIfReady();

    private void CompareIfReady()
    {
        if (_leftPath is null || _rightPath is null)
        {
            DiffSummary.Text = "Load both files before comparing.";
            return;
        }

        var left = Lines(LeftText.Text ?? "");
        var right = Lines(RightText.Text ?? "");
        var count = Math.Max(left.Length, right.Length);
        var differences = new List<string>();

        for (var i = 0; i < count; i++)
        {
            var l = i < left.Length ? left[i] : "<no line>";
            var r = i < right.Length ? right[i] : "<no line>";

            if (!string.Equals(l, r, StringComparison.Ordinal))
                differences.Add($"Line {i + 1}: - {Preview(l)}  |  + {Preview(r)}");
        }

        DiffList.ItemsSource = differences;
        DiffSummary.Text = differences.Count == 0
            ? $"Files match exactly · {left.Length} lines."
            : $"{differences.Count} differing line position(s) · " +
              $"{left.Length} left lines · {right.Length} right lines.";

        StatusText.Text = differences.Count == 0
            ? "Comparison complete · no differences."
            : $"Comparison complete · {differences.Count} differing line position(s).";
    }

    private static string[] Lines(string value) =>
        value.Replace("\r\n", "\n").Replace('\r', '\n').Split('\n');

    private static string Preview(string value)
    {
        value = value.Replace("\t", "    ");
        return value.Length > 100 ? value[..100] + "…" : value;
    }

    private void UseLeft_Click(object? sender, RoutedEventArgs e)
    {
        MergedText.Text = LeftText.Text ?? "";
        StatusText.Text = "Merge output initialized from the left version. Review before saving.";
    }

    private void UseRight_Click(object? sender, RoutedEventArgs e)
    {
        MergedText.Text = RightText.Text ?? "";
        StatusText.Text = "Merge output initialized from the right version. Review before saving.";
    }

    private async void SaveMerged_Click(object? sender, RoutedEventArgs e)
    {
        try
        {
            var file = await StorageProvider.SaveFilePickerAsync(
                new FilePickerSaveOptions
                {
                    Title = "Save merged text",
                    SuggestedFileName = "merged.txt",
                    FileTypeChoices =
                    [
                        new FilePickerFileType("Text file") { Patterns = ["*.txt"] },
                        new FilePickerFileType("All files") { Patterns = ["*"] }
                    ]
                });

            var path = file?.TryGetLocalPath();
            if (string.IsNullOrWhiteSpace(path)) return;

            await File.WriteAllTextAsync(path, MergedText.Text ?? "");
            StatusText.Text = $"Merged output saved · {path}";
        }
        catch (Exception ex) { Error("Saving merged output", ex); }
    }

    private void Error(string operation, Exception ex) =>
        StatusText.Text = $"{operation} failed: {ex.Message}";
}
