using System;
using System.Collections.Generic;
using Game.Character;
using Game.Character.CharacterController;
using Game.Character.Stats;
using Game.GlobalComponent;
using Game.GlobalComponent.Qwest;
using Game.GlobalComponent.Training;
using Game.Shop;
using Naxeex.GameModes;
using Roulette;
using UnityEngine;

public class BaseProfile
{
	public static void StoreValue<T>(T value, string key)
	{
		string value2 = MiamiSerializier.JSONSerialize(value);
		PlayerPrefs.SetString("v1_3_" + key, value2);
		PlayerPrefs.Save();
	}

	public static T ResolveValue<T>(string key, T defaultValue)
	{
		string key2 = "v1_3_" + key;
		string @string = PlayerPrefs.GetString(key2);
		object obj;
		try
		{
			obj = MiamiSerializier.JSONDeserialize(@string);
			if (obj is IConvertible)
			{
				IConvertible obj2 = obj as IConvertible;
				return BaseProfile.To<T>(obj2);
			}
		}
		catch (Exception)
		{
			BaseProfile.StoreValue<T>(defaultValue, key);
			return defaultValue;
		}
		if (obj == null)
		{
			BaseProfile.StoreValue<T>(defaultValue, key);
			obj = defaultValue;
		}
		return (T)((object)obj);
	}

	public static T ResolveValueWitoutDefault<T>(string key)
	{
		string text = "v1_3_" + key;
		string @string = PlayerPrefs.GetString(text);
		object obj = null;
		try
		{
			obj = MiamiSerializier.JSONDeserialize(@string);
		}
		catch (Exception)
		{
			UnityEngine.Debug.LogFormat("BaseProfile: Error loading value with key = {0}", new object[]
			{
				text
			});
		}
		if (obj is IConvertible)
		{
			IConvertible obj2 = obj as IConvertible;
			return BaseProfile.To<T>(obj2);
		}
		return (T)((object)obj);
	}

	public static void ClearValue(string key)
	{
		PlayerPrefs.DeleteKey("v1_3_" + key);
	}

	public static void StoreArray<T>(T[] values, string key)
	{
		BaseProfile.StoreValue<int>(values.Length, key + "Length");
		for (int i = 0; i < values.Length; i++)
		{
			BaseProfile.StoreValue<T>(values[i], key + i);
		}
	}

	public static void StoreLastElementOfArray<T>(T value, string key)
	{
		int num = BaseProfile.ResolveValue<int>(key + "Length", 0);
		BaseProfile.StoreValue<int>(num + 1, key + "Length");
		BaseProfile.StoreValue<T>(value, key + num);
	}

	public static T[] ResolveArray<T>(string key)
	{
		int num = BaseProfile.ResolveValue<int>(key + "Length", 0);
		if (num == 0)
		{
			return null;
		}
		List<T> list = new List<T>();
		for (int i = 0; i < num; i++)
		{
			list.Add(BaseProfile.ResolveValueWitoutDefault<T>(key + i));
		}
		return list.ToArray();
	}

	public static void ClearArray<T>(string key)
	{
		int num = BaseProfile.ResolveValue<int>(key + "Length", 0);
		if (num == 0)
		{
			return;
		}
		for (int i = 0; i < num; i++)
		{
			PlayerPrefs.DeleteKey("v1_3_" + key + i);
		}
		BaseProfile.StoreValue<int>(0, key + "Length");
	}

	public static T GetValue<T>(string key)
	{
		string @string = PlayerPrefs.GetString("v1_3_" + key);
		if (@string == null)
		{
			throw new BaseProfile.NullValueStoredExeption();
		}
		object obj = null;
		try
		{
			obj = MiamiSerializier.JSONDeserialize(@string);
		}
		catch (Exception)
		{
			throw new BaseProfile.NullValueStoredExeption();
		}
		if (obj == null)
		{
			throw new BaseProfile.NullValueStoredExeption();
		}
		if (obj is IConvertible)
		{
			IConvertible obj2 = obj as IConvertible;
			return BaseProfile.To<T>(obj2);
		}
		return (T)((object)obj);
	}

	public static void ClearBaseProfileWithoutSystemSettings()
	{
		foreach (string[] array in new List<string[]>
		{
			Enum.GetNames(typeof(StatsList)),
			Enum.GetNames(typeof(WeaponNameList)),
			Enum.GetNames(typeof(QwestProfileList)),
			new string[]
			{
				"SkipTraining"
			}
		})
		{
			foreach (string str in array)
			{
				PlayerPrefs.DeleteKey("v1_3_" + str);
			}
		}
		TrainingManager.ClearCompletedTrainingsInfo();
		ShopManager.ClearBI(false);
		PlayerStoreProfile.ClearLoadout();
		FactionsManager.ClearPlayerRelations();
		PlayerInfoManager.ClearPlayerInfo();
		SurvivalRouletteController.Clear();
		WaveRouletteController.Clear();
		ArenaTutorial.ClearAll();
	}

	public static float SoundVolume
	{
		get
		{
			return (float)BaseProfile.ResolveValue<double>(SystemSettingsList.SoundVolume.ToString(), 1.0);
		}
		set
		{
			BaseProfile.StoreValue<double>((double)value, SystemSettingsList.SoundVolume.ToString());
		}
	}

	public static float MusicVolume
	{
		get
		{
			return BaseProfile.ResolveValue<float>(SystemSettingsList.MusicVolume.ToString(), 1f);
		}
		set
		{
			BaseProfile.StoreValue<float>(value, SystemSettingsList.MusicVolume.ToString());
		}
	}

	public static bool SkipTraining
	{
		get
		{
			return BaseProfile.ResolveValue<bool>("SkipTraining", false);
		}
		set
		{
			BaseProfile.StoreValue<bool>(value, "SkipTraining");
		}
	}

	public static T To<T>(IConvertible obj)
	{
		Type typeFromHandle = typeof(T);
		Type underlyingType = Nullable.GetUnderlyingType(typeFromHandle);
		if (underlyingType == null)
		{
			return (T)((object)Convert.ChangeType(obj, typeFromHandle));
		}
		if (obj == null)
		{
			return default(T);
		}
		return (T)((object)Convert.ChangeType(obj, underlyingType));
	}

	public static bool HasKey(string key)
	{
		return PlayerPrefs.HasKey("v1_3_" + key);
	}

	public static void ClearProfile()
	{
		PlayerPrefs.DeleteAll();
	}

	private const string ArrayLengthInteline = "Length";

	private const string SerializationPrefix = "v1_3_";

	public class NullValueStoredExeption : Exception
	{
	}
}
