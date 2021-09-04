using UnityEngine;
using System.Collections;

public class CryptoPlayerPrefsManager : MonoBehaviour {
	public int salt = int.MaxValue;
	public bool useRijndael = true;
	public bool useXor = true;
	
	// Use this for initialization
	void Awake () {
		CryptoPlayerPrefs.setSalt(salt);
		CryptoPlayerPrefs.useRijndael(useRijndael);
		CryptoPlayerPrefs.useXor(useXor);
		Destroy(this);
	}
}
