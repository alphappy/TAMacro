### TAMacro v0.3.8.2

- There are three new <code>goto</code> conditions:
  - <code>scug hold *type* in *hand*</code>: True if Slugcat is holding an object of the given type.  
    - <code>*type*</code> must be <code>ANY</code>, <code>WEAPON</code>, or an <code>AbstractObjectType</code>.  Does not work for creatures yet.
    - <code>*hand*</code> must be <code>first</code> or <code>second</code>.  Objects in the <code>first</code> hand (player's left, Slugcat's right) will be thrown first.
  - <code>scug want *type*</code>: True if Slugcat wants to pick up (read: has a <code>pickupCandidate</code>) a nearby object of the given <code>*type*</code>.
  - <code>scug *direction* *threshold*</code>: True if Slugcat is beyond a certain <code>x</code> or <code>y</code> in tiles.
    - <code>*direction*</code> must be <code>left of</code>, <code>right of</code>, <code>above</code>, or <code>below</code>.
    - <code>*threshold*</code> must be a decimal number (e.g., <code>30.7</code>).
- Some minor optimizations.

### TAMacro v0.3.5.4

- <code>goto *label* if *condition*</code>, the opposite of <code>goto *label* unless *condition*</code>, is now supported.
- It is now possible to <code>goto</code> a <code>label</code> that is later in the macro.
- Macros now terminate automatically if they run 100 instructions without ticking.
- Macros can now be manually terminated any time (not just while Slugcat is conscious and realized).

### TAMacro v0.3.4.17

- Initial upload to GitHub.
