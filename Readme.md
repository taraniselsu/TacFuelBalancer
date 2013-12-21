TAC Fuel Balancer
===================

A Fuel Balancer from Thunder Aerospace Corporation (TAC), designed by Taranis Elsu.

For use with the Kerbal Space Program, http://kerbalspaceprogram.com/

This mod is made available under the Attribution-NonCommercial-ShareAlike 3.0 (CC BY-NC-SA 3.0) creative commons license. See the LICENSE.txt file.

Source code can be found at https://github.com/taraniselsu/TacFuelBalancer

### Features
* Transfer a resource into a part, drawing an equal amount from each other part.
* Transfer a resource out of a part, transferring an equal amount into each other part.
* Dump a resource out of a part. Note that the resource is lost, never to be found.
* When still Prelaunch (or Landed): edit the amount of a resource loaded in a part. Works
  on all resources, even solid rocket fuel.
* Enable balance mode to transfer a resource such that all parts are the same percentage
  full.
* Lock a part, so that none of the resource will be transferred into or out of the part.

Note that it can transfer any resource that uses the "pump" resource transfer mode,
including liquid fuel, oxidizer, electric charge, Kethane, and RCS fuel; but not resources
such as solid rocket fuel.

This system does not consume power itself, but the vessel is required to have power and
required to be controllable (have a probe core or at least one Kerbal on-board). Otherwise,
everything is disabled.

### How to use
Open the GUI using the button along the screen edge. It defaults to the top right-hand corner,
but can be dragged to anywhere along any edge.

Click the button at the end of a row, and:
* Highlight - highlights the part so you can find it.
* Edit - edit the amount in the part (only available Prelaunch or when Landed).
* Lock - prevents it from transferring the resource into or out of the part.
* In - transfers the resource into the part, taking an equal amount from each other part.
* Out - transfers the resource out of the part, putting an equal amount in each other part.
* Balance - transfers the resource around such that all parts being balanced are the same
  percentage full.
* Balance All - balances all parts that are not in one of the other modes (In, Out, Lock, etc).

Click the "S" button to bring up the settings menu.
Click the "?" button to bring up the help/about menu.
Click the "X" button to close the window.

### Settings
* Maximum Fuel Flow Rate - controls how quickly fuel is transferred around. This limits each
  action to only transfer up to the selected amount.
* Fuel Warning Level - warns (yellow) when a resource drops below this percentage of capacity.
* Fuel Warning Level - warns (red) when a resource drops below this percentage of capacity.
* Show <whatever> - toggles the display of the columns on the main window.
* Balance In's - when this is enabled, it will balance the resource level between parts that are
  set to In. Note that this can cause the resource level in a part to drop until it evens out with
  the other parts, then it will start increasing again.
* Balance Out's - when this is enabled, it will balance the resource level between parts that
  are set to Out. Note that this can cause the resource level in a part to rise until it evens out
  with the other parts, then it will start decreasing again.

### Installation procedure
1) Copy everything in the GameData directory to the {KSP}/GameData directory.
