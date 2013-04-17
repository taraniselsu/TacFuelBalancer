A Fuel Balancer from Thunder Aerospace Corporation (TAC),
designed by Taranis Elsu.

For use with the Kerbal Space Program, http://kerbalspaceprogram.com/

This mod is made available under the Attribution-NonCommercial-ShareAlike 3.0 (CC BY-NC-SA
3.0) creative commons license. See the LICENSE.txt file.

Source code can be found at https://github.com/taraniselsu/TacFuelBalancer

===== Features =====
* Transfer a resource into a part, drawing an equal amount from each other part.
* Transfer a resource out of a part, tranferring an equal amount into each other part.
* Enable balance mode to transfer a resource such that all parts are the same percentage
  full.
* Lock a part, so that none of the resource will be transferred into or out of the part.
  This does not prevent other systems, like engines, from drawing resources from the part.
  It only disallows this system from transferring the resource.

Note that it can transfer any resource that uses the "pump" resource transfer mode,
including liquid fuel, oxidizer, electric charge, and RCS fuel; but not resources such
as solid rocket fuel.

===== How to use =====
Add the part to your vessel.
Add the action to an action group, if desired.
Open the GUI using the action group or by right clicking the part.

* Lock - prevents it from transferring the resource into or out of the part.
* In - transfers the resource into the part, taking an equal amount from each other part.
* Out - transfers the resource out of the part, putting an equal amount in each other part.
* Balance - transfers the resource around such that all parts are the same percentage full.

Click the "C" button to bring up the config menu.
Click the "?" button to bring up the help/about menu.
Click the "X" button to close the window.

===== Installation procedure =====
1) copy everything from the Parts directory to the Parts directory in the game.
2) copy everything from the Plugins directory to the Plugins directory in the game, creating
     the directory if needed.

Optional:
Add the following lines to any part that you want to have the Fuel Balancer functionality:

MODULE
{
	name = TacFuelBalancer
}
