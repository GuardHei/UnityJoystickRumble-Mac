using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public static class MacXboxControllerRumbleUtil {

	#region SDL2 Bridge

	/*
	 * This part of work is based on the project SDL2# - C# Wrapper for SDL2 by Ethan "flibitijibibo" Lee <flibitijibibo@flibitijibibo.com>
	 * Copyright (c) 2013-2016 Ethan Lee.
	 * https://github.com/flibitijibibo/SDL2-CS
	 */

	public const string nativeLibName = "SDL2";
	public const uint SDL_INIT_HAPTIC = 0x00001000;

	// todo customized rumble effects
	
	public const ushort SDL_HAPTIC_CONSTANT = 1 << 0;
	public const ushort SDL_HAPTIC_SINE = 1 << 1;
	public const ushort SDL_HAPTIC_LEFTRIGHT = 1 << 2;
	public const ushort SDL_HAPTIC_TRIANGLE = 1 << 3;
	public const ushort SDL_HAPTIC_SAWTOOTHUP = 1 << 4;
	public const ushort SDL_HAPTIC_SAWTOOTHDOWN = 1 << 5;
	public const ushort SDL_HAPTIC_SPRING = 1 << 7;
	public const ushort SDL_HAPTIC_DAMPER = 1 << 8;
	public const ushort SDL_HAPTIC_INERTIA = 1 << 9;
	public const ushort SDL_HAPTIC_FRICTION = 1 << 10;
	public const ushort SDL_HAPTIC_CUSTOM = 1 << 11;
	public const ushort SDL_HAPTIC_GAIN = 1 << 12;
	public const ushort SDL_HAPTIC_AUTOCENTER = 1 << 13;
	public const ushort SDL_HAPTIC_STATUS = 1 << 14;
	public const ushort SDL_HAPTIC_PAUSE = 1 << 15;

	public const byte SDL_HAPTIC_POLAR = 0;
	public const byte SDL_HAPTIC_CARTESIAN = 1;
	public const byte SDL_HAPTIC_SPHERICAL = 2;

	public const uint SDL_HAPTIC_INFINITY = 4292967295U;

	// Operation State Int
	public const int SDL_SUCCESS = 0;

	// CPP BOOL
	public static class SDL_BOOL {
		public const int SDL_FALSE = 0;
		public const int SDL_TRUE = 1;
	}

	[StructLayout(LayoutKind.Sequential)]
	public unsafe struct SDL_HapticDirection {
		public byte type;
		public fixed int dir[3];
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct SDL_HapticConstant {
		public ushort type;
		public SDL_HapticDirection direction;

		public uint length;
		public ushort delay;

		public ushort button;
		public ushort interval;

		public short level;

		public ushort attack_length;
		public ushort attack_level;
		public ushort fade_length;
		public ushort fade_level;
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct SDL_HapticPeriodic {
		public ushort type;
		public SDL_HapticDirection direction;

		public uint length;
		public ushort delay;

		public ushort button;
		public ushort interval;

		public ushort period;
		public short magnitude;
		public short offset;
		public ushort phase;

		public ushort attack_length;
		public ushort attack_level;
		public ushort fade_length;
		public ushort fade_level;
	}

	[StructLayout(LayoutKind.Sequential)]
	public unsafe struct SDL_HapticCondition {
		public ushort type;
		public SDL_HapticDirection direction;

		public uint length;
		public ushort delay;

		public ushort button;
		public ushort interval;

		public fixed ushort right_sat[3];
		public fixed ushort left_sat[3];
		public fixed short right_coeff[3];
		public fixed short left_coeff[3];
		public fixed ushort deadband[3];
		public fixed short center[3];
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct SDL_HapticRamp {
		public ushort type;
		public SDL_HapticDirection direction;

		public uint length;
		public ushort delay;

		public ushort button;
		public ushort interval;

		public short start;
		public short end;

		public ushort attack_length;
		public ushort attack_level;
		public ushort fade_length;
		public ushort fade_level;
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct SDL_HapticLeftRight {
		public ushort type;

		public uint length;

		public ushort large_magnitude;
		public ushort small_magnitude;
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct SDL_HapticCustom {
		public ushort type;
		public SDL_HapticDirection direction;

		public uint length;
		public ushort delay;

		public ushort button;
		public ushort interval;

		public byte channels;
		public ushort period;
		public ushort samples;
		public IntPtr data;

		public ushort attack_length;
		public ushort attack_level;
		public ushort fade_length;
		public ushort fade_level;
	}

	[StructLayout(LayoutKind.Explicit)]
	public struct SDL_HapticEffect {
		[FieldOffset(0)] public ushort type;
		[FieldOffset(0)] public SDL_HapticConstant constant;
		[FieldOffset(0)] public SDL_HapticPeriodic periodic;
		[FieldOffset(0)] public SDL_HapticCondition condition;
		[FieldOffset(0)] public SDL_HapticRamp ramp;
		[FieldOffset(0)] public SDL_HapticLeftRight leftright;
		[FieldOffset(0)] public SDL_HapticCustom custom;
	}

	[DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
	private static extern int SDL_Init(uint flags);

	[DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
	private static extern void SDL_Quit();

	[DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
	private static extern uint SDL_WasInit(uint flags);

	[DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
	private static extern void SDL_HapticClose(IntPtr haptic);

	[DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
	private static extern void SDL_HapticDestroyEffect(IntPtr haptic, int effect);

	[DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
	private static extern int SDL_HapticEffectSupported(IntPtr haptic, ref SDL_HapticEffect effect);

	[DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
	private static extern int SDL_HapticGetEffectStatus(IntPtr haptic, int effect);

	[DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
	private static extern int SDL_HapticIndex(IntPtr haptic);

	[DllImport(nativeLibName, EntryPoint = "SDL_HapticName", CallingConvention = CallingConvention.Cdecl)]
	private static extern IntPtr INTERNAL_SDL_HapticName(int device_index);

	private static string SDL_HapticName(int device_index) {
		return UTF8_ToManaged(INTERNAL_SDL_HapticName(device_index));
	}

	[DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
	private static extern int SDL_HapticNewEffect(IntPtr haptic, ref SDL_HapticEffect effect);

	[DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
	private static extern int SDL_HapticNumAxes(IntPtr haptic);

	[DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
	private static extern int SDL_HapticNumEffects(IntPtr haptic);

	[DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
	private static extern int SDL_HapticNumEffectsPlaying(IntPtr haptic);

	[DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
	private static extern IntPtr SDL_HapticOpen(int device_index);

	[DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
	private static extern int SDL_HapticOpened(int device_index);

	[DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
	private static extern int SDL_HapticPause(IntPtr haptic);

	[DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
	private static extern uint SDL_HapticQuery(IntPtr haptic);

	[DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
	private static extern int SDL_HapticRumbleInit(IntPtr haptic);

	[DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
	private static extern int SDL_HapticRumblePlay(IntPtr haptic, float strength, uint length);

	[DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
	private static extern int SDL_HapticRumbleStop(IntPtr haptic);

	[DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
	private static extern int SDL_HapticRumbleSupported(IntPtr haptic);

	[DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
	private static extern int SDL_HapticRunEffect(IntPtr haptic, int effect, uint iterations);

	[DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
	private static extern int SDL_HapticSetAutocenter(IntPtr haptic, int autocenter);

	[DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
	private static extern int SDL_HapticSetGain(IntPtr haptic, int gain);

	[DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
	private static extern int SDL_HapticStopAll(IntPtr haptic);

	[DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
	private static extern int SDL_HapticStopEffect(IntPtr haptic, int effect);

	[DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
	private static extern int SDL_HapticUnpause(IntPtr haptic);

	[DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
	private static extern int SDL_HapticUpdateEffect(IntPtr haptic, int effect, ref SDL_HapticEffect data);

	[DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
	private static extern int SDL_NumHaptics();

	[DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
	private static extern IntPtr SDL_malloc(IntPtr size);

	[DllImport(nativeLibName, CallingConvention = CallingConvention.Cdecl)]
	private static extern void SDL_free(IntPtr memblock);

	private static unsafe string UTF8_ToManaged(IntPtr s, bool freePtr = false) {
		if (s == IntPtr.Zero) return null;
		byte* ptr = (byte*) s;
		while (*ptr != 0) {
			ptr++;
		}

		string result = System.Text.Encoding.UTF8.GetString((byte*) s, (int) (ptr - (byte*) s));
		if (freePtr) SDL_free(s);
		return result;
	}

	#endregion

	public static bool IsInitiated => SDL_WasInit(SDL_INIT_HAPTIC) == SDL_BOOL.SDL_TRUE;
	public static int DeviceNum => SDL_NumHaptics();

	public static string[] DeviceNames {
		get {
			int deviceNum = DeviceNum;
			if (deviceNum < 0) {
				return null;
			}

			string[] deviceNames = new string[deviceNum];
			for (int i = 0; i < deviceNum; i++) {
				deviceNames[i] = SDL_HapticName(i);
			}

			return deviceNames;
		}
	}

	private static Dictionary<int, IntPtr> availableDevices = new Dictionary<int, IntPtr>();

	static MacXboxControllerRumbleUtil() {
		SDL_Init(SDL_INIT_HAPTIC);
	}

	public static bool Rumble(int deviceIndex, float strength, int milliseconds) {
		if (!CheckAndInitDevice(deviceIndex)) return false;
		if (milliseconds < 0) return false;
		IntPtr device = availableDevices[deviceIndex];
		return SDL_HapticRumblePlay(device, strength, (uint) milliseconds) == SDL_SUCCESS;
	}

	public static bool Rumble(float strength, int milliseconds) {
		return Rumble(0, strength, milliseconds);
	}

	public static bool Rumble(int milliseconds = 1000) {
		return Rumble(0, 1, milliseconds);
	}

	public static void StopRumble(int deviceIndex) {
		if (availableDevices.ContainsKey(deviceIndex)) return;
		SDL_HapticRumbleStop(availableDevices[deviceIndex]);
	}

	public static void StopRumbleAll() {
		foreach (var pair in availableDevices) {
			SDL_HapticRumbleStop(pair.Value);
		}
	}

	private static bool CheckAndInitDevice(int deviceIndex) {
		if (availableDevices.ContainsKey(deviceIndex)) return true;
		if (deviceIndex >= DeviceNum) return false;
		IntPtr ptr = SDL_HapticOpen(deviceIndex);
		if (SDL_HapticRumbleSupported(ptr) == SDL_BOOL.SDL_FALSE) return false;
		if (SDL_HapticRumbleInit(ptr) != SDL_SUCCESS) return false;

		availableDevices[deviceIndex] = ptr;
		Debug.Log("Register Device [" + deviceIndex + "] " + SDL_HapticName(deviceIndex) + " !");
		return true;
	}
}