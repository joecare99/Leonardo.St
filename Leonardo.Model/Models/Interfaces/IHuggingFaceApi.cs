using System.Threading.Tasks;

namespace Leonardo.Models.Interfaces;

public interface IHuggingFaceApi
{
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
}