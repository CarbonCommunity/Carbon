# The carbon bootloading process
Carbon uses a three stage bootloader process, each of the stages is a standalone .dll
file.

## Carbon.Doorstop
Loads before the Unity environment and it's main goal is to publicize the game's C# code
so it is available to be used by the plugins. 

## Carbon.Stub
Is loaded by the game's built-in Harmony Loader and it's goal is to act as a stub, this
was needed when Facepunch created a new Harmony loader based on Cecil which dybamically
changed the assembly names. This module will also move any other harmony plugins on the
native folder to cabon's harmony folder so we can manage them.

## Carbon.Loader
This is the main loader which deals with all the external assembly resolving, loading
and "unloading". It will also apply some core harmony patches.