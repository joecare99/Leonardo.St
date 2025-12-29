using System;

namespace Leonardo.ViewModels.Interfaces;

/// <summary>
/// Defines a contract for file dialog operations, providing a platform-independent abstraction
/// for displaying file selection dialogs to the user.
/// </summary>
/// <remarks>
/// <para>
/// This interface serves as an abstraction layer for file dialogs, enabling the ViewModel layer
/// to interact with file selection functionality without directly depending on platform-specific
/// implementations (e.g., WPF's <c>OpenFileDialog</c>, WinForms' <c>SaveFileDialog</c>, or
/// Avalonia's file picker APIs).
/// </para>
/// <para>
/// By implementing <see cref="IDisposable"/>, this interface ensures that any underlying
/// unmanaged resources (such as native dialog handles) can be properly released when the
/// dialog is no longer needed.
/// </para>
/// <para>
/// Typical implementations of this interface wrap platform-specific file dialog classes
/// and expose their core functionality through this unified API.
/// </para>
/// </remarks>
/// <example>
/// <code>
/// using (IFileDialog dialog = fileDialogFactory.CreateOpenFileDialog())
/// {
///     dialog.Filter = "Text Files (*.txt)|*.txt|All Files (*.*)|*.*";
///     dialog.FileName = "document.txt";
///     
///     if (dialog.ShowDialog())
///     {
///         string selectedFile = dialog.FileName;
///         // Process the selected file
///     }
/// }
/// </code>
/// </example>
public interface IFileDialog : IDisposable
{
    /// <summary>
    /// Gets or sets the filter string that determines which file types are displayed
    /// in the file dialog.
    /// </summary>
    /// <value>
    /// A string containing the file filter pattern. The format follows the standard
    /// Windows file dialog filter syntax: <c>"Description|Pattern"</c>, with multiple
    /// filters separated by <c>|</c> characters.
    /// </value>
    /// <remarks>
    /// <para>
    /// The filter string uses a pipe-delimited format where each filter consists of
    /// a description followed by the pattern. Multiple filters can be specified by
    /// separating them with additional pipe characters.
    /// </para>
    /// <para>
    /// Format: <c>"Description1|Pattern1|Description2|Pattern2|..."</c>
    /// </para>
    /// </remarks>
    /// <example>
    /// <code>
    /// // Single filter for text files
    /// dialog.Filter = "Text Files (*.txt)|*.txt";
    /// 
    /// // Multiple filters
    /// dialog.Filter = "Image Files (*.png;*.jpg)|*.png;*.jpg|All Files (*.*)|*.*";
    /// 
    /// // Filter for Leonardo project files
    /// dialog.Filter = "Leonardo Files (*.leo)|*.leo|XML Files (*.xml)|*.xml";
    /// </code>
    /// </example>
    string Filter { get; set; }

    /// <summary>
    /// Gets or sets the file name selected in the file dialog.
    /// </summary>
    /// <value>
    /// <para>
    /// When set before calling <see cref="ShowDialog"/>: The default file name
    /// that appears in the dialog's file name input field.
    /// </para>
    /// <para>
    /// When retrieved after <see cref="ShowDialog"/> returns <see langword="true"/>:
    /// The full path of the file selected by the user.
    /// </para>
    /// <para>
    /// Returns <see langword="null"/> if no file has been selected or if the dialog
    /// was cancelled.
    /// </para>
    /// </value>
    /// <remarks>
    /// <para>
    /// This property serves a dual purpose depending on when it is accessed:
    /// </para>
    /// <list type="bullet">
    /// <item>
    /// <description>
    /// <b>Before ShowDialog:</b> Setting this property pre-populates the file name
    /// field in the dialog, providing a suggested default name for the user.
    /// </description>
    /// </item>
    /// <item>
    /// <description>
    /// <b>After ShowDialog:</b> Reading this property returns the complete file path
    /// that the user selected, including the directory path and file extension.
    /// </description>
    /// </item>
    /// </list>
    /// </remarks>
    /// <example>
    /// <code>
    /// // Set a default file name before showing the dialog
    /// dialog.FileName = "NewDocument.txt";
    /// 
    /// if (dialog.ShowDialog())
    /// {
    ///     // Retrieve the full path of the selected file
    ///     string fullPath = dialog.FileName; // e.g., "C:\Users\Documents\MyFile.txt"
    /// }
    /// </code>
    /// </example>
    string? FileName { get; set; }

    /// <summary>
    /// Displays the file dialog to the user and waits for the user to select a file
    /// or cancel the operation.
    /// </summary>
    /// <returns>
    /// <see langword="true"/> if the user selected a file and confirmed the selection
    /// (e.g., by clicking "OK" or "Save"); <see langword="false"/> if the user cancelled
    /// the dialog or closed it without making a selection.
    /// </returns>
    /// <remarks>
    /// <para>
    /// This method is a blocking call that displays the native file dialog and waits
    /// for user interaction. The calling thread will be blocked until the user either
    /// confirms their selection or cancels the dialog.
    /// </para>
    /// <para>
    /// After this method returns <see langword="true"/>, the <see cref="FileName"/>
    /// property will contain the full path of the selected file. If the method returns
    /// <see langword="false"/>, the <see cref="FileName"/> property should not be
    /// relied upon and may contain its previous value or <see langword="null"/>.
    /// </para>
    /// <para>
    /// Implementations should ensure that the <see cref="Filter"/> property is applied
    /// to restrict the visible file types, and that any value set in <see cref="FileName"/>
    /// is used as the default file name in the dialog.
    /// </para>
    /// </remarks>
    /// <example>
    /// <code>
    /// dialog.Filter = "All Files (*.*)|*.*";
    /// 
    /// bool userConfirmed = dialog.ShowDialog();
    /// 
    /// if (userConfirmed)
    /// {
    ///     Console.WriteLine($"User selected: {dialog.FileName}");
    /// }
    /// else
    /// {
    ///     Console.WriteLine("User cancelled the dialog.");
    /// }
    /// </code>
    /// </example>
    bool ShowDialog();
}