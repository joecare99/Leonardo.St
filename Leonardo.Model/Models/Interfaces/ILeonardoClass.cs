using BaseLib.Interfaces;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Threading.Tasks;

namespace Leonardo.Models.Interfaces;

/// <summary>
/// Defines the contract for the main Leonardo application class that handles image encryption,
/// decryption, and communication with the Hugging Face AI service.
/// </summary>
/// <remarks>
/// This interface provides functionality for:
/// <list type="bullet">
///     <item><description>Encrypting and decrypting images using steganography techniques</description></item>
///     <item><description>Communicating with Hugging Face AI models for image generation</description></item>
///     <item><description>Managing user interface callbacks for dialogs and messages</description></item>
/// </list>
/// Implements <see cref="INotifyPropertyChanged"/> to support data binding in UI frameworks.
/// </remarks>
public interface ILeonardoClass : INotifyPropertyChanged
{
    /// <summary>
    /// Gets or sets the callback function used to prompt the user for a file save location.
    /// </summary>
    /// <value>
    /// A function that takes a filter string as input and returns the selected file path,
    /// or <see langword="null"/> if the user cancels the dialog. The property itself can be
    /// <see langword="null"/> if no save file dialog is configured.
    /// </value>
    Func<string, string?>? SaveFileQuery { get; set; }

    /// <summary>
    /// Gets or sets the callback action used to display message boxes to the user.
    /// </summary>
    /// <value>
    /// An action that takes a message string to display. Can be <see langword="null"/>
    /// if no message box functionality is configured.
    /// </value>
    Action<string>? MessageBoxShow { get; set; }

    /// <summary>
    /// Gets the current decrypted image ready for display in a picture box control.
    /// </summary>
    /// <value>
    /// A <see cref="Bitmap"/> containing the decrypted image, or <see langword="null"/>
    /// if no image has been decrypted yet.
    /// </value>
    Bitmap? PicBoxDECImg { get; }

    /// <summary>
    /// Gets the current status or result message text to be displayed to the user.
    /// </summary>
    /// <value>
    /// A string containing informational messages about the current operation status,
    /// error messages, or operation results.
    /// </value>
    string MessageText { get; }

    /// <summary>
    /// Gets or sets the callback function used to prompt the user for text input.
    /// </summary>
    /// <value>
    /// A function that takes a prompt message as input and returns the user's input string.
    /// Can be <see langword="null"/> if no input dialog is configured.
    /// </value>
    Func<string, string>? InputQuery { get; set; }

    /// <summary>
    /// Gets or sets the callback action invoked to show a loading or generating indicator.
    /// </summary>
    /// <value>
    /// An action that displays a visual indicator that a long-running operation is in progress.
    /// Can be <see langword="null"/> if no loading indicator is configured.
    /// </value>
    Action? ShowGeneratingMessage { get; set; }

    /// <summary>
    /// Gets or sets the callback action invoked to hide the loading or generating indicator.
    /// </summary>
    /// <value>
    /// An action that hides the visual indicator after a long-running operation completes.
    /// Can be <see langword="null"/> if no loading indicator is configured.
    /// </value>
    Action? HideGeneratingMessage { get; set; }

    /// <summary>
    /// Sends an asynchronous request to the Hugging Face AI service to generate an image.
    /// </summary>
    /// <returns>
    /// A <see cref="Task"/> representing the asynchronous operation. The generated image
    /// can be accessed through the <see cref="PicBoxDECImg"/> property upon completion.
    /// </returns>
    /// <remarks>
    /// This method initiates communication with the Hugging Face API to generate images
    /// based on configured parameters. The operation progress and results are communicated
    /// through the <see cref="ShowGeneratingMessage"/>, <see cref="HideGeneratingMessage"/>,
    /// and <see cref="MessageText"/> members.
    /// </remarks>
    Task HuggingRequest();

    /// <summary>
    /// Sends an asynchronous request to the Hugging Face AI service to generate an image
    /// and subsequently encrypts the specified text into the generated image.
    /// </summary>
    /// <param name="text">The text message to encrypt and embed into the generated image using steganography.</param>
    /// <returns>
    /// A <see cref="Task"/> representing the asynchronous operation.
    /// </returns>
    /// <remarks>
    /// This method combines image generation with steganographic encryption, allowing
    /// hidden messages to be embedded within AI-generated images.
    /// </remarks>
    Task HuggingRequest2ENC(string text);

    /// <summary>
    /// Prompts the user for a decryption password and decrypts the hidden message from the specified image file.
    /// </summary>
    /// <param name="fileName">The full path to the image file containing the encrypted hidden message.</param>
    /// <remarks>
    /// This method uses steganography techniques to extract and decrypt hidden data from images.
    /// The user will be prompted for the decryption password via the <see cref="InputQuery"/> callback.
    /// Results and error messages are communicated through the <see cref="MessageText"/> property.
    /// </remarks>
    void PromptAndDecrypt(string fileName);

    /// <summary>
    /// Prompts the user for an encryption password and encrypts a message into the specified image file.
    /// </summary>
    /// <param name="fileName">The full path to the image file where the hidden message will be embedded.</param>
    /// <remarks>
    /// This method uses steganography techniques to embed encrypted data within images.
    /// The user will be prompted for the message and encryption password via the <see cref="InputQuery"/> callback.
    /// The save location is determined via the <see cref="SaveFileQuery"/> callback.
    /// </remarks>
    void PromptAndEncrypt(string fileName);

    /// <summary>
    /// Configures the console output interface for logging and debugging purposes.
    /// </summary>
    /// <param name="console">
    /// An <see cref="IConsole"/> implementation that will receive log output and debugging information.
    /// </param>
    /// <remarks>
    /// This method allows the application to redirect console output to different targets,
    /// such as a UI text control, file logger, or debug output window.
    /// </remarks>
    void SetConsole(IConsole console);
}