
using UndertaleModLib.Models;

namespace GMMK;
public class Databases
{
    // TODO: Automatically check for the small variations that are between these things. This is a fucking mess and I am ashamed of myself.
    // If this were a school assignment the teacher would make me write "I will not create giant hardcodeed databases with slight variations between their entries" 1000 times on the blackboard :sob:

    public static Dictionary<string, (EventType, uint)> EventNames = new Dictionary<string, (EventType, uint)>
    {

        //===================================================== STANDALONE EVENTS =========================================================================================
        {"Create", (EventType.Create, 0)},
        {"Create_0", (EventType.Create, 0)},
        {"Create 0", (EventType.Create, 0)},


        //===================================================== STEP EVENTS =========================================================================================
        {"Step", (EventType.Step, (int)EventSubtypeStep.Step)},
        {"Step_0", (EventType.Step, (int)EventSubtypeStep.Step)},
        {"Step 0", (EventType.Step, (int)EventSubtypeStep.Step)},

        {"BeginStep", (EventType.Step, (int)EventSubtypeStep.BeginStep)},
        {"Begin Step", (EventType.Step, (int)EventSubtypeStep.BeginStep)},
        {"Step_1", (EventType.Step, (int)EventSubtypeStep.Step)},
        {"Step 1", (EventType.Step, (int)EventSubtypeStep.Step)},

        {"EndStep", (EventType.Step, (int)EventSubtypeStep.EndStep)},
        {"End Step", (EventType.Step, (int)EventSubtypeStep.EndStep)},
        {"Step_2", (EventType.Step, (int)EventSubtypeStep.Step)},
        {"Step 2", (EventType.Step, (int)EventSubtypeStep.Step)},




        //===================================================== DRAW EVENTS =========================================================================================

        {"Draw", (EventType.Draw, (int)EventSubtypeDraw.Draw)},
        {"Draw_0", (EventType.Draw, (int)EventSubtypeDraw.Draw)},
        {"Draw 0", (EventType.Draw, (int)EventSubtypeDraw.Draw)},

        {"DrawGUI", (EventType.Draw, (int)EventSubtypeDraw.DrawGUI)},
        {"Draw GUI", (EventType.Draw, (int)EventSubtypeDraw.DrawGUI)},
        {"GUI", (EventType.Draw, (int)EventSubtypeDraw.DrawGUI)},
        {"Draw_64", (EventType.Draw, (int)EventSubtypeDraw.DrawGUI)},
        {"Draw 64", (EventType.Draw, (int)EventSubtypeDraw.DrawGUI)},

        {"DrawResize", (EventType.Draw, (int)EventSubtypeDraw.Resize)},
        {"Draw Resize", (EventType.Draw, (int)EventSubtypeDraw.Resize)},
        {"Resize", (EventType.Draw, (int)EventSubtypeDraw.Resize)},
        {"Draw_65", (EventType.Draw, (int)EventSubtypeDraw.Resize)},
        {"Draw 65", (EventType.Draw, (int)EventSubtypeDraw.Resize)},

        {"DrawBegin", (EventType.Draw, (int)EventSubtypeDraw.DrawBegin)},
        {"Draw Begin", (EventType.Draw, (int)EventSubtypeDraw.DrawBegin)},
        {"Draw_72", (EventType.Draw, (int)EventSubtypeDraw.DrawBegin)},
        {"Draw 72", (EventType.Draw, (int)EventSubtypeDraw.DrawBegin)},

        {"DrawEnd", (EventType.Draw, (int)EventSubtypeDraw.DrawEnd)},
        {"Draw End", (EventType.Draw, (int)EventSubtypeDraw.DrawEnd)},
        {"Draw_73", (EventType.Draw, (int)EventSubtypeDraw.DrawEnd)},
        {"Draw 73", (EventType.Draw, (int)EventSubtypeDraw.DrawEnd)},

        {"DrawGUIBegin", (EventType.Draw, (int)EventSubtypeDraw.DrawGUIBegin)},
        {"DrawGUI Begin", (EventType.Draw, (int)EventSubtypeDraw.DrawGUIBegin)},
        {"Draw GUI Begin", (EventType.Draw, (int)EventSubtypeDraw.DrawGUIBegin)},
        {"GUI Begin", (EventType.Draw, (int)EventSubtypeDraw.DrawGUIBegin)},
        {"Draw_74", (EventType.Draw, (int)EventSubtypeDraw.DrawGUIBegin)},
        {"Draw 74", (EventType.Draw, (int)EventSubtypeDraw.DrawGUIBegin)},

        {"DrawGUIEnd", (EventType.Draw, (int)EventSubtypeDraw.DrawGUIEnd)},
        {"DrawGUI End", (EventType.Draw, (int)EventSubtypeDraw.DrawGUIEnd)},
        {"Draw GUI End", (EventType.Draw, (int)EventSubtypeDraw.DrawGUIEnd)},
        {"GUI End", (EventType.Draw, (int)EventSubtypeDraw.DrawGUIEnd)},
        {"Draw_75", (EventType.Draw, (int)EventSubtypeDraw.DrawGUIEnd)},
        {"Draw 75", (EventType.Draw, (int)EventSubtypeDraw.DrawGUIEnd)},

        {"PreDraw", (EventType.Draw, (int)EventSubtypeDraw.PreDraw)},
        {"Draw_76", (EventType.Draw, (int)EventSubtypeDraw.PreDraw)},
        {"Draw 76", (EventType.Draw, (int)EventSubtypeDraw.PreDraw)},

        {"PostDraw", (EventType.Draw, (int)EventSubtypeDraw.PostDraw)},
        {"Draw_77", (EventType.Draw, (int)EventSubtypeDraw.PostDraw)},
        {"Draw 77", (EventType.Draw, (int)EventSubtypeDraw.PostDraw)},

        //===================================================== OTHER EVENTS =========================================================================================

        {"OutsideRoom", (EventType.Step, (int)EventSubtypeOther.OutsideRoom)},
        {"Outside Room", (EventType.Other, (int)EventSubtypeOther.OutsideRoom)},
        {"Other_0", (EventType.Other, (int)EventSubtypeOther.OutsideRoom)},
        {"Other 0", (EventType.Other, (int)EventSubtypeOther.OutsideRoom)},

        {"IntersectBoundary", (EventType.Other, (int)EventSubtypeOther.IntersectBoundary)},
        {"Intersect Boundary", (EventType.Other, (int)EventSubtypeOther.IntersectBoundary)},
        {"Other_1", (EventType.Other, (int)EventSubtypeOther.IntersectBoundary)},
        {"Other 1", (EventType.Other, (int)EventSubtypeOther.IntersectBoundary)},

        {"GameStart", (EventType.Other, (int)EventSubtypeOther.GameStart)},
        {"Game Start", (EventType.Other, (int)EventSubtypeOther.GameStart)},
        {"Other_2", (EventType.Other, (int)EventSubtypeOther.GameStart)},
        {"Other 2", (EventType.Other, (int)EventSubtypeOther.GameStart)},

        {"GameEnd", (EventType.Other, (int)EventSubtypeOther.GameEnd)},
        {"Game End", (EventType.Other, (int)EventSubtypeOther.GameEnd)},
        {"Other_3", (EventType.Other, (int)EventSubtypeOther.GameEnd)},
        {"Other 3", (EventType.Other, (int)EventSubtypeOther.GameEnd)},

        {"RoomStart", (EventType.Other, (int)EventSubtypeOther.RoomStart)},
        {"Room Start", (EventType.Other, (int)EventSubtypeOther.RoomStart)},
        {"Other_4", (EventType.Other, (int)EventSubtypeOther.RoomStart)},
        {"Other 4", (EventType.Other, (int)EventSubtypeOther.RoomStart)},

        {"RoomEnd", (EventType.Other, (int)EventSubtypeOther.RoomEnd)},
        {"Room End", (EventType.Other, (int)EventSubtypeOther.RoomEnd)},
        {"Other_5", (EventType.Other, (int)EventSubtypeOther.RoomEnd)},
        {"Other 5", (EventType.Other, (int)EventSubtypeOther.RoomEnd)},

        {"NoMoreLives", (EventType.Other, (int)EventSubtypeOther.NoMoreLives)},
        {"No More Lives", (EventType.Other, (int)EventSubtypeOther.NoMoreLives)},
        {"Other_6", (EventType.Other, (int)EventSubtypeOther.NoMoreLives)},
        {"Other 6", (EventType.Other, (int)EventSubtypeOther.NoMoreLives)},

        {"AnimationEnd", (EventType.Other, (int)EventSubtypeOther.AnimationEnd)},
        {"Animation End", (EventType.Other, (int)EventSubtypeOther.AnimationEnd)},
        {"Other_7", (EventType.Other, (int)EventSubtypeOther.AnimationEnd)},
        {"Other 7", (EventType.Other, (int)EventSubtypeOther.AnimationEnd)},

        {"EndOfPath", (EventType.Other, (int)EventSubtypeOther.EndOfPath)},
        {"End Of Path", (EventType.Other, (int)EventSubtypeOther.EndOfPath)},
        {"Other_8", (EventType.Other, (int)EventSubtypeOther.EndOfPath)},
        {"Other 8", (EventType.Other, (int)EventSubtypeOther.EndOfPath)},

        {"NoMoreHealth", (EventType.Other, (int)EventSubtypeOther.NoMoreHealth)},
        {"No More Health", (EventType.Other, (int)EventSubtypeOther.NoMoreHealth)},
        {"Other_9", (EventType.Other, (int)EventSubtypeOther.NoMoreHealth)},
        {"Other 9", (EventType.Other, (int)EventSubtypeOther.NoMoreHealth)},


        //===================================================== USER EVENTS =========================================================================================

        {"User0", (EventType.Other, (int)EventSubtypeOther.User0)},
        {"User1", (EventType.Other, (int)EventSubtypeOther.User1)},
        {"User2", (EventType.Other, (int)EventSubtypeOther.User2)},
        {"User3", (EventType.Other, (int)EventSubtypeOther.User3)},
        {"User4", (EventType.Other, (int)EventSubtypeOther.User4)},
        {"User5", (EventType.Other, (int)EventSubtypeOther.User5)},
        {"User6", (EventType.Other, (int)EventSubtypeOther.User6)},
        {"User7", (EventType.Other, (int)EventSubtypeOther.User7)},
        {"User8", (EventType.Other, (int)EventSubtypeOther.User8)},
        {"User9", (EventType.Other, (int)EventSubtypeOther.User9)},
        {"User10", (EventType.Other, (int)EventSubtypeOther.User10)},
        {"User11", (EventType.Other, (int)EventSubtypeOther.User11)},
        {"User12", (EventType.Other, (int)EventSubtypeOther.User12)},
        {"User13", (EventType.Other, (int)EventSubtypeOther.User13)},
        {"User14", (EventType.Other, (int)EventSubtypeOther.User14)},
        {"User15", (EventType.Other, (int)EventSubtypeOther.User15)},
        {"User16", (EventType.Other, (int)EventSubtypeOther.User16)},

        {"User 0", (EventType.Other, (int)EventSubtypeOther.User0)},
        {"User 1", (EventType.Other, (int)EventSubtypeOther.User1)},
        {"User 2", (EventType.Other, (int)EventSubtypeOther.User2)},
        {"User 3", (EventType.Other, (int)EventSubtypeOther.User3)},
        {"User 4", (EventType.Other, (int)EventSubtypeOther.User4)},
        {"User 5", (EventType.Other, (int)EventSubtypeOther.User5)},
        {"User 6", (EventType.Other, (int)EventSubtypeOther.User6)},
        {"User 7", (EventType.Other, (int)EventSubtypeOther.User7)},
        {"User 8", (EventType.Other, (int)EventSubtypeOther.User8)},
        {"User 9", (EventType.Other, (int)EventSubtypeOther.User9)},
        {"User 10", (EventType.Other, (int)EventSubtypeOther.User10)},
        {"User 11", (EventType.Other, (int)EventSubtypeOther.User11)},
        {"User 12", (EventType.Other, (int)EventSubtypeOther.User12)},
        {"User 13", (EventType.Other, (int)EventSubtypeOther.User13)},
        {"User 14", (EventType.Other, (int)EventSubtypeOther.User14)},
        {"User 15", (EventType.Other, (int)EventSubtypeOther.User15)},
        {"User 16", (EventType.Other, (int)EventSubtypeOther.User16)},

        {"User_0", (EventType.Other, (int)EventSubtypeOther.User0)},
        {"User_1", (EventType.Other, (int)EventSubtypeOther.User1)},
        {"User_2", (EventType.Other, (int)EventSubtypeOther.User2)},
        {"User_3", (EventType.Other, (int)EventSubtypeOther.User3)},
        {"User_4", (EventType.Other, (int)EventSubtypeOther.User4)},
        {"User_5", (EventType.Other, (int)EventSubtypeOther.User5)},
        {"User_6", (EventType.Other, (int)EventSubtypeOther.User6)},
        {"User_7", (EventType.Other, (int)EventSubtypeOther.User7)},
        {"User_8", (EventType.Other, (int)EventSubtypeOther.User8)},
        {"User_9", (EventType.Other, (int)EventSubtypeOther.User9)},
        {"User_10", (EventType.Other, (int)EventSubtypeOther.User10)},
        {"User_11", (EventType.Other, (int)EventSubtypeOther.User11)},
        {"User_12", (EventType.Other, (int)EventSubtypeOther.User12)},
        {"User_13", (EventType.Other, (int)EventSubtypeOther.User13)},
        {"User_14", (EventType.Other, (int)EventSubtypeOther.User14)},
        {"User_15", (EventType.Other, (int)EventSubtypeOther.User15)},
        {"User_16", (EventType.Other, (int)EventSubtypeOther.User16)},

        {"Other10", (EventType.Other, (int)EventSubtypeOther.User0)},
        {"Other11", (EventType.Other, (int)EventSubtypeOther.User1)},
        {"Other12", (EventType.Other, (int)EventSubtypeOther.User2)},
        {"Other13", (EventType.Other, (int)EventSubtypeOther.User3)},
        {"Other14", (EventType.Other, (int)EventSubtypeOther.User4)},
        {"Other15", (EventType.Other, (int)EventSubtypeOther.User5)},
        {"Other16", (EventType.Other, (int)EventSubtypeOther.User6)},
        {"Other17", (EventType.Other, (int)EventSubtypeOther.User7)},
        {"Other18", (EventType.Other, (int)EventSubtypeOther.User8)},
        {"Other19", (EventType.Other, (int)EventSubtypeOther.User9)},
        {"Other20", (EventType.Other, (int)EventSubtypeOther.User10)},
        {"Other21", (EventType.Other, (int)EventSubtypeOther.User11)},
        {"Other22", (EventType.Other, (int)EventSubtypeOther.User12)},
        {"Other23", (EventType.Other, (int)EventSubtypeOther.User13)},
        {"Other24", (EventType.Other, (int)EventSubtypeOther.User14)},
        {"Other25", (EventType.Other, (int)EventSubtypeOther.User15)},
        {"Other26", (EventType.Other, (int)EventSubtypeOther.User16)},

        {"Other_10", (EventType.Other, (int)EventSubtypeOther.User0)},
        {"Other_11", (EventType.Other, (int)EventSubtypeOther.User1)},
        {"Other_12", (EventType.Other, (int)EventSubtypeOther.User2)},
        {"Other_13", (EventType.Other, (int)EventSubtypeOther.User3)},
        {"Other_14", (EventType.Other, (int)EventSubtypeOther.User4)},
        {"Other_15", (EventType.Other, (int)EventSubtypeOther.User5)},
        {"Other_16", (EventType.Other, (int)EventSubtypeOther.User6)},
        {"Other_17", (EventType.Other, (int)EventSubtypeOther.User7)},
        {"Other_18", (EventType.Other, (int)EventSubtypeOther.User8)},
        {"Other_19", (EventType.Other, (int)EventSubtypeOther.User9)},
        {"Other_20", (EventType.Other, (int)EventSubtypeOther.User10)},
        {"Other_21", (EventType.Other, (int)EventSubtypeOther.User11)},
        {"Other_22", (EventType.Other, (int)EventSubtypeOther.User12)},
        {"Other_23", (EventType.Other, (int)EventSubtypeOther.User13)},
        {"Other_24", (EventType.Other, (int)EventSubtypeOther.User14)},
        {"Other_25", (EventType.Other, (int)EventSubtypeOther.User15)},
        {"Other_26", (EventType.Other, (int)EventSubtypeOther.User16)},

        {"Other 10", (EventType.Other, (int)EventSubtypeOther.User0)},
        {"Other 11", (EventType.Other, (int)EventSubtypeOther.User1)},
        {"Other 12", (EventType.Other, (int)EventSubtypeOther.User2)},
        {"Other 13", (EventType.Other, (int)EventSubtypeOther.User3)},
        {"Other 14", (EventType.Other, (int)EventSubtypeOther.User4)},
        {"Other 15", (EventType.Other, (int)EventSubtypeOther.User5)},
        {"Other 16", (EventType.Other, (int)EventSubtypeOther.User6)},
        {"Other 17", (EventType.Other, (int)EventSubtypeOther.User7)},
        {"Other 18", (EventType.Other, (int)EventSubtypeOther.User8)},
        {"Other 19", (EventType.Other, (int)EventSubtypeOther.User9)},
        {"Other 20", (EventType.Other, (int)EventSubtypeOther.User10)},
        {"Other 21", (EventType.Other, (int)EventSubtypeOther.User11)},
        {"Other 22", (EventType.Other, (int)EventSubtypeOther.User12)},
        {"Other 23", (EventType.Other, (int)EventSubtypeOther.User13)},
        {"Other 24", (EventType.Other, (int)EventSubtypeOther.User14)},
        {"Other 25", (EventType.Other, (int)EventSubtypeOther.User15)},
        {"Other 26", (EventType.Other, (int)EventSubtypeOther.User16)},



        //===================================================== ASYNC EVENTS =========================================================================================
       
        {"AsyncImageLoaded", (EventType.Other, (int)EventSubtypeOther.AsyncImageLoaded)},
        {"Async Image Loaded", (EventType.Other, (int)EventSubtypeOther.AsyncImageLoaded)},
        {"Async ImageLoaded", (EventType.Other, (int)EventSubtypeOther.AsyncImageLoaded)},
        {"ImageLoaded", (EventType.Other, (int)EventSubtypeOther.AsyncImageLoaded)},
        {"Image Loaded", (EventType.Other, (int)EventSubtypeOther.AsyncImageLoaded)},
        {"Other_60", (EventType.Other, (int)EventSubtypeOther.AsyncImageLoaded)},
        {"Other 60", (EventType.Other, (int)EventSubtypeOther.AsyncImageLoaded)},

        {"AsyncSoundLoaded", (EventType.Other, (int)EventSubtypeOther.AsyncSoundLoaded)},
        {"Async Sound Loaded", (EventType.Other, (int)EventSubtypeOther.AsyncSoundLoaded)},
        {"Async SoundLoaded", (EventType.Other, (int)EventSubtypeOther.AsyncSoundLoaded)},
        {"SoundLoaded", (EventType.Other, (int)EventSubtypeOther.AsyncSoundLoaded)},
        {"Sound Loaded", (EventType.Other, (int)EventSubtypeOther.AsyncSoundLoaded)},
        {"Other_61", (EventType.Other, (int)EventSubtypeOther.AsyncSoundLoaded)},
        {"Other 61", (EventType.Other, (int)EventSubtypeOther.AsyncSoundLoaded)},

        {"AsyncHTTP", (EventType.Other, (int)EventSubtypeOther.AsyncHTTP)},
        {"Async HTTP", (EventType.Other, (int)EventSubtypeOther.AsyncHTTP)},
        {"HTTP", (EventType.Other, (int)EventSubtypeOther.AsyncHTTP)},
        {"Other_62", (EventType.Other, (int)EventSubtypeOther.AsyncHTTP)},
        {"Other 62", (EventType.Other, (int)EventSubtypeOther.AsyncHTTP)},

        {"AsyncDialog", (EventType.Other, (int)EventSubtypeOther.AsyncDialog)},
        {"Async Dialog", (EventType.Other, (int)EventSubtypeOther.AsyncDialog)},
        {"Dialog", (EventType.Other, (int)EventSubtypeOther.AsyncDialog)},
        {"Other_63", (EventType.Other, (int)EventSubtypeOther.AsyncDialog)},
        {"Other 63", (EventType.Other, (int)EventSubtypeOther.AsyncDialog)},

        {"AsyncIAP", (EventType.Other, (int)EventSubtypeOther.AsyncIAP)},
        {"Async IAP", (EventType.Other, (int)EventSubtypeOther.AsyncIAP)},
        {"IAP", (EventType.Other, (int)EventSubtypeOther.AsyncIAP)},
        {"Other_66", (EventType.Other, (int)EventSubtypeOther.AsyncIAP)},
        {"Other 66", (EventType.Other, (int)EventSubtypeOther.AsyncIAP)},

        {"AsyncCloud", (EventType.Other, (int)EventSubtypeOther.AsyncCloud)},
        {"Async Cloud", (EventType.Other, (int)EventSubtypeOther.AsyncCloud)},
        {"Cloud", (EventType.Other, (int)EventSubtypeOther.AsyncCloud)},
        {"Other_67", (EventType.Other, (int)EventSubtypeOther.AsyncCloud)},
        {"Other 67", (EventType.Other, (int)EventSubtypeOther.AsyncCloud)},

        {"AsyncNetworking", (EventType.Other, (int)EventSubtypeOther.AsyncNetworking)},
        {"Async Networking", (EventType.Other, (int)EventSubtypeOther.AsyncNetworking)},
        {"Networking", (EventType.Other, (int)EventSubtypeOther.AsyncNetworking)},
        {"Other_68", (EventType.Other, (int)EventSubtypeOther.AsyncNetworking)},
        {"Other 68", (EventType.Other, (int)EventSubtypeOther.AsyncNetworking)},

        {"AsyncSteam", (EventType.Other, (int)EventSubtypeOther.AsyncSteam)},
        {"Async Steam", (EventType.Other, (int)EventSubtypeOther.AsyncSteam)},
        {"Steam", (EventType.Other, (int)EventSubtypeOther.AsyncSteam)},
        {"Other_69", (EventType.Other, (int)EventSubtypeOther.AsyncSteam)},
        {"Other 69", (EventType.Other, (int)EventSubtypeOther.AsyncSteam)},

        {"AsyncSocial", (EventType.Other, (int)EventSubtypeOther.AsyncSocial)},
        {"Async Social", (EventType.Other, (int)EventSubtypeOther.AsyncSocial)},
        {"Social", (EventType.Other, (int)EventSubtypeOther.AsyncSocial)},
        {"Other_70", (EventType.Other, (int)EventSubtypeOther.AsyncSocial)},
        {"Other 70", (EventType.Other, (int)EventSubtypeOther.AsyncSocial)},

        {"AsyncPushNotification", (EventType.Other, (int)EventSubtypeOther.AsyncPushNotification)},
        {"Async Push Notification", (EventType.Other, (int)EventSubtypeOther.AsyncPushNotification)},
        {"Async PushNotification", (EventType.Other, (int)EventSubtypeOther.AsyncPushNotification)},
        {"PushNotification", (EventType.Other, (int)EventSubtypeOther.AsyncPushNotification)},
        {"Push Notification", (EventType.Other, (int)EventSubtypeOther.AsyncPushNotification)},
        {"Other_71", (EventType.Other, (int)EventSubtypeOther.AsyncPushNotification)},
        {"Other 71", (EventType.Other, (int)EventSubtypeOther.AsyncPushNotification)},

        {"AsyncSaveAndLoad", (EventType.Other, (int)EventSubtypeOther.AsyncSaveAndLoad)},
        {"Async SaveAndLoad", (EventType.Other, (int)EventSubtypeOther.AsyncSaveAndLoad)},
        {"Async Save And Load", (EventType.Other, (int)EventSubtypeOther.AsyncSaveAndLoad)},
        {"Async Save Load", (EventType.Other, (int)EventSubtypeOther.AsyncSaveAndLoad)},
        {"AsyncSaveLoad", (EventType.Other, (int)EventSubtypeOther.AsyncSaveAndLoad)},
        {"Async SaveLoad", (EventType.Other, (int)EventSubtypeOther.AsyncSaveAndLoad)},
        {"SaveAndLoad", (EventType.Other, (int)EventSubtypeOther.AsyncSaveAndLoad)},
        {"Save And Load", (EventType.Other, (int)EventSubtypeOther.AsyncSaveAndLoad)},
        {"Save Load", (EventType.Other, (int)EventSubtypeOther.AsyncSaveAndLoad)},
        {"Other_72", (EventType.Other, (int)EventSubtypeOther.AsyncSaveAndLoad)},
        {"Other 72", (EventType.Other, (int)EventSubtypeOther.AsyncSaveAndLoad)},

        {"AsyncAudioRecording", (EventType.Other, (int)EventSubtypeOther.AsyncAudioRecording)},
        {"Async Audio Recording", (EventType.Other, (int)EventSubtypeOther.AsyncAudioRecording)},
        {"Async AudioRecording", (EventType.Other, (int)EventSubtypeOther.AsyncAudioRecording)},
        {"AudioRecording", (EventType.Other, (int)EventSubtypeOther.AsyncAudioRecording)},
        {"Audio Recording", (EventType.Other, (int)EventSubtypeOther.AsyncAudioRecording)},
        {"Other_73", (EventType.Other, (int)EventSubtypeOther.AsyncAudioRecording)},
        {"Other 73", (EventType.Other, (int)EventSubtypeOther.AsyncAudioRecording)},

        {"AsyncAudioPlayback", (EventType.Other, (int)EventSubtypeOther.AsyncAudioPlayback)},
        {"Async Audio Playback", (EventType.Other, (int)EventSubtypeOther.AsyncAudioPlayback)},
        {"Async AudioPlayback", (EventType.Other, (int)EventSubtypeOther.AsyncAudioPlayback)},
        {"Async Playback", (EventType.Other, (int)EventSubtypeOther.AsyncAudioPlayback)},
        {"AsyncPlayback", (EventType.Other, (int)EventSubtypeOther.AsyncAudioPlayback)},
        {"AudioPlayback", (EventType.Other, (int)EventSubtypeOther.AsyncAudioPlayback)},
        {"Audio Playback", (EventType.Other, (int)EventSubtypeOther.AsyncAudioPlayback)},
        {"Playback", (EventType.Other, (int)EventSubtypeOther.AsyncAudioPlayback)},
        {"Other_74", (EventType.Other, (int)EventSubtypeOther.AsyncAudioPlayback)},
        {"Other 74", (EventType.Other, (int)EventSubtypeOther.AsyncAudioPlayback)},

        {"AsyncSystem", (EventType.Other, (int)EventSubtypeOther.AsyncSystem)},
        {"Async System", (EventType.Other, (int)EventSubtypeOther.AsyncSystem)},
        {"System", (EventType.Other, (int)EventSubtypeOther.AsyncSystem)},
        {"Other_75", (EventType.Other, (int)EventSubtypeOther.AsyncSystem)},
        {"Other 75", (EventType.Other, (int)EventSubtypeOther.AsyncSystem)},
    };




}