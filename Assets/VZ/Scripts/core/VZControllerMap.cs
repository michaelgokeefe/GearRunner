//***********************************************************************
// Copyright 2014 VirZOOM
//***********************************************************************

using UnityEngine;
using System;

public class VZControllerMap
{
   public enum ControlType
   {
      Key,
      Axis
   };

   public class Control
   {
      public ControlType Type = ControlType.Key;
      public string Id = "None";
      public float Multiplier = 1;
      public float Offset = 0;
      public KeyCode Code;

      public virtual bool GetBool()
      {
         if (Type == ControlType.Key)
            return Input.GetKey(Code);
         else
            return Input.GetAxis(Id) * Multiplier + Offset > 0;
      }

      public virtual float GetFloat()
      {
         if (Type == ControlType.Key)
            return Input.GetKey(Code) ? 1 : 0;
         else
            return Input.GetAxis(Id) * Multiplier + Offset;
      }

      public void Finalize(int joystick)
      {
         Id = string.Format(Id, joystick);

         if (Type == ControlType.Key)
            Code = (KeyCode)Enum.Parse(typeof(KeyCode), Id);
      }
   }

#if UNITY_ANDROID && !UNITY_EDITOR && UNITY_HAS_GOOGLEVR
   public class GvrControl : Control
   {
      public enum Action 
      {
         Forward,
         Reverse,
         X,
         LeftTrigger,
         RightTrigger,
         LeanLeft,
         LeanRight,
         LeanForward,
         LeanBack
      }

      Action mAction;

      public GvrControl(Action action)
      {
         mAction = action;
      }

      public override bool GetBool()
      {
         return GetFloat() != 0 ? true : false;
      }

      public override float GetFloat()
      {
         switch (mAction)
         {
            case Action.Forward:
            {
               return (GvrController.IsTouching ? 1.2f - GvrController.TouchPos.y : 0);
            }
            case Action.Reverse:
            {
               return 0;
            }
            case Action.X:
            {
               return GvrController.ClickButton ? 1 : 0;
            }
            case Action.LeftTrigger:
            {
               return GvrController.AppButton ? 1 : 0;
            }
            case Action.RightTrigger:
            {
               return GvrController.ClickButton ? 1 : 0;
            }
            case Action.LeanLeft:
            {
               float ang = GvrController.Orientation.eulerAngles.y;
               if (ang > 180)
                  ang -= 360;
               return Mathf.Min(1.0f, -ang / 30);
            }
            case Action.LeanRight:
            {
               float ang = GvrController.Orientation.eulerAngles.y;
               if (ang > 180)
                  ang -= 360;
               return Mathf.Min(1.0f, ang / 30);
            }
            case Action.LeanForward:
            {
               float ang = GvrController.Orientation.eulerAngles.x;
               if (ang > 180)
                  ang -= 360;
               return Mathf.Min(1.0f, ang / 30);
            }
            case Action.LeanBack:
            {
               float ang = GvrController.Orientation.eulerAngles.x;
               if (ang > 180)
                  ang -= 360;
               return Mathf.Min(1.0f, -ang / 30);
            }
            default:
            {
               return 0;
            }
         }
      }
   }
#endif

   public class Controller
   {
      public string Name;
      public string Description;
      public Control LeftUp;
      public Control LeftLeft;
      public Control LeftRight;
      public Control LeftDown;
      public Control RightUp;
      public Control RightLeft;
      public Control RightRight;
      public Control RightDown;
      public Control LeftTrigger;
      public Control RightTrigger;
      public Control LeanLeft;
      public Control LeanRight;
      public Control LeanForward;
      public Control LeanBack;
      public Control LookLeft;
      public Control LookRight;
      public Control LookUp;
      public Control LookDown;
      public Control X;
      public Control Forward;
      public Control Reverse;
   }

   public Controller[] Controllers;

   Controller FindController(string description)
   {
      foreach (var controller in Controllers)
      {
         if (controller.Description == description)
            return controller;
      }
      
      return null;
   }

   public Controller PickController()
   {
      string[] joysticks = Input.GetJoystickNames();
      int joyNum = 0;
      Controller controller = null;

#if UNITY_ANDROID && !UNITY_EDITOR && UNITY_HAS_GOOGLEVR
      controller = new Controller();
      controller.Name = "Gvr";
      controller.Description = "Gvr";
      controller.LeftUp = new Control();
      controller.LeftLeft = new Control();
      controller.LeftRight = new Control();
      controller.LeftDown = new Control();
      controller.RightUp = new Control();
      controller.RightLeft = new Control();
      controller.RightRight = new Control();
      controller.RightDown = new Control();
      controller.LookLeft = new Control();
      controller.LookRight = new Control();
      controller.LookUp = new Control();
      controller.LookDown = new Control();
      controller.LeftTrigger = new GvrControl(GvrControl.Action.LeftTrigger);
      controller.RightTrigger = new GvrControl(GvrControl.Action.RightTrigger);
      controller.LeanLeft = new GvrControl(GvrControl.Action.LeanLeft);
      controller.LeanRight = new GvrControl(GvrControl.Action.LeanRight);
      controller.LeanForward = new GvrControl(GvrControl.Action.LeanForward);
      controller.LeanBack = new GvrControl(GvrControl.Action.LeanBack);
      controller.X = new GvrControl(GvrControl.Action.X);
      controller.Forward = new GvrControl(GvrControl.Action.Forward);
      controller.Reverse = new GvrControl(GvrControl.Action.Reverse);
#endif

#if !UNITY_ANDROID || UNITY_EDITOR
      foreach (var joystick in joysticks)
      {
//			VZUtl.Log("Joystick = " + joysticks[0]);
         joyNum++;

# if UNITY_PS4 && !UNITY_EDITOR
         if (joystick != "")
         {
            controller = FindController("DS4");
            break;
         }
# else
         controller = FindController(joystick);

         if (controller != null)
            break;
# endif
      }
#endif

      if (controller == null)
         controller = FindController("Keyboard");

      // Set joystick index
      controller.LeftUp.Finalize(joyNum);
      controller.LeftLeft.Finalize(joyNum);
      controller.LeftRight.Finalize(joyNum);
      controller.LeftDown.Finalize(joyNum);
      controller.RightUp.Finalize(joyNum);
      controller.RightLeft.Finalize(joyNum);
      controller.RightRight.Finalize(joyNum);
      controller.RightDown.Finalize(joyNum);
      controller.LeftTrigger.Finalize(joyNum);
      controller.RightTrigger.Finalize(joyNum);
      controller.LeanLeft.Finalize(joyNum);
      controller.LeanRight.Finalize(joyNum);
      controller.LeanForward.Finalize(joyNum);
      controller.LeanBack.Finalize(joyNum);
      controller.LookLeft.Finalize(joyNum);
      controller.LookRight.Finalize(joyNum);
      controller.LookUp.Finalize(joyNum);
      controller.LookDown.Finalize(joyNum);
      controller.X.Finalize(joyNum);
      controller.Forward.Finalize(joyNum);
      controller.Reverse.Finalize(joyNum);

      return controller;
   }
}
