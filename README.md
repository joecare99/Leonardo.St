# Leonardo

![leo_0007_btn-eye-hp](https://github.com/2alf/Leonardo/assets/113948114/c4c8e3e3-c773-43fe-ab0e-966edbbcb015)


Leonardo is a tool that combines steganography, symmetric encryption, and additional AI protection to securely encrypt and embed strings into images.


![GitHub Downloads (all assets, specific tag)](https://img.shields.io/github/downloads/2alf/Leonardo/alpha/total?style=for-the-badge&label=INSTALLS%40v1&color=3E9544)


### Table of Contents


- [Features](#features)
- [Known Issues](#known-issues)
- [Download](#download)
- [Usage](#usage)
- [Support](#support)
- [Credits](#credits)

## Features

> **New:** Encrypt a string into an already existing image using Stegodon (a C# library built for this project) and high-bit AES encryption.

> **Crack:** Decrypt any message that was injected through this app.

> **Gen:** Insert Stable Diffusion prompt, insert the string you want to encrypt, and save your one-of-a-kind password holder.

## Known Issues

IF YOU RECEIVE THE *BAD REQUEST* ERROR YOU ARE USING AN OUTDATED VERSION OF THE PROGRAM. Download the latest build to fix this issue [here](https://github.com/2alf/Leonardo/releases/latest)
This error is made because HF had a token breach and a lot of existing tokens were deleted. This new version has a new token.

> **Gen HTTP Request:** Gen utilizes an HTTP request to fetch from an API, which can fail when loading the service initially. Restart the application a few times until you establish a connection. We are actively working on fixing this in the next patch series.
<img src="https://github.com/2alf/Leonardo/assets/113948114/f9ebabdf-7aa9-402d-81ca-aada743129fb" width="250" alt="image">



## Download

Get the latest binary release [here](https://github.com/2alf/Leonardo/releases/).



![leo_0009_Layer-7](https://github.com/2alf/Leonardo/assets/113948114/9ca30cba-727a-4bc2-b322-ec804b028b93)

Feel free to contribute or report any issues!

## Usage
- [Generation with encryption](#generation-with-encryption)
- [Decryption](#decryption)
- [Copy to clipboard](#copy-to-clipboard)
- [Standard encryption](#standard-encryption)




### Generation with encryption

<img src="https://github.com/2alf/Leonardo/assets/113948114/29f33330-bbe8-4a98-9559-21da2ee48baf" width="500" alt="image">


>Write in the prompt for the image you want to be generated.


<img src="https://github.com/2alf/Leonardo/assets/113948114/163b8174-790f-4751-b669-264bbc11caaf" width="500" alt="image">


>After its done you will get the following popup when your image has been generated. 

>If there are issues with this part restart your application and check the [known issues](#known-issues) tab.


<img src="https://github.com/2alf/Leonardo/assets/113948114/75a989c9-3bbf-41c3-9bad-fa83b54671cb" width="500" alt="image">


>Now enter the password you want to encrypt.


<img src="https://github.com/2alf/Leonardo/assets/113948114/a8519ca8-ac48-4507-a448-5df0ab9d1eb7" width="500" alt="image">


>Save it on your disk and youre done!


<img src="https://github.com/2alf/Leonardo/assets/113948114/1e1a6e40-3e2e-454c-be55-48e258b2ed7f" width="500" alt="ghtest">


>For decryption refer to the decryption tab.






### Decryption

<img src="https://github.com/2alf/Leonardo/assets/113948114/16218cb8-4d52-4a93-8c76-5a08dfa457d4" width="500" alt="image">


>Now choose an image that you used our algorithm on to decrypt and get your password back.


<img src="https://github.com/2alf/Leonardo/assets/113948114/04d9a14d-19bb-41e7-8f93-a036ed83cbb6" width="500" alt="image">


>Congrats! 🥳 You got your password ready for use! Refer to the [clipboard](copy-to-clipboard) to use your password.


<img src="https://github.com/2alf/Leonardo/assets/113948114/632826f0-7d75-4a6f-a6f5-abc5213d0c52" width="500" alt="image">





### Copy to clipboard


>Strings arent displayed as text in the app so to safely use the password use the following button to copy the decrypted string to your 📋 clipboard.


<img src="https://github.com/2alf/Leonardo/assets/113948114/13333449-c617-4593-9fab-efa68a8ee631" width="500" alt="image">





### Standard encryption


>To simply encrypy an password into an already existing image using our algorithm and encryption follow the steps below. 


<img src="https://github.com/2alf/Leonardo/assets/113948114/b673b914-57d1-440d-9359-533608e38428" width="500" alt="image">


>You will then be prompted to select an image after which you will get a popup to store your string.


<img src="https://github.com/2alf/Leonardo/assets/113948114/53c11743-d2ed-4b3b-b515-f19310951320" width="500" alt="image">


>And thats it! Now you just save your new encrypted image wherever you want on your disk!

>To decrypt an image you encrypted with our algorithm look [here](#decryption).





### Support
>You can support the project or simply check if theres any updated binary files by clicking the following button. 

<img src="https://github.com/2alf/Leonardo/assets/113948114/0013eb92-0e16-47cd-8ae6-0ee81abf3406" width="500" alt="image">

<img src="https://github.com/2alf/Leonardo/assets/113948114/7b982b7f-83b6-470f-933a-7466609954f6" width="500" alt="image">

>🙏 **Thank you!** 

[![ko-fi](https://ko-fi.com/img/githubbutton_sm.svg)](https://ko-fi.com/C1C3UABOS)



## Credits

🐶 Dog Images API [dog.ceo](https://dog.ceo/dog-api/)

🤗 HugginFace Model `my-favourite-dog` by [Maheshmarathe](https://huggingface.co/Maheshmarathe/my-favourite-dog).

🐘 Steganography library [Stegodon](https://github.com/2alf/Stegodon) by 2alf.
