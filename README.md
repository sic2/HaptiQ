A Haptic Device for Graph and Text Exploration by People with Visual Disabilities

===============================================================================

Student: Simone Ivan Conte

Supervisor: Miguel A. Nacenta

# Project Description:

Traditional touchscreen devices do not provide any tactile feedback, being then of scarce usability for people with visual disabilities.
Miguel A. Nacenta and his team at the University of Calgary, now a Lecturer in St Andrews, developed the Haptic TableTop Puck (HTP), an inexpensive tactile feedback input device to be used on digital tabletop surfaces. Friction, height, texture, and malleability are communicated through a combination of properties of the HTP: a rod and a brake pad controlled by two servo motors, and a pressure sensor on the rod. The HTP, however, uses only one actuator on a finger to convey information to the user, which makes it unsuitable for use by people with visual disabilities, since people cannot detect directions and edges of tactile objects.

The aim of this project is to extend the HTP concept and overcome some of its limitations for its use for people with visual disabilities. The extension will consist into using multiple servo motors and rods as well as redesigning the API to support new functionalities (haptic objects, behaviours, etc).
 
My hypothesis is that the extended HTP will facilitate the recognition of directions and edges of displayed objects. The first main objective is to create a physical prototype that is adapted for the specific needs of people with visual disabilities. 
-------------------------------------------------------------------------------

# Objectives

## Primary
    - Design and implement an haptic device for graph exploration by people with visual disabilities
    - Design and implement haptic device for text exploration by people with visual disabilities
    - Develop an API for the Haptic Device
    - Design and implement an application for Graphs exploration
    - Develop an API for WPF client applications

## Secondary

    - Design and implement an application for Cartesian Graph exploration
    - Increase the haptic device resolution

## Tertiary

    - Enable the haptic device to be used collaboratively
    - Enable the haptic device to sense textures

# Hardware used

- Phidgets ServoBoard and InterfaceKit
- Phidgets Servo Motors
- Pressure Sensors
- Etc

# Collaborating

This project is currently under beta development and the designing is constantly changing.

Please do create a branch of the project if a new feature has to be implemented. Also, use the issue tracking on GitHub if any bug is found or new functionality is wanted.

## Bugs
Whenever a bug is found please state the following:
- Assign a short, clear and meaningful name to the bug
- Be precise
- Be clear: explain how to reproduce the problem, step by step, so others can reproduce the bug
- Include only one problem per report
- Attach any screenshot or Log trace if necessary 

-------------------------------------------------------------------------------

# Known issues

- DirectionBehaviour and PulsationBehaviour work only on a 4-MHTP.
- PrinterModels/ does not contain the latest models (18/03/2014).
