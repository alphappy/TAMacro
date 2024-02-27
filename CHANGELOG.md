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
