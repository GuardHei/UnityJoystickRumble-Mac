# UnityJoystickRumble-Mac

## Purpose
This project implements the rumble / force feedback function of the joysticks on Mac.

## Origin
As an indie developer, I've been long suffering from lacking of official support on Mac platform. To vibrate the joysticks on Windows, XInput and InControl can do a good job, but still, there is no solution for Mac. Therefore, I started this project.

## How I Make it
I have to admit my implementation is quite weird. Baiscally, I imported the framework from another game engine __SDL-2__, based on C, and tried to access it from C#. Therefore, unexpected problems or bugs might appear.

The idea of using __SDL-2__ came from another popular Mac XBox Controller driver repo [360Controller](https://github.com/360Controller/360Controller) (Also works with Xbox One Controller).

I didn't really make a native plugin but found a solution. To wrap up the C interfaces into C#, I checked this repo [SDL2-CS](https://github.com/flibitijibibo/SDL2-CS) and extracted and edited stuffs to get everything done and explicit.

## Requirements
1. The aimed controller is wired connected via [this driver](https://github.com/360Controller/360Controller) (The repo above). Other drivers might still work because __SDL-2__ lib is pretty platform universal.

2. This repo

## Installation
1. To import a native plugin into Unity for mac, you need a *.bundle* file. This file is located in the __Plugins__ folder of this repo. However, you can also make it yourself. Download the installation file *.dmg* from __SDL-2__'s official site (Make sure it is Development Libraries instead of Runtime Binaries). Then, open the *.dmg* file, you will see the framework file __SDL2.framework__ inside. Just change the suffix *.framework* to *.bundle* and drag it into the __Plugins__ folder of your Unity project. This is because *.framework* is a subform of bundle file on Mac. Unity doesn't recognize framework files. However, during runtime, Mac will swallow it the same as *.bundle*.

2. Make sure this plugin is targeted only for Editor (if the development environment is Mac) and OSX 64 Standalones. Note that this plugin doesn't work with x86 system (32 bit).

3. Import the C# script into your Unity project. You can change the script implementation if you want. Just make it your self.

## How To Use It
To make this tool script simple, I even don't add an extra namespace to it. Just direct call from the static class `MacXboxControllerRumbleUtil`.

### Rumble
To make a rumble, directly add this line of code into your script:

```csharp
MacXboxControllerRumbleUtil.Rumble();
```

The your joystick should rumble for one second. The overrides of the methods are listed below:

```csharp
public static bool Rumble(int deviceIndex, float strength, int milliseconds);
public static bool Rumble(float strength, int milliseconds);
public static bool Rumble(int milliseconds = 1000);
```

1. The first parameter device index is the index of you game controllers. Most of the time should be 0 (the first recognizd device). This lib doesn't recognize normal keyboards and mouses. Actually the 2nd and the 3rd overrides just set the default deviceIndex to 0.

2. The second paramenter strength is the magnitude of rumble between 0 to 1. Default is 1.

3. The third parameter milliseconds is the rumble duration in milliseconds.

4. The return bool value represents whether the execuation is successful (true) or failed (false).

### Stop Rumble

```csharp
public static void StopRumble(int deviceIndex);
public static void StopRumbleAll();
```

### Other Attributes
1. Whether the __SDL-2__ lib has been successfully initiated:

```csharp
public static bool IsInitiated;
```

The initiation should be done automatically in the static constructor. Don't do it manually.

2. The number of haptic devices connected:

```csharp
public static int DeviceNum;
```

This is an accessor property.

3. The names of haptic devices connected:

```csharp
public static string[] DeviceNames;
```

This is an accessor property.

## Customize

I haven't finish every part of the rumble function yet. If you check my script, you can see many public structs and static extern methods modified private. This is because the customized effect rumble function is still under development, including Sine Rumble, Spring Rumble...... But you can try to make it your self.

To customize your own rumble effects, you first have to get an `IntPtr` instance which represents a device. Call the bridge function `SDL_HapticOpen(int device_index)` will return an `IntPrt` instance points to the device at `device_index`.

Then, `new SDL_HapticEffect()` to create the data of your customized effect. I won't go into details of each field, because I am still studying this. But basically, consider the rumble as the movement of waves. Setting up the wave type, direction and other attributes will give your various forms of force feedback. If you want to learn more, you can check the [official __SDL-2__ api](http://wiki.libsdl.org/CategoryForceFeedback) (written in C).

After that, use method `SDL_HapticNewEffect(IntPtr haptic, ref SDL_HapticEffect effect)` to register your customized effect into the system. The return value is int type. Smaller than 0 means a failure registration. Otherwise, the value represent the effect ID. Store this value because it is useful in the next step.

To run a registerd effect, you need method `SDL_HapticRunEffect(IntPtr haptic, int effect, uint iterations)` to make the effect work. Note that the second parameter is the id of the effect you want to call, and iterations means the number of repeatings.

After all, use `SDL_HapticDestroyEffect(IntPtr haptic, int effect)` to destroy the used effects. And then call `SDL_HapticClose(IntPtr haptic)` to close the module. Meanwhile, if you call `SDL_HapticClose(IntPtr haptic)` directly, the registered effects will be automatically destroyed.

I don't recommand you to close the device everytime after rumble, just destroy the effect is enough.

PS: There might be some unsafe codes of some specific effect type, using pointers. Make sure you have change the settings of "allow unsafe code" in Unity Player Settings.