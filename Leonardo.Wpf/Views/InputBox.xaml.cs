using Leonardo.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Leonardo.Views;

/// <summary>
/// Represents a modal input dialog window that prompts the user for text input.
/// This WPF window provides a simple, reusable mechanism for collecting single-line
/// text input from users within the Leonardo application.
/// </summary>
/// <remarks>
/// <para>
/// The <see cref="InputBox"/> class follows the MVVM (Model-View-ViewModel) pattern,
/// utilizing <see cref="InputBoxViewModel"/> as its data context to manage the dialog's
/// state and behavior. The view is responsible only for presentation, while all business
/// logic resides in the view model.
/// </para>
/// <para>
/// <b>Design Pattern:</b> This class implements a simplified dialog pattern where:
/// </para>
/// <list type="bullet">
/// <item>
/// <description>The view model handles the message display and input collection</description>
/// </item>
/// <item>
/// <description>A delegate (<see cref="InputBoxViewModel.Close"/>) is injected into the view model
/// to allow it to request window closure without direct window reference</description>
/// </item>
/// <item>
/// <description>The static <see cref="ShowDialog(string)"/> method provides a convenient
/// one-liner API for displaying the dialog and retrieving input</description>
/// </item>
/// </list>
/// <para>
/// <b>Usage Scenarios:</b>
/// </para>
/// <list type="bullet">
/// <item><description>Prompting for file names or identifiers</description></item>
/// <item><description>Requesting user confirmation with custom input</description></item>
/// <item><description>Collecting single-line text data in modal workflow</description></item>
/// </list>
/// <para>
/// <b>XAML Requirements:</b> This class expects a corresponding <c>InputBox.xaml</c> file
/// that defines the visual layout, including controls bound to the view model's
/// <see cref="InputBoxViewModel.Message"/> and <see cref="InputBoxViewModel.Input"/> properties.
/// </para>
/// </remarks>
/// <example>
/// <code>
/// // Simple usage with the static method
/// string userName = InputBox.ShowDialog("Please enter your name:");
/// if (!string.IsNullOrEmpty(userName))
/// {
///     MessageBox.Show($"Hello, {userName}!");
/// }
/// 
/// // Alternative: Creating and configuring the dialog manually
/// var dialog = new InputBox();
/// if (dialog.DataContext is InputBoxViewModel vm)
/// {
///     vm.Message = "Enter project name:";
///     dialog.ShowDialog();
///     string projectName = vm.Input;
/// }
/// </code>
/// </example>
/// <seealso cref="InputBoxViewModel"/>
/// <seealso cref="Window"/>
public partial class InputBox : Window
{
    /// <summary>
    /// Initializes a new instance of the <see cref="InputBox"/> class.
    /// </summary>
    /// <remarks>
    /// <para>
    /// This constructor performs the standard WPF initialization by calling
    /// <see cref="InitializeComponent"/>, which loads and parses the associated
    /// XAML file (<c>InputBox.xaml</c>) and sets up all defined UI elements.
    /// </para>
    /// <para>
    /// The <see cref="FrameworkElement.DataContext"/> is not set in this constructor;
    /// instead, it is configured in the <see cref="OnInitialized"/> method to ensure
    /// proper timing with the WPF initialization lifecycle.
    /// </para>
    /// </remarks>
    public InputBox()
    {
        InitializeComponent();
    }

    /// <summary>
    /// Called when the window has been initialized and raises the <see cref="FrameworkElement.Initialized"/> event.
    /// Sets up the data context and configures the view model's close callback.
    /// </summary>
    /// <param name="e">
    /// An <see cref="EventArgs"/> instance containing the event data.
    /// This parameter is not used directly but is passed to the base implementation.
    /// </param>
    /// <remarks>
    /// <para>
    /// This method performs critical setup operations in the following order:
    /// </para>
    /// <list type="number">
    /// <item>
    /// <description>
    /// Creates a new <see cref="InputBoxViewModel"/> instance and assigns it to
    /// the window's <see cref="FrameworkElement.DataContext"/>.
    /// </description>
    /// </item>
    /// <item>
    /// <description>
    /// Calls the base class implementation to ensure proper event propagation.
    /// </description>
    /// </item>
    /// <item>
    /// <description>
    /// Configures the view model's <see cref="InputBoxViewModel.Close"/> delegate
    /// to invoke this window's <see cref="Window.Close"/> method, enabling the
    /// view model to request window closure without holding a direct reference
    /// to the window instance.
    /// </description>
    /// </item>
    /// </list>
    /// <para>
    /// <b>MVVM Bridge:</b> The close delegate pattern allows the view model to remain
    /// testable and decoupled from WPF-specific types while still being able to
    /// control the dialog's lifecycle. The boolean parameter in the delegate
    /// (currently unused in the lambda) could be used to distinguish between
    /// OK and Cancel scenarios.
    /// </para>
    /// </remarks>
    /// <example>
    /// <code>
    /// // The Close delegate in the ViewModel can be called like this:
    /// // vm.Close?.Invoke(true);  // Request window closure (e.g., on OK button)
    /// // vm.Close?.Invoke(false); // Request window closure (e.g., on Cancel button)
    /// </code>
    /// </example>
    protected override void OnInitialized(EventArgs e)
    {
        DataContext = new InputBoxViewModel(); 
        base.OnInitialized(e);
        if (DataContext is InputBoxViewModel vm)
        {
            vm.Close = (b) => Close();
        }
    }

    /// <summary>
    /// Displays a modal input dialog with the specified prompt message and returns
    /// the user's input.
    /// </summary>
    /// <param name="text">
    /// The message or prompt to display to the user, explaining what input is expected.
    /// This text is assigned to the view model's <see cref="InputBoxViewModel.Message"/> property.
    /// </param>
    /// <returns>
    /// The text entered by the user in the input field. Returns an empty string (<c>""</c>)
    /// if the dialog's data context is not an <see cref="InputBoxViewModel"/> instance,
    /// which would indicate an unexpected configuration error.
    /// </returns>
    /// <remarks>
    /// <para>
    /// This static method provides a convenient, single-call API for displaying an input
    /// dialog and retrieving the result. It encapsulates all the setup and teardown logic
    /// required for modal dialog interaction.
    /// </para>
    /// <para>
    /// <b>Execution Flow:</b>
    /// </para>
    /// <list type="number">
    /// <item><description>Creates a new <see cref="InputBox"/> window instance</description></item>
    /// <item><description>Retrieves the <see cref="InputBoxViewModel"/> from the data context</description></item>
    /// <item><description>Sets the <see cref="InputBoxViewModel.Message"/> property to the provided text</description></item>
    /// <item><description>Displays the dialog modally using <see cref="Window.ShowDialog"/></description></item>
    /// <item><description>After the dialog closes, retrieves and returns the <see cref="InputBoxViewModel.Input"/> value</description></item>
    /// </list>
    /// <para>
    /// <b>Modal Behavior:</b> The calling code will block until the user closes the dialog,
    /// either by clicking an OK/Cancel button (handled by the view model) or by closing
    /// the window directly.
    /// </para>
    /// <para>
    /// <b>Thread Affinity:</b> This method must be called from the UI thread, as it creates
    /// and displays a WPF window.
    /// </para>
    /// </remarks>
    /// <example>
    /// <code>
    /// // Prompt for a file name
    /// string fileName = InputBox.ShowDialog("Enter the name for the new file:");
    /// if (!string.IsNullOrWhiteSpace(fileName))
    /// {
    ///     // Create file with the specified name
    ///     File.Create(Path.Combine(directory, fileName));
    /// }
    /// 
    /// // Prompt for a search term
    /// string searchTerm = InputBox.ShowDialog("Enter search text:");
    /// var results = SearchDocuments(searchTerm);
    /// </code>
    /// </example>
    /// <seealso cref="InputBoxViewModel"/>
    /// <seealso cref="Window.ShowDialog"/>
    public static string ShowDialog(string text)
    {
        var dlg = new InputBox();
        if (dlg.DataContext is InputBoxViewModel vm)
        {
            vm.Message = text;
            dlg.ShowDialog();
            return vm.Input;
        }
        return "";
    }
}