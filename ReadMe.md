<div align=center>
![kinectLOGO]
</div>

# Introduce
-----
Kinect Gesture Control System(KGCS) Demo is used for the test and teaching, creating by FesianXu.
Including a Voice Assistant Chris and Kinect Gesture Control and Serial port Communicator.

****
# Version Review
-----
## Version 1.0 
Date: `2017/8/17`
**Functions**:
* A Basic Kinect Skeletons drawing.
* A Basic color hands tracking, drawing the color both hands images.
* A Basic CNN model for palm and fist recognition.
* A Basic Voice Assistant **Chris**, who can manage the those task like:
	1. log in and log out, identity authorization.
	2. run and stop the kinect.
	3. refuse the limited authorization operation.
	4. **Voice Services**
* Serial Communication with Pixhawk to control the model car.
* single hand driving.

**Bugs**:
* Left hand in hold single hand driving mode is invalid and in bug.


**ToDO list**:
* add a more efficient hand gesture recognition model than the single spatial CNN model, which is both time-consuming and in high-error rate. May be some model like LSTM-CNN attention based model is ideal.
* rewrite the main thread code structure, it is mess now.
* add more function to **Chris**

![version1.0main]
-----



[version1.0main]: ./ReadMeImgs/v1main.png
[kinectLOGO]: ./ReadMeImgs/kinectLOGO.png