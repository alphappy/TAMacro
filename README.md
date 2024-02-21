# TAMacro - WIP TAS/Macro tool for Rain World Remix

**TAMacro** is a WIP tool for writing macros for experimenting with movement and eventually [TAS](https://en.wikipedia.org/wiki/Tool-assisted_speedrun)es.
The mod can be downloaded [from the Workshop](https://steamcommunity.com/sharedfiles/filedetails/?id=3163948083) (and eventually from here on the GitHub).

TAMacro is in a very early alpha state and is nowhere near feature-complete.
This guide is for TAMacro 0.3.4.17.

## How to use
Install and enable TAMacro like any other Workshop mod.
Once the simulation is running (i.e., in Story/Arena/Expedition), a display panel will appear that can be moved with the backslash key.

TAMacro currently only reads from one .tmc file (TAMacro cookbook) at <code>%appdata%\..\LocalLow\Videocult\Rain World\ModConfigs\TAMacro\main.tmc</code> on Windows systems.
The exact file path is printed to the console log (<code>Steam\steamapps\common\Rain World\consoleLog.txt</code>) as Rain World starts up.
Pressing <code>F5</code> while the simulation is running reads from this file again.
Any errors that occur while reading the file will be printed to the console log and do not currently show up in-game.

### Controls
- <code>F1</code>: Previous page
- <code>F2</code>: Interrupt currently running macro
- <code>F3</code>: Next page
- <code>F5</code>: Reload macros from file
- <code>\\</code>: Move display panel
- <code>1</code> - <code>9</code> and <code>0</code>: Run macro from current page

## Writing macros
You can edit cookbook files with any external text editor and make TAMacro reload them by pressing <code>F5</code> while the simulation is running.

Each cookbook must start with *cookbook metadata* - right now, this just means you need to put <code>//PARSER: 1</code> as the first line
(in the future, this will allow for different sets of rules to be used to parse macros).

Following the cookbook metadata can be any number of *macros*, which can each have their own metadata defined by lines that start with <code>/</code>.
The only metadata that matters right now is <code>/NAME</code>, which must be unique for each macro.
This is an example of a valid cookbook:
<pre>
//PARSER: 1
/NAME: throw-boosted eihop to the right
RDJ~2
R~10
LT
RJ
RDJT~40
</pre>
This book contains only one macro which performs a throw-boosted [eihop](https://rwtechwiki.github.io/docs/movement/extendedslideinstanthop/) to the right.
Each line of the macro is an input, which may be some combination of <code>LRUDJGTM</code> followed by <code>~</code> and the number of ticks to hold that input (which, if omitted, defaults to 1 tick).
<code>LR</code> can also be replaced with <code>FB</code> for *forward* and *backward* relative to the direction Slugcat is facing when the macro is executed:
<pre>
/NAME: throw-boosted eihop
FDJ~2
F~10
BT
FJ
FDJT~40
</pre>
It is also possible to define *labels* and use *goto* statements for code flow control.
Statements, as opposed to inputs, must begin with <code>></code>:
<pre>
/NAME: REFERENCE EIJUMP
U
>label 1
  L~9
  LG
>goto 1 unless scug touch wall
R~15
RDJ~2
R~12
LT
RJ~5
>label 2
  RJ
>goto 2 unless scug touch floor
</pre>
This macro moves Slugcat left while pressing <code>GRAB</code> every 10th tick until they reach a wall,
then moves them right slightly and performs an [eijump](https://rwtechwiki.github.io/docs/movement/extendedslideinstantjump/) to the right.
After the jump, it holds <code>RIGHT</code> and <code>JUMP</code> only until Slugcat contacts the floor as a way to guage distance covered.

Note also that indentation may be used anywhere (not just between labels and gotos); all leading and trailing spaces are ignored by the default parser.

Labels may be given any alphanumeric name and then jumped to with <code>goto *label* unless *condition*</code> statements.
Only a few conditions are currently supported:
- <code>scug touch wall</code>: True if Slugcat touches vertical geometry.
- <code>scug touch floor</code>: True if Slugcat touches horizontal geometry below.
- <code>scug touch ceiling</code>: True if Slugcat touches horizontal geometry above.

## Known issues
- Some things are printed to the console log more than once.
- The display panel text doesn't look great when the labels have a lot of text.
- The display panel is hard to see against some backgrounds.
- Display panel resets when the simulation restarts.

## Planned features
- Load from multiple files.
- Allow rebinding controls.
- Allow configuration of display panel's appearance.
- Allow interfering with a macro during execution with manual inputs.  Currently, player input is disabled entirely during macro execution.
- Allow macros to execute each other.
- Introduce other conditions to check for flow control - at least the following:
  - when scug exits pipe
  - when scug reaches a certain x or y
  - when scug is holding an object
  - when scug collides with a creature
- Expose an API for creating custom commands.
- Support fastroll.
- Support analog input.
