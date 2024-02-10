# Leonardo
Leonardo is a tool that combines steganography, symmetric encryption, and additional AI protection to securely encrypt and embed strings into images.

## Features

- **New:** Encrypt a string into an already existing image using Stegodon (a C# library built for this project) and high-bit AES encryption.
  
- **Crack:** Decrypt any message that was injected through this app.
  
- **Gen:** Insert Stable Diffusion prompt, insert the string you want to encrypt, and save your one-of-a-kind password holder.

## Known Issues

- **Gen HTTP Request:** Gen utilizes an HTTP request to fetch from an API, which can fail when loading the service initially. Restart the application a few times until you establish a connection. We are actively working on fixing this in the next patch series.

## Download

Get the latest binary release [here](https://github.com/2alf/Leonardo/releases/tag/alpha).

Visual documentation will be added soon.

Feel free to contribute or report any issues!
