UnityScreenSaver
================

Unity でスクリーンセーバーを作れる？

Can I make a screen saver with Unity?

----

Files for Android
-----------------

For Android screen saver (known as Daydream but not VR platform),
I made these files:

- Assets/
  - Plugins/
    - Android/
      - AndroidManifest.xml
      - UnityPlayerDreamService.kt
      - UnityPlayerDream.kt

This Daydream can close itself by C# managed code. For example,
see **Quit()** method on **Assets/QuitTimerScript.cs**.

License
-------

MIT
