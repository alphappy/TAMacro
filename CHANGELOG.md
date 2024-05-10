### TAMacro 0.8.4.4 (preview)
- **BREAKING CHANGE:** `!execute` now accepts Unix-style relative paths.
  - Existing `!execute` commands which were absolute paths in prior versions will be interpreted as relative paths in this version.
  - Existing absolute paths must now start with `/` (e.g., `>execute foo/bar/baz` must be changed to `>execute /foo/bar/baz`) or be converted to relative paths.
- Display panel now indicates whether recording is in progress.
- Exceptions rewritten a bit.
- Remove leftover debug logging (hopefully for the last time).

### TAMacro 0.8.3.29

- TAMacro's interface has been overhauled and is now controllable by mouse input!  Most keyboard shortcuts will still continue to function.
  - Additionally, the control panel and the macro panel can now be separated.
  - The macro panel also scrolls through the macro as it executes instead of becoming so long it falls off the screen.
	- KNOWN ISSUE: If `label`-`goto` loops are involved, this can cause rapid flashing.
- It is now possible to spontaneously `!get` an item.  See [the wiki](https://github.com/alphappy/TAMacro/wiki/Cheat#get) for details.
- Start position is now saved as a `!warp` command, followed by 20 ticks of neutral input, when recording a macro.
  - This is far from accurately saving Slugcat's state, which is a planned separately implemented command.
- Macros are now automatically reloaded when the simulation starts.
- Some controls can now be rebound in the Remix settings.
- Font can now be toggled in the Remix settings.
- The entire directory path is now shown on the control panel instead of just the current directory name.
- KNOWN ISSUE: `!warp`ing to another room sometimes causes Slugcat's torso to become invisible.
- KNOWN ISSUE: Dying during macro execution sometimes makes the game not respond to `PAUSE` input, resulting in a softlock.
- KNOWN ISSUE: Infinite instruction loops freeze the application immediately now (instead of on simulation end).
- Bugfix: Macro execution should no longer carry over between simulations.
- Removed even more leftover logging.

### TAMacro 0.7.2.0

- BREAKING CHANGE: `scug located` now requires specifying positions in units rather than tiles.
- It is now possible for a macro to `!warp` Slugcat to a specific position.  See [the wiki](https://github.com/alphappy/TAMacro/wiki/Cheat#warp) for details.
  - Warp Menu must be enabled to warp to another room.
  - KNOWN ISSUE: Warping to a non-existent room freezes the simulation.  Pausing and warping manually unfreezes it.
  - This is the first of several planned *cheat* commands which will begin with `!` to distinguish them from other commands.
- TAMacro now uses the `devconsolas` font if Dev Console is enabled.
- The names of recorded macros now use Unix timestamps instead of random numbers.
- Removed some debugging logging.
- Bugfix: TAMacro should no longer send input to, or record input from, AI-controlled Slugcats or Slugpups.

### TAMacro 0.6.0.3

- TAMacro can now record inputs!  Toggle recording with `F7`.
  - Recorded macros are created in `recorded.tmc`.  You will have to reload (with `F5`) after recording to run the macro.
  - This is still very much a work in progress.  There are probably some things that don't get recorded perfectly.

### TAMacro 0.5.0.16

- It is now possible for macros to `>execute` each other!  See [the wiki](https://github.com/alphappy/TAMacro/wiki/Command#execute) for details.
  - There are several known issues with this command, including one freeze, which will be addressed in a future update.  See [the wiki](https://github.com/alphappy/TAMacro/wiki/Known-issues) for details.
- Some simplifications made to the `Macro` object which might slightly speed up loading macros.

### TAMacro v0.4.1.1

- Cookbook names in-game no longer have the `.tmc` extension.
- TAMacro will no longer recreate the `main.tmc` file unless the entire `TAMacro` folder is missing.
- Removed console log spam.

### TAMacro v0.4.0.24

- TAMacro now reads all `.tmc` files and subfolders in the `TAMacro` folder.  The in-game interface now allows navigating through these folders and files in the same way it previously navigated through all macros.
- BREAKING CHANGE: the [position condition](https://github.com/alphappy/TAMacro/wiki/Condition#scug-located) must now start with `scug located`.
- The display panel now shows the current page number and count.

### TAMacro v0.3.8.2

- There are three new <code>goto</code> conditions: `scug hold`, `scug want`, and a position condition.  See [the wiki](https://github.com/alphappy/TAMacro/wiki/Condition) for details.
- Some minor optimizations.

### TAMacro v0.3.5.4

- <code>goto *label* if *condition*</code>, the opposite of <code>goto *label* unless *condition*</code>, is now supported.
- It is now possible to <code>goto</code> a <code>label</code> that is later in the macro.
- Macros now terminate automatically if they run 100 instructions without ticking.
- Macros can now be manually terminated any time (not just while Slugcat is conscious and realized).

### TAMacro v0.3.4.17

- Initial upload to GitHub.
