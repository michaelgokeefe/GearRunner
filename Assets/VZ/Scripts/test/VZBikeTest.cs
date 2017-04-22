//***********************************************************************
// Copyright 2014 VirZOOM
//***********************************************************************

using UnityEngine;
using UnityEngine.UI;
using System;
using System.IO;

public class VZBikeTest : MonoBehaviour
{
   TextMesh mText;

   void Start()
   {
      // Lookup text objects
      mText = GetComponent<TextMesh>();
   }

   void Update()
   {
      var controller = VZPlayer.Controller;
      var state = controller.BikeState();

      mText.text = 
         "Type: " + TypeText(state.Type) + "\n" +
         "Connected: " + state.Connected + "\n" +
         "SenderAddress: " + state.Sender() + "\n" +
         "HeartRate: " + state.HeartRate + "\n" +
         "BatteryVolts: " + state.BatteryVolts + "\n" +
         "Speed: " + controller.InputSpeed + "\n" +
         "Resistance: " + state.FilteredResistance + "\n" +
         "LeftGrip: " + GripText(controller.LeftButton.Down, controller.DpadUp.Down, controller.DpadDown.Down, controller.DpadLeft.Down, controller.DpadRight.Down) + "\n" +
         "RightGrip: " + GripText(controller.RightButton.Down, controller.RightUp.Down, controller.RightDown.Down, controller.RightLeft.Down, controller.RightRight.Down);
   }

   string TypeText(int type)
   {
      if (type < 0)
         return "none";
      else if (type == 0)
         return "wired";
      else if (type == 1)
         return "alpha";
      else if (type == 2)
         return "beta";
      else
         return "reserved";
   }

   string GripText(bool trigger, bool up, bool down, bool left, bool right)
   {
      string text = "";

      if (trigger)
         text += "trigger ";
      if (up)
         text += "up ";
      if (down)
         text += "down ";
      if (left)
         text += "left ";
      if (right)
         text += "right ";

      return text;
   }
}
