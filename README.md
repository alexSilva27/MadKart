# MadKart

A sample unity 2022 LTS project to help you to get started building a mobile kart or car game. 

## Key points:

1. A kart controller based on a sphere-shaped non-kinematic rigid body (so no wheel colliders or other kind of colliders).
2. A tile-based track system. A track is made of several tiles assembled together. All tiles have the same squared unit size and can be rotated by 90 degrees increments.
3. A simple camera follow controller.
4. A blender file where you can see on how to create more tiles following a non destructive workflow approach by using geometry nodes and other kind of modifiers. 
5. Tracks do not need to belong to unity scenes. A json file is used to specify the layout of a track. This open the possibility for the player to create his own tracks.
6. A unity editor window (BuildMapWindow.cs) so you can generate the track json file according to the content of the currently active scene.
7. Collider.ClosestPoint(Vector3 point) support for non-convex mesh collider (which is not supported by Unity).
8. The ground where the kart is moving on can be of any kind of collider, including non convex mesh colliders.
9. Great physics performance in the editor or on mobile devices even with extensive usage of non-convex mesh colliders.
10. Physics global settings (Project Settings > Physics, Project Settings > Time > Fixed TimeStep) heavily tunned for a smooth kart driving experience.
11. As with most mobile kart games, player do not control kart acceleration or breaking. For steering, press key "A" or "D" or press one of the two buttons on screen.

## How to run in the editor:

Open scene SampleScene and hit button Play.

## How to run on android:

In Build Settings, make sure that platform is set to android and then hit button Build.
