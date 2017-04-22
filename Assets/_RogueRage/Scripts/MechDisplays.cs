using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MechDisplays : MonoBehaviour {
    public Text MPHValueText;
    public Text ShieldValueText;


    public void UpdateVelocityDisplay(float velocity) {
        MPHValueText.text = (velocity * 2.24f).ToString("F0");
    }
}
