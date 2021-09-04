using UnityEngine;
using System.Collections;

public class HitManager : MonoBehaviour
{
	public string id = "cube1";
	
	public int hits = 0;
	
	private TextMesh myTextMesh;
	
	// Use this for initialization
	void Start ()
	{
		myTextMesh = GetComponentInChildren<TextMesh>();
		
		if(!CryptoPlayerPrefs.HasKey(getPrefKey())) {
			SetHits(0);
		} else {
			SetHits(CryptoPlayerPrefs.GetInt(getPrefKey(), 0));
		}
	}
	
    void OnCollisionEnter(Collision collision) {
       	SetHits(hits+1);
    }
	
	void SetHits(int hits) {
		this.hits = hits;
		Save();
		myTextMesh.text = hits.ToString();
	}
		
	private void Save() {
		CryptoPlayerPrefs.SetInt(getPrefKey(), hits);
		PlayerPrefs.SetInt(getPrefKey(), hits);
		CryptoPlayerPrefs.Save();
	}
		
	private string getPrefKey() {
		return "cpp_test_hits_" + id;
	}
}

