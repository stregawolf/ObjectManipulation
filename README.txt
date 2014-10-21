--------------------
Object Manipulation
--------------------

------------
Description:
------------
This prototype showcases a simple object manipulation scheme that tries to give
the user the feeling of having telekinesis. All interactions in this prototype are 
preformed by moving the camera and preforming simple gestures such as shaking and nodding.

For this prototype I implemented a simple mouse based camera control scheme, 
a head gesture recognition system, and an object selection interface. This prototype
could be easily modified to support a VR headset such that the gesture recognition system
detects the user's actual head motions.

WebPlayer Build: http://bit.ly/1sJoTxS
Unity Version: 4.5.4f1 Free

---------
Controls:
---------
- Center the camera on an object in the scene to begin selecting it. 
- Stay focused on the object until it is highlighted green and begins to float.
- Once selected, the object will float towards the location of your gaze at a fixed distance.
- Gently nod the camera up and down to bring the object close to the camera for examination.
- Gently nod the camera again to move it back to its original distance.
- Gently shake the camera left and right to deselect the object and drop it.
- Only one object can be selected at a time.

- R key to restart the application

-------------------
External Libraries:
-------------------
- OutlineShader, taken from the Unity Community Wiki to provide some user feedback.
- Leantween, for simple graphics effects - tweening the outline to give a glowing pulse effect.
- UnityVS 2010, prefer using Visual Studios over Monodevelop.

-------------
Known Issues:
-------------
- The outlines for the cylinder and cube models are not completely connected, 
  due to the way the outline shader works. The default Unity cylinder and cube 
  primatives do not share vertices and vertex normals causing the expansion 
  method for outlining used by the shader to somewhat fail.
- Gestures may sometimes not respond accurately. Preforming the gestures more 
  slowly may help.

--------
Contact:
--------
Vu Ha
vu@vu-ha.com