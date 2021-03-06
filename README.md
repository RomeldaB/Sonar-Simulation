# Simulation of an Imaging Sonar
A program that can simulate a sonar image of a 3D scene using ray tracing.

## Tools

- The platform used - [Unity](https://unity.com)
- The language used - C#
- Scene taken from - [Unity Assets](https://assetstore.unity.com)

## Initialization

- [Download Unity](https://docs.unity3d.com/Manual/GettingStartedInstallingHub.html) - Download Unity Hub and follow the instructions on the above mentioned webpage, based on the provided operating system. The unity version used for this project is 2019.4.25.
- Add the project folder to Unity Projects and open it.

## Instructions

> Open Scene:
- Go to *Assets > Underwater Cave Environment Pack > Scenes >*
- Double click on: Underwater Cave Environment
- Enter the Play Mode in the Game section to see how the program works.

> Camera movement:
- Press the mouse down to be able to change the camera positon and direction.
- Moving the cursor causes the camera to rotate along the vertical axis.
- The arrows (or keys *W*, *A*, *S*, *D*) make the camera move back, forth, left and right into scene.
- Keys *E* and *Q* make the camera move up and down, respectively.

> Change of parameters
- The cursor needs to be visible (press *Esc* if the mouse has already been pressed and is connected to the camera movement)
- By pressing the key *C*, the cursor can change the values of angle and range in their respective horizontal sliders on the top right part of the scene.
- Press *C* again if the mouse is needed to be captured.

> Image generation
- After moving the camera to the wanted position, the image of how the sonar would represent that part of the scene can be generated by pressing *Space*.
- The image is saved on the project main directory under the name: 'sonar.png', together with three images having different amount of Perlin noise.
- To generate a new image, simply move the camera again and type *Space*. The image preview on the top left side of the scene will be changed accordingly.

> Ray visualization
- This functionality is disabled by default as it slows down the process of image formation.
- In order to be able to see the rays after they are cast, uncomment line 115 in *GenerateSonarImage.cs* script.
