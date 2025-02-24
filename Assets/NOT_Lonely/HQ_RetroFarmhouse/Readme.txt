Hi!

Thank you for purchasing this package!


IMPORTANT!

To achieve the same graphics as in the video demo and screenshots you have to be sure that you:


1. Set your project to Deferred Rendering Path;

2. Set your project Color Space to Linear;

3. Switch your camera to HDR mode (Checkbox in the camera component);

4. Use Post Processing Stack v2, which is available through the Package Manager, and use the presets included (Placed in the \Assets\NOT_Lonely\HQ_RetroFarmhouse\DemoScene\DemoScene_day_Profiles folder)


>>>>>>HOW TO USE SUBSTANCES<<<<<<<

1. First of all download the official plugin called Substance in Unity from the Asset Store: https://assetstore.unity.com/packages/tools/utilities/substance-in-unity-110555
2. Import the Substnace.unitypackage that is placed in \Assets\NOT_Lonely\HQ_RetroFarmhouse\ folder.

Once you made this, just drag and drop any substance material on any mesh and tweak properties by your taste.

Recommendation: it's better to use regular textures instead of substances when you build your project. To do this you can tweak anything in your scene using substance, 
then export preset by pressing appropriate button in the substance interface and use Substance Player (free standalone programm) to generate textures.


>>>>>>Scriptable Render Pipelines<<<<<<<

To use this pack with HDRP or URP you should:
1. Select and convert all the materials that have Standard Shader using the builtin Material Upgrade Tool (Edit > Render Pipeline > Upgrade Selected Materials to High Definition Materials)
2. Import the Unity package placed in the \Assets\NOT_Lonely\HQ_RetroFarmhouse\Shaders\HDRP or URP folder according to the pipeline you use.
3. Select the objects in the scene that are still shown as they use broken shaders, go to the material section and change it's shader with the the same named shader, but from the HDRP or URP section (accordin to the pipeline you use).

>>>>>>SMART MODULE CONSTRUCTOR<<<<<<<

To speed up your level building you can use the integrated Smart Module Constructor system. To do this simply use prefabs placed in \Assets\NOT_Lonely\HQ_RetroFarmhouse\Prefabs\Modules\Smart Module Constructor\
and \Assets\NOT_Lonely\HQ_RetroFarmhouse\Prefabs\Kitchen\
These prefabs have own editor interface, which allows to switch modules right on a place inside your scene, and chose some other options. Also, if you need to
change the material on the module, use this interface instead of assigning materials directly to the objects in the scene.
To show up all the hierarchy of selected module, you can unfold the Input Objects list. Once you click on it, you will see the whole hierarchy in the Hierarchy window. It's not recommended to change anything under the Input Objects list.

The construction system has a NL_MCS_Manager.cs script. This is a solution to cleanup the hierarchy of an assembled building, so all the unused stuff will be removed before the final build of the game.
To use it:
1. Create an empty GameObject.
2. Make all the MCS modules as children to this empty object.
3. Add the NL_MCS_Manager.cs sctipt to this empty object (\Assets\NOT_Lonely\HQ_RetroFarmhouse\Scripts\Modular Construction System\MCS_Manager\).
4. Press the Remove Unused modules button.

>>>>>>DOORS<<<<<<<

1. Make sure that you have a Sphere Collider with the isTrigger enabled on your player's camera.
2. Make sure that your player's camera tag is the same that listed in the door inspector.

If the doors don't work, please check Console for the help info.

All the help information about the Door script 
you can find in tooltips for almost all variables and properties in the inspector.


>>>>>>OBJECT PLACEMENT TOOL<<<<<<<

This package also includes a lite version of the specially designed tool for speed-up level building. You can find instruction for this tool inside the Readme file, placed in the appropriate folder.
One quick tip here: use ctrl + shift + R to rotate selected object by 90 degrees along the Y axis, ctrl + shift + D to deselect all and ctrl + shift + G to snap selected objects to the closest grid point. 
These features are very useful when you building a level with the modular stuff =)


If you don't have Amplify Shader Editor, you will see some warnings about shaders interface, but you should simply ignore this.

That's all!



If you have any other questions, please write an email: support@not-lonely.com

Don't forget to rate this pack at the Asset Store and write a review. It's very important for me!

Thank you!
