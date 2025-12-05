using Leonardo.ViewModels.Interfaces;
using System;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Platform.Storage;
using System.Collections.Generic;
using Avalonia;
using System.Linq;

/// <summary>
/// Provides an Avalonia-specific implementation of the <see cref="ISaveFileDialog"/> interface,
/// wrapping the Avalonia <see cref="FilePickerSaveOptions"/> to enable save file dialog functionality
/// within the Leonardo application.
/// </summary>
/// <remarks>
/// <para>
/// This class serves as a platform-specific adapter that bridges the abstract <see cref="ISaveFileDialog"/>
/// interface with Avalonia's native file picker API. It enables the ViewModel layer to request file save
/// operations without being coupled to Avalonia-specific types.
/// </para>
/// <para>
/// The implementation converts between the Windows-style filter string format (used by the interface)
/// and Avalonia's <see cref="FilePickerFileType"/> collection format. This ensures compatibility with
/// existing code that expects the traditional pipe-delimited filter syntax.
/// </para>
/// <para>
/// <b>Threading Considerations:</b> The <see cref="ShowDialog"/> method blocks the calling thread
/// by calling <c>.Result</c> on the async file picker operation. This is intentional to maintain
/// compatibility with the synchronous <see cref="ISaveFileDialog"/> interface contract, but should
/// be used carefully in UI contexts to avoid deadlocks.
/// </para>
/// <para>
/// <b>Platform Requirements:</b> This class requires an active Avalonia application with a
/// <see cref="IClassicDesktopStyleApplicationLifetime"/> and a valid <see cref="IStorageProvider"/>
/// on the main window.
/// </para>
/// </remarks>
/// <example>
/// <code>
/// using ISaveFileDialog dialog = new SaveFileProxy();
/// dialog.Filter = "Text Files (*.txt)|*.txt|All Files (*.*)|*.*";
/// dialog.FileName = "document.txt";
/// 
/// if (dialog.ShowDialog())
/// {
///     string savedPath = dialog.FileName;
///     // File path selected by user, proceed with save operation
/// }
/// </code>
/// </example>
/// <seealso cref="ISaveFileDialog"/>
/// <seealso cref="FilePickerSaveOptions"/>
/// <seealso cref="IStorageProvider"/>
public class SaveFileProxy : ISaveFileDialog
{
    /// <summary>
    /// The underlying Avalonia file picker save options used to configure and display the save dialog.
    /// </summary>
    private FilePickerSaveOptions fileDialog = new FilePickerSaveOptions();

    /// <summary>
    /// Gets or sets the filter string that determines which file types are available
    /// in the save file dialog.
    /// </summary>
    /// <value>
    /// <para>
    /// When getting: A pipe-delimited string reconstructed from the current
    /// <see cref="FilePickerSaveOptions.FileTypeChoices"/> collection.
    /// </para>
    /// <para>
    /// When setting: A Windows-style filter string in the format
    /// <c>"Description|Pattern|Description2|Pattern2|..."</c> that will be parsed
    /// and converted to Avalonia's <see cref="FilePickerFileType"/> collection.
    /// </para>
    /// </value>
    /// <remarks>
    /// <para>
    /// The getter reconstructs the filter string by iterating through the file type choices
    /// and formatting each as <c>"Name|Patterns"</c>. Note that this reconstruction may not
    /// produce an identical string to what was originally set, as Avalonia stores patterns
    /// differently than the Windows filter format.
    /// </para>
    /// <para>
    /// The setter parses the pipe-delimited string and creates <see cref="FilePickerFileType"/>
    /// instances for each description/pattern pair. Patterns within a single filter should be
    /// space-separated when multiple extensions are needed (e.g., <c>"*.jpg *.png"</c>).
    /// </para>
    /// </remarks>
    /// <example>
    /// <code>
    /// // Setting a filter with multiple file types
    /// proxy.Filter = "Image Files|*.jpg *.png|Text Files|*.txt";
    /// 
    /// // The above creates two FilePickerFileType entries:
    /// // 1. "Image Files" with patterns ["*.jpg", "*.png"]
    /// // 2. "Text Files" with patterns ["*.txt"]
    /// </code>
    /// </example>
    public string Filter { get => string.Join("|", fileDialog.FileTypeChoices.Select(s=>$"{s.Name}|{s.ToString()}")); set => SetDialogFilter(value); }

    /// <summary>
    /// Gets or sets the file name for the save dialog.
    /// </summary>
    /// <value>
    /// <para>
    /// When set before calling <see cref="ShowDialog"/>: The suggested default file name
    /// that appears in the dialog's file name input field.
    /// </para>
    /// <para>
    /// When retrieved after <see cref="ShowDialog"/> returns <see langword="true"/>:
    /// The full local file path selected by the user for saving.
    /// </para>
    /// <para>
    /// May be <see langword="null"/> if no file name has been set or selected.
    /// </para>
    /// </value>
    /// <remarks>
    /// <para>
    /// Before showing the dialog, this property value is copied to
    /// <see cref="FilePickerSaveOptions.SuggestedFileName"/> to pre-populate
    /// the file name field for user convenience.
    /// </para>
    /// <para>
    /// After a successful dialog interaction, this property contains the
    /// <see cref="IStorageFile.Path"/> converted to a local file system path
    /// via <see cref="Uri.LocalPath"/>.
    /// </para>
    /// </remarks>
    /// <example>
    /// <code>
    /// proxy.FileName = "MyDocument.txt";  // Set suggested name
    /// 
    /// if (proxy.ShowDialog())
    /// {
    ///     // FileName now contains full path like "C:\Users\...\MyDocument.txt"
    ///     File.WriteAllText(proxy.FileName, content);
    /// }
    /// </code>
    /// </example>
    public string? FileName { get ; set; }

    /// <summary>
    /// Parses and applies a Windows-style filter string to the Avalonia file picker options.
    /// </summary>
    /// <param name="value">
    /// A pipe-delimited filter string in the format <c>"Description|Pattern|Description2|Pattern2|..."</c>.
    /// Patterns for a single filter type should be space-separated if multiple extensions are needed.
    /// </param>
    /// <remarks>
    /// <para>
    /// This method converts the traditional Windows file dialog filter format to Avalonia's
    /// <see cref="FilePickerFileType"/> collection. The conversion process:
    /// </para>
    /// <list type="number">
    /// <item><description>Splits the input string by pipe (<c>|</c>) characters</description></item>
    /// <item><description>Iterates through pairs of description and pattern strings</description></item>
    /// <item><description>Creates <see cref="FilePickerFileType"/> instances with patterns split by spaces</description></item>
    /// <item><description>Assigns the resulting collection to <see cref="FilePickerSaveOptions.FileTypeChoices"/></description></item>
    /// </list>
    /// <para>
    /// <b>Important:</b> This method assumes the input string has an even number of pipe-delimited
    /// segments. An odd number of segments will cause an <see cref="IndexOutOfRangeException"/>.
    /// </para>
    /// </remarks>
    /// <example>
    /// <code>
    /// // Input: "Text Files|*.txt|Images|*.jpg *.png *.gif"
    /// // Result: Two FilePickerFileType entries:
    /// //   - "Text Files" with Patterns = ["*.txt"]
    /// //   - "Images" with Patterns = ["*.jpg", "*.png", "*.gif"]
    /// </code>
    /// </example>
    private void SetDialogFilter(string value)
    {
        var sp = value.Split('|');
        List<FilePickerFileType> fileTypes = new();
        for (int i = 0; i < sp.Length; i += 2)
        {
            fileTypes.Add(new(sp[i]) { Patterns = sp[i + 1].Split(' ') });
        }
        fileDialog.FileTypeChoices = fileTypes;
    }


    /// <summary>
    /// Releases any resources used by the <see cref="SaveFileProxy"/> instance.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This implementation is currently empty as the <see cref="FilePickerSaveOptions"/>
    /// class does not hold unmanaged resources that require explicit disposal.
    /// </para>
    /// <para>
    /// The method is provided to satisfy the <see cref="IDisposable"/> contract inherited
    /// from <see cref="ISaveFileDialog"/>, ensuring consistent usage patterns across
    /// different platform implementations that may require resource cleanup.
    /// </para>
    /// </remarks>
    public void Dispose()
    {
    }

    /// <summary>
    /// Displays the Avalonia save file picker dialog and waits for the user to select
    /// a save location or cancel the operation.
    /// </summary>
    /// <returns>
    /// <see langword="true"/> if the user selected a file location and confirmed;
    /// <see langword="false"/> if the user cancelled the dialog or closed it without selecting.
    /// </returns>
    /// <remarks>
    /// <para>
    /// This method performs the following operations:
    /// </para>
    /// <list type="number">
    /// <item>
    /// <description>
    /// Validates that the application has a valid <see cref="IClassicDesktopStyleApplicationLifetime"/>
    /// with an accessible <see cref="IStorageProvider"/> on the main window.
    /// </description>
    /// </item>
    /// <item>
    /// <description>
    /// Copies the current <see cref="FileName"/> value to the dialog's
    /// <see cref="FilePickerSaveOptions.SuggestedFileName"/> property.
    /// </description>
    /// </item>
    /// <item>
    /// <description>
    /// Invokes the asynchronous <see cref="IStorageProvider.SaveFilePickerAsync"/> method
    /// and blocks until completion using <c>.Result</c>.
    /// </description>
    /// </item>
    /// <item>
    /// <description>
    /// If a file was selected, updates <see cref="FileName"/> with the local file path
    /// and returns <see langword="true"/>.
    /// </description>
    /// </item>
    /// </list>
    /// <para>
    /// <b>Threading Warning:</b> This method blocks the calling thread by accessing
    /// <c>.Result</c> on an async operation. When called from the UI thread in certain
    /// scenarios, this could potentially cause deadlocks. However, Avalonia's file picker
    /// is designed to handle this pattern safely in typical desktop application contexts.
    /// </para>
    /// </remarks>
    /// <exception cref="NullReferenceException">
    /// Thrown when the application does not have a valid <see cref="IClassicDesktopStyleApplicationLifetime"/>
    /// or when the main window's <see cref="IStorageProvider"/> is not available. This typically
    /// indicates that the method was called before the application was fully initialized or
    /// in a non-desktop application context.
    /// </exception>
    /// <example>
    /// <code>
    /// var proxy = new SaveFileProxy();
    /// proxy.Filter = "Text Files|*.txt";
    /// proxy.FileName = "output.txt";
    /// 
    /// if (proxy.ShowDialog())
    /// {
    ///     // User selected a location
    ///     await File.WriteAllTextAsync(proxy.FileName, "Hello, World!");
    /// }
    /// else
    /// {
    ///     // User cancelled
    /// }
    /// </code>
    /// </example>
    public bool ShowDialog()
    {
        // See IoCFileOps project for an example of how to accomplish this.
        if (Application.Current?.ApplicationLifetime is not IClassicDesktopStyleApplicationLifetime desktop ||
            desktop.MainWindow?.StorageProvider is not { } provider)
            throw new NullReferenceException("Missing StorageProvider instance.");

        fileDialog.SuggestedFileName = FileName;
        var file = provider.SaveFilePickerAsync(fileDialog).Result;

        if (file != null)
        {
            FileName = file.Path.LocalPath;
            return true;
        }

        return false;
    }

}