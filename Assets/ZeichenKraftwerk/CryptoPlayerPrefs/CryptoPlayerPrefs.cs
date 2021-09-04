using System;
using UnityEngine;
using System.Collections.Generic;
using System.IO;

public static class CryptoPlayerPrefs
{
	#region New PlayerPref Stuff
	/// <summary>
	/// Returns true if key exists in the preferences.
	/// </summary>
	public static bool HasKey (string key)
	{
		// Get hashed key
		string cKey = hashedKey (key);
		
		return PlayerPrefs.HasKey (cKey);
	}
	
	/// <summary>
	/// Removes key and its corresponding value from the preferences.
	/// </summary>
	public static void DeleteKey (string key)
	{
		// Get hashed key
		string cKey = hashedKey (key);
		
		PlayerPrefs.DeleteKey (cKey);
	}
	
	/// <summary>
	/// Removes all keys and values from the preferences. Use with caution.
	/// </summary>
	public static void DeleteAll ()
	{
		PlayerPrefs.DeleteAll ();
	}
	
	/// <summary>
	/// Writes all modified preferences to disk.
	/// </summary>
	public static void Save ()
	{
		PlayerPrefs.Save ();
	}
	#endregion
	
	#region New PlayerPref Setters
	/// <summary>
	/// Sets the value of the preference identified by key.
	/// </summary>
	public static void SetInt (string key, int val)
	{	
		// Get crypted key
		string cKey = hashedKey (key);
		
		int cryptedInt = val;
		
		// If enabled use xor algo
		if (_useXor) {
			// Get crypt helper values
			int xor = computeXorOperand (key, cKey);
			int ad = computePlusOperand (xor);
		
			// Compute crypted int
			cryptedInt = (val + ad) ^ xor;
		}
		
		// If enabled use rijndael algo
		if (_useRijndael) {
			// Save
			PlayerPrefs.SetString (cKey, encrypt (cKey, string.Empty + cryptedInt));
		} else {
			PlayerPrefs.SetInt (cKey, cryptedInt);
		}
		
	}
	
	/// <summary>
	/// Sets the value of the preference identified by key.
	/// </summary>
	public static void SetLong (string key, long val)
	{
		SetString(key, val.ToString());
	}
	
	/// <summary>
	/// Sets the value of the preference identified by key.
	/// </summary>
	public static void SetString (string key, string val)
	{
		// Get crypted key
		string cKey = hashedKey (key);
		
		string cryptedString = val;
		// If enabled use xor algo
		if (_useXor) {
			// Get crypt helper values
			int xor = computeXorOperand (key, cKey);
			int ad = computePlusOperand (xor);
		
			// Compute crypted string
			cryptedString = "";
			foreach (char c in val) {
				char cryptedChar = (char)(((int)c + ad) ^ xor);
				cryptedString += cryptedChar;
			}
		}
		
		// If enabled use rijndael algo
		if (_useRijndael) {
			// Save
			PlayerPrefs.SetString (cKey, encrypt (cKey, cryptedString));
		} else {
			PlayerPrefs.SetString (cKey, cryptedString);
		}
	}
	
	/// <summary>
	/// Sets the value of the preference identified by key.
	/// </summary>
	public static void SetFloat (string key, float val)
	{
		SetString (key, val.ToString ());
	}
	#endregion
	
	#region New PlayerPref Getters
	/// <summary>
	/// Returns the value corresponding to key in the preference file if it exists.
	/// If it doesn't exist, it will return defaultValue.
	/// </summary>
	public static int GetInt (string key, int defaultValue)
	{
		// Get crypted key
		string cKey = hashedKey (key);
		
		// If the key doesn't exists, return defaultValue
		if (!PlayerPrefs.HasKey (cKey)) {
			return defaultValue;
		}
		
		// Read storedPref
		int storedPref;
		if(_useRijndael) {
			storedPref = int.Parse (decrypt (cKey));
		} else {
			storedPref = PlayerPrefs.GetInt(cKey);
		}
		
		// If xor algo enabled
		int realValue = storedPref;
		if(_useXor) {
			// Get crypt helper values
			int xor = computeXorOperand (key, cKey);
			int ad = computePlusOperand (xor);
			
			// Compute real value
			realValue = (xor ^ storedPref) - ad;
		}
		
		return realValue;
	}
	
	public static int GetInt(string key) {
		return GetInt(key, 0);
	}

	/// <summary>
	/// Returns the value corresponding to key in the preference file if it exists.
	/// If it doesn't exist, it will return defaultValue.
	/// </summary>
	public static long GetLong (string key, long defaultValue)
	{
		return long.Parse(GetString(key, defaultValue.ToString()));
	}
	
	public static long GetLong(string key) {
		return GetLong(key, 0);
	}
	
	/// <summary>
	/// Returns the value corresponding to key in the preference file if it exists.
	/// If it doesn't exist, it will return defaultValue.
	/// </summary>
	public static string GetString (string key, string defaultValue)
	{
		// Get crypted key
		string cKey = hashedKey (key);
				
		// If the key doesn't exists, return defaultValue
		if (!PlayerPrefs.HasKey (cKey)) {
			return defaultValue;
		}
		
		// Read storedPref
		string storedPref;
		if(_useRijndael) {
			storedPref = decrypt (cKey);
		} else {
			storedPref = PlayerPrefs.GetString(cKey);
		}
		
		// XOR algo enabled?
		string realString = storedPref;
		if(_useXor) {
			// Get crypt helper values
			int xor = computeXorOperand (key, cKey);
			int ad = computePlusOperand (xor);
			
			// Compute real value
			realString = "";
			foreach (char c in storedPref) {
				char realChar = (char)((xor ^ c) - ad);
				realString += realChar;
			}
		}
		
		return realString;
	}
	
	public static string GetString(string key) {
		return GetString(key, "");
	}
	
	/// <summary>
	/// Returns the value corresponding to key in the preference file if it exists.
	/// If it doesn't exist, it will return defaultValue.
	/// </summary>
	public static float GetFloat (string key, float defaultValue)
	{
		return float.Parse (GetString (key, defaultValue.ToString ()));
	}
	
	public static float GetFloat(string key) {
		return GetFloat(key, 0);
	}
	#endregion
	
	#region Crypto
	private static string encrypt (string cKey, string data)
	{
		return ZKW.CryptoPlayerPrefs.Helper.EncryptString (data, getEncryptionPassword (cKey));
	}
		
	private static string decrypt (string cKey)
	{
		return ZKW.CryptoPlayerPrefs.Helper.DecryptString (PlayerPrefs.GetString (cKey), getEncryptionPassword (cKey));
	}
		
	private static Dictionary<string, string> keyHashs;

	private static string hashedKey (string key)
	{
		// Initialize HashKey-Dictionary
		if (keyHashs == null) {
			keyHashs = new Dictionary<string, string> ();
		}
		
		// Check if hashed key already was computed
		if (keyHashs.ContainsKey (key)) {
			// Return computed key
			return keyHashs [key];
		}
		
		// Create hashed key and add to dictionary
		string cKey = hashSum (key);
		keyHashs.Add (key, cKey);
		
		return cKey;
	}
	
	private static Dictionary<string, int> xorOperands;

	private static int computeXorOperand (string key, string cryptedKey)
	{
		if (xorOperands == null) {
			xorOperands = new Dictionary<string, int> ();
		}
		
		if (xorOperands.ContainsKey (key)) {
			return xorOperands [key];
		}
		
		int xorOperand = 0;
		foreach (char c in cryptedKey) {
			xorOperand += (int)c;
		}
		xorOperand += salt;
		
		xorOperands.Add (key, xorOperand);
		return xorOperand;
	}
	
	private static int computePlusOperand (int xor)
	{
		return xor & xor << 1;
	}
		
	public static string hashSum (string strToEncrypt)
	{
		System.Text.UTF8Encoding ue = new System.Text.UTF8Encoding ();
		byte[] bytes = ue.GetBytes (strToEncrypt);

		
		byte[] hashBytes = ZKW.CryptoPlayerPrefs.Helper.hashBytes (bytes);
 
		// Convert the encrypted bytes back to a string (base 16)
		string hashString = "";
 
		for (int i = 0; i < hashBytes.Length; i++) {
			hashString += System.Convert.ToString (hashBytes [i], 16).PadLeft (2, '0');
		}
 
		return hashString.PadLeft (32, '0');
	}

	private static string getEncryptionPassword (string pw)
	{
		return hashSum (pw + salt);
	}
	#endregion
	
	#region Test
	private static bool test (bool use_Rijndael, bool use_Xor)
	{
		// OVerall result
		bool result = true;
		
		// Save user values
		bool user_r = _useRijndael;
		bool user_x = _useXor;
		
		// Set Settings for this test
		CryptoPlayerPrefs.useRijndael(use_Rijndael);
		CryptoPlayerPrefs.useXor(use_Xor);
		
		//// INT Test
		// Zero
		int input = 0;
		string testKey = "cryptotest_int";
		string cKey = CryptoPlayerPrefs.hashedKey (testKey);
		SetInt (testKey, input);
		int read = CryptoPlayerPrefs.GetInt (testKey);
		bool ok = input == read;
		result &= ok;
		Debug.Log ("INT Bordertest Zero: " + (ok ? "ok" : "fail"));
		Debug.Log ("(Key: " + testKey + "; Crypted Key: " + cKey + "; Input value: " + input + "; Saved value: " + PlayerPrefs.GetString (cKey) + "; Return value: " + read + ")");
		
		// Max (Border)
		input = int.MaxValue;
		testKey = "cryptotest_intmax";
		cKey = CryptoPlayerPrefs.hashedKey (testKey);
		CryptoPlayerPrefs.SetInt (testKey, input);
		read = CryptoPlayerPrefs.GetInt (testKey);
		ok = input == read;
		result &= ok;
		Debug.Log ("INT Bordertest Max: " + (ok ? "ok" : "fail"));
		Debug.Log ("(Key: " + testKey + "; Crypted Key: " + cKey + "; Input value: " + input + "; Saved value: " + PlayerPrefs.GetString (cKey) + "; Return value: " + read + ")");
		
		// Min (Border)
		input = int.MinValue;
		testKey = "cryptotest_intmin";
		cKey = CryptoPlayerPrefs.hashedKey (testKey);
		CryptoPlayerPrefs.SetInt (testKey, input);
		read = CryptoPlayerPrefs.GetInt (testKey);
		ok = input == read;
		result &= ok;
		Debug.Log ("INT Bordertest Min: " + (ok ? "ok" : "fail"));
		Debug.Log ("(Key: " + testKey + "; Crypted Key: " + cKey + "; Input value: " + input + "; Saved value: " + PlayerPrefs.GetString (cKey) + "; Return value: " + read + ")");
		
		// 100.000 Random
		testKey = "cryptotest_intrand";
		cKey = CryptoPlayerPrefs.hashedKey (testKey);
		bool randOk = true;
		for (int i = 0; i < 100; i++) {
			int random = (int)UnityEngine.Random.Range (int.MinValue, int.MaxValue);
			input = random;
			CryptoPlayerPrefs.SetInt (testKey, input);
			read = CryptoPlayerPrefs.GetInt (testKey);
			ok = input == read;
			randOk &= ok;
			result &= ok;
			//Debug.Log("(Key: " + testKey + "; Crypted Key: " + cKey +"; Input value: " + input + "; Saved value: " + PlayerPrefs.GetInt(cKey) + "; Return value: " + read + ")");
		}

		Debug.Log ("INT Test Random: " + (randOk ? "ok" : "fail"));
		
		
		//// Float (Based on string -> String tests aren't required)
		float inputFl = 0;
		testKey = "cryptotest_float";
		cKey = CryptoPlayerPrefs.hashedKey (testKey);
		CryptoPlayerPrefs.SetFloat (testKey, inputFl);
		float readFl = CryptoPlayerPrefs.GetFloat (testKey);
		ok = inputFl.ToString ().Equals (readFl.ToString ());
		result &= ok;
		Debug.Log ("FLOAT Bordertest Zero: " + (ok ? "ok" : "fail"));
		Debug.Log ("(Key: " + testKey + "; Crypted Key: " + cKey + "; Input value: " + inputFl + "; Saved value: " + PlayerPrefs.GetString (cKey) + "; Return value: " + readFl + ")");
		
		// Max (Border)
		inputFl = float.MaxValue;
		testKey = "cryptotest_floatmax";
		cKey = CryptoPlayerPrefs.hashedKey (testKey);
		CryptoPlayerPrefs.SetFloat (testKey, inputFl);
		readFl = CryptoPlayerPrefs.GetFloat (testKey);
		ok = inputFl.ToString ().Equals (readFl.ToString ());
		result &= ok;
		Debug.Log ("FLOAT Bordertest Max: " + (ok ? "ok" : "fail"));
		Debug.Log ("(Key: " + testKey + "; Crypted Key: " + cKey + "; Input value: " + inputFl + "; Saved value: " + PlayerPrefs.GetString (cKey) + "; Return value: " + readFl + ")");
		
		// Min (Border)
		inputFl = float.MinValue;
		testKey = "cryptotest_floatmin";
		cKey = CryptoPlayerPrefs.hashedKey (testKey);
		CryptoPlayerPrefs.SetFloat (testKey, inputFl);
		readFl = CryptoPlayerPrefs.GetFloat (testKey);
		ok = inputFl.ToString ().Equals (readFl.ToString ());
		result &= ok;
		Debug.Log ("FLOAT Bordertest Min: " + (ok ? "ok" : "fail"));
		Debug.Log ("(Key: " + testKey + "; Crypted Key: " + cKey + "; Input value: " + inputFl + "; Saved value: " + PlayerPrefs.GetString (cKey) + "; Return value: " + readFl + ")");
		
		// 100.000 Random
		testKey = "cryptotest_floatrand";
		cKey = CryptoPlayerPrefs.hashedKey (testKey);
		randOk = true;
		for (int i = 0; i < 100; i++) {
			float random = UnityEngine.Random.Range (int.MinValue, int.MaxValue) * UnityEngine.Random.value;
			inputFl = random;
			CryptoPlayerPrefs.SetFloat (testKey, inputFl);
			readFl = CryptoPlayerPrefs.GetFloat (testKey);
			ok = inputFl.ToString ().Equals (readFl.ToString ());
			randOk &= ok;
			result &= ok;
			//Debug.Log("(Key: " + testKey + "; Crypted Key: " + cKey +"; Input value: " + input + "; Saved value: " + PlayerPrefs.GetInt(cKey) + "; Return value: " + read + ")");
		}

		Debug.Log ("FLOAT Test Random: " + (randOk ? "ok" : "fail"));
		
		// Delete created prefs
		DeleteKey("cryptotest_int");
		DeleteKey("cryptotest_intmax");
		DeleteKey("cryptotest_intmin");
		DeleteKey("cryptotest_intrandom");
		DeleteKey("cryptotest_float");
		DeleteKey("cryptotest_floatmax");
		DeleteKey("cryptotest_floatmin");
		DeleteKey("cryptotest_floatrandom");
		
		// Set Baqck uservalues
		useRijndael(user_r);
		useXor(user_x);
		
		return result;
		
	}
	
	/// <summary>
	/// Runs testsuit for CryptoPlayerPrefs Class
	/// </summary>
	public static bool test() {
		bool r1 = test(true, true);
		bool r2 = test(true, false);
		bool r3 = test(false, true);
		bool r4 = test(false, false);
		bool result = r1 && r2 && r3 && r4;
		
		return result;
	}
	#endregion
	
	#region Settings
	private static int salt = int.MaxValue;
	
	/// <summary>
	/// Sets the salt. 
	/// Define this before use of the CryptoPlayerPrefs Class for your project. 
	/// NEVER change this again for this project!
	/// </summary>
	/// <param name='s'>
	/// Salt value
	/// </param>
	public static void setSalt (int s)
	{
		salt = s;
	}
	
	private static bool _useRijndael = true;
	
	/// <summary>
	/// Sets if Rijndael Algo should be used. 
	/// Define this before use of the CryptoPlayerPrefs Class for your project. 
	/// NEVER change this again for this project!
	/// </summary>
	/// <param name='use'>
	/// Use Rijndael or not
	/// </param>
	public static void useRijndael (bool use)
	{
		_useRijndael = use;
	}
	
	private static bool _useXor = true;
	
	/// <summary>
	/// Sets if XOR Algo should be used. 
	/// Define this before use of the CryptoPlayerPrefs Class for your project. 
	/// NEVER change this again for this project!
	/// </summary>
	/// <param name='use'>
	/// Use XOR Algo or not
	/// </param>
	public static void useXor (bool use)
	{
		_useXor = use;
	}
	#endregion
}

