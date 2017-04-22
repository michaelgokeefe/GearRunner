using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class VehicleColorProfile {
    public string ProfileName;
    public Texture VehicleTexture;
    


}

public class VehicleColor : MonoBehaviour {

    public VehicleColorProfile[] VehicleColorProfiles;
    public Renderer[] AffectedRenderers;
    private int _ProfileIndex;
    public string ProfileName;

    void Start() {
        if (!string.IsNullOrEmpty(ProfileName)) {
            SetColorProfile(ProfileName);
        }
    }

    void SetColorProfile(string profileName) {
        ProfileName = profileName;
        _ProfileIndex = -1;
        for (int i=0; i< VehicleColorProfiles.Length; i++) {
            if (VehicleColorProfiles[i].ProfileName == profileName) {
                _ProfileIndex = i;
                break;
            }

        }

        if (_ProfileIndex != -1) {
            for (int i= 0; i< AffectedRenderers.Length; i++) {
                AffectedRenderers[i].material.mainTexture = VehicleColorProfiles[_ProfileIndex].VehicleTexture;
            }
        }
        else {
            Debug.LogError("We didn't find a profile match!");
        }

    }
}
